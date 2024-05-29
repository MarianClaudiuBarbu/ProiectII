using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BackEnd.Models;

namespace BackEnd.Controllers
{
    public class PatientController : Controller
    {
        private readonly IMongoCollection<PatientModel> _patients;
        private readonly IMongoCollection<AppointmentsModel> _appointments;

        public PatientController(IMongoCollection<AppointmentsModel> appointments, IMongoCollection<PatientModel> patients)
        {
            _appointments = appointments;
            _patients = patients;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View();
            }

            var hashedPassword = HashPassword(password);
            var patient = await _patients.Find(p => p.Email == email && p.Password == hashedPassword).FirstOrDefaultAsync();

            if (patient == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, patient.Email)
            };

            var identity = new ClaimsIdentity(claims, "Cookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);

            return RedirectToAction("AfterLogin");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            var existingPatient = await _patients.Find(p => p.Email == registerViewModel.Email).FirstOrDefaultAsync();
            if (existingPatient != null)
            {
                ModelState.AddModelError("Email", "Email already in use.");
                return View(registerViewModel);
            }

            var patient = new PatientModel
            {
                Email = registerViewModel.Email,
                Age = registerViewModel.Age,
                Name = registerViewModel.Name,
                Password = HashPassword(registerViewModel.Password)
            };

            await _patients.InsertOneAsync(patient);

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AfterLogin()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Appointments()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Appointments(AppointmentsModel appointmentsModel)
        {
            if (!ModelState.IsValid)
            {
                return View(appointmentsModel);
            }

            var existingAppointment = await _appointments
                .Find(a => a.Data == appointmentsModel.Data && a.Hour == appointmentsModel.Hour)
                .FirstOrDefaultAsync();

            if (existingAppointment != null)
            {
                ModelState.AddModelError("Data", "Appointment already exists at this time.");
                return View(appointmentsModel);
            }

            var appointment = new AppointmentsModel
            {
                Name = appointmentsModel.Name,
                Data = appointmentsModel.Data,
                Hour = appointmentsModel.Hour
            };

            await _appointments.InsertOneAsync(appointment);

            return RedirectToAction("AfterLogin");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPut]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var filter = Builders<PatientModel>.Filter.And(
                Builders<PatientModel>.Filter.Eq(x => x.Email, changePasswordModel.Email),
                Builders<PatientModel>.Filter.Eq(x => x.Password, HashPassword(changePasswordModel.OldPassword))
            );
            var update = Builders<PatientModel>.Update.Set(x => x.Password, HashPassword(changePasswordModel.NewPassword));
            var result = await _patients.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
            {
                return BadRequest("Failed to update password. Please check your old password.");
            }

            return Ok("Password updated successfully.");
        }

        [HttpGet]
        public IActionResult DeleteUser()
        {
            return View();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(LoginViewModel loginViewModel)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest(new { success = false, errors = new[] { "User not authenticated." } });
            }

            var existingPatient = await _patients.Find(p => p.Email == userEmail).FirstOrDefaultAsync();

            if (existingPatient == null)
            {
                return BadRequest(new { success = false, errors = new[] { "User not found." } });
            }

            var deleteResult = await _patients.DeleteOneAsync(p => p.Id == existingPatient.Id);

            if (deleteResult.DeletedCount == 0)
            {
                return StatusCode(500, new { success = false, errors = new[] { "Failed to delete user." } });
            }

            await HttpContext.SignOutAsync("Cookies");

            return Ok(new { success = true });
        }

    }
}
