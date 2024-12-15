using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MS3GUI.Models;
using System.Threading.Tasks;

namespace MS3GUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseProjectContext _context;

        public AccountController(DatabaseProjectContext context)
        {
            _context = context;
        }

        // This action serves the initial registration page with role selection buttons
        public IActionResult Register()
        {
            return View();
        }

        // This action handles the button click for role selection
        [HttpPost]
        public IActionResult RegisterRole(string role)
        {
            if (role == "Learner")
            {
                return RedirectToAction("RegisterLearner"); // Redirect to Learner registration form
            }
            else if (role == "Instructor")
            {
                return RedirectToAction("RegisterInstructor"); // Redirect to Instructor registration form
            }
            else
            {
                return RedirectToAction("Register"); // Default return if no role is selected
            }
        }

        // This action handles Learner Registration
        [HttpGet]
        public IActionResult RegisterLearner()
        {
            return View(); // A view for learner registration form
        }

        // This action handles Instructor Registration
        [HttpGet]
        public IActionResult RegisterInstructor()
        {
            return View(); // A view for instructor registration form
        }

        // This action handles Learner Registration submission (POST)
        [HttpPost]
        public async Task<IActionResult> RegisterLearner(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Execute AddLearner stored procedure
                var parameters = new[]
                {
                    new SqlParameter("@FirstName", model.FirstName),
                    new SqlParameter("@LastName", model.LastName),
                    new SqlParameter("@Gender", model.Gender),
                    new SqlParameter("@BirthDate", model.BirthDate),
                    new SqlParameter("@Country", model.Country),
                    new SqlParameter("@CulturalBackground", model.CulturalBackground),
                    new SqlParameter("@Email", model.Email)
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC AddLearner @FirstName, @LastName, @Gender, @BirthDate, @Country, @CulturalBackground, @Email", parameters
                );

                // Redirect to a success page or home
                return RedirectToAction("Index", "Home");
            }

            // If the form is not valid, return to the same page with errors
            return View(model);
        }

        // This action handles Instructor Registration submission (POST)
        [HttpPost]
        public async Task<IActionResult> RegisterInstructor(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Execute AddInstructor stored procedure
                var parameters = new[]
                {
                    new SqlParameter("@InstructorName", model.InstructorName),
                    new SqlParameter("@LatestQualification", model.LatestQualification),
                    new SqlParameter("@ExpertiseArea", model.ExpertiseArea),
                    new SqlParameter("@Email", model.Email)
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC AddInstructor @InstructorName, @LatestQualification, @ExpertiseArea, @Email", parameters
                );

                // Redirect to a success page or home
                return RedirectToAction("Index", "Home");
            }

            // If the form is not valid, return to the same page with errors
            return View(model);
        }
    }
}
