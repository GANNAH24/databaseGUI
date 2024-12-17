using Microsoft.AspNetCore.Mvc;
using MS3GUI.Models;
using System;
using System.Threading.Tasks;

namespace MS3GUI.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly DatabaseProjectContext _context;

        public RegistrationController(DatabaseProjectContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(string role, string firstName, string lastName, string gender, DateTime birthDate, string country, string culturalBackground, string qualification, string expertise, string email, string password)
        {
            if (role == "learner")
            {
                var learner = new Learner
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Gender = gender,
                    BirthDate = DateOnly.FromDateTime(birthDate), // Convert DateTime to DateOnly
                    Country = country,
                    CulturalBackground = culturalBackground,
                    Email = email,
                    Password = password
                };

                _context.Learners.Add(learner);
            }
            else if (role == "instructor")
            {
                var instructor = new Instructor
                {
                    InstructorName = firstName,
                    LatestQualification = qualification,
                    ExpertiseArea = expertise,
                    Email = email,
                    Password = password
                };

                _context.Instructors.Add(instructor);
            }
            else
            {
                ViewBag.Message = "Invalid role specified.";
                return View("Index");
            }

            try
            {
                await _context.SaveChangesAsync();
                ViewBag.Message = "Registration successful.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
            }

            return View("Index");
        }
    }
}
