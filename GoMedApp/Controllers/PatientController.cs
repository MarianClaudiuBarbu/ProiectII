using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using GoMedApp.Models;

namespace GoMedApp.Controllers
{
    public class PatientController : Controller
    {
        private readonly IMongoCollection<PatientModel> _patients;

        public PatientController(IMongoCollection<PatientModel> patients)
        {
            _patients = patients;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
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
                return RedirectToAction("Login", new { ErrorMessage = "Email and password are required." });
            }

            var hashedPassword = HashPassword(password);

            var patient = await _patients.Find(p => p.Email == email && p.Password == hashedPassword).FirstOrDefaultAsync();

            if (patient == null)
            {
                return RedirectToAction("Login", new { ErrorMessage = "Invalid username or password." });
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, patient.Email)
        };
      
            var identity = new ClaimsIdentity(claims, "Cookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookie", principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            var existingPatient = await _patients.Find(p => p.Email == registerViewModel.Email).FirstOrDefaultAsync();
            if (existingPatient != null)
            {
                ModelState.AddModelError("Email", "Email already in use.");
                return View("Register", registerViewModel);
            }
            if (ModelState.IsValid)
            {
                var patient = new PatientModel
                {
                    Email = registerViewModel.Email,
                    Age = registerViewModel.Age,
                    Name = registerViewModel.Name,
                    Password = HashPassword(registerViewModel.HashedPassword)
                };

                await _patients.InsertOneAsync(patient);
                return RedirectToAction("Login", "Patient");
            }
            return View("Register", registerViewModel);
        }
    }
}