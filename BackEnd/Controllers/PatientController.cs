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
using Microsoft.AspNetCore.Authentication.Cookies;


namespace BackEnd.Controllers
{
    public class PatientController : Controller
    {
        private readonly IMongoCollection<PatientModel> _patients;
        private readonly IMongoCollection<AppointmentsModel> _appointments;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IMongoCollection<AppointmentsModel> appointments, IMongoCollection<PatientModel> patients, ILogger<PatientController> logger)
        {
            _appointments = appointments;
            _patients = patients;
            _logger = logger;
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

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            _logger.LogInformation("ChangePassword action called");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is not valid");
                return View(changePasswordModel);
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("User not authenticated");
                ModelState.AddModelError("", "User not authenticated.");
                return View("Error", changePasswordModel);
            }

            var existingPatient = await _patients.Find(p => p.Email == userEmail).FirstOrDefaultAsync();
            if (existingPatient == null)
            {
                _logger.LogWarning("User not found");
                ModelState.AddModelError("", "User not found.");
                return View(changePasswordModel);
            }

            var hashedOldPassword = HashPassword(changePasswordModel.OldPassword);
            if (existingPatient.Password != hashedOldPassword)
            {
                _logger.LogWarning("Incorrect old password");
                ModelState.AddModelError("", "Incorrect old password.");
                return View(changePasswordModel);
            }

            var hashedNewPassword = HashPassword(changePasswordModel.NewPassword);
            var updateDefinition = Builders<PatientModel>.Update.Set(p => p.Password, hashedNewPassword);

            var updateResult = await _patients.UpdateOneAsync(p => p.Id == existingPatient.Id, updateDefinition);

            if (updateResult.ModifiedCount == 0)
            {
                _logger.LogError("Failed to update password");
                ModelState.AddModelError("", "Failed to update password.");
                return View(changePasswordModel);
            }

            _logger.LogInformation("Password updated successfully");

            return RedirectToAction("AfterLogin");
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
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index");
            
        }


        [HttpGet]
        public IActionResult AfterLogin()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

    }
}
