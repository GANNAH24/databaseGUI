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
                var parameters = new[]
                {
            new SqlParameter("@FirstName", model.FirstName),
            new SqlParameter("@LastName", model.LastName),
            new SqlParameter("@Gender", model.Gender),
            new SqlParameter("@BirthDate", model.BirthDate),
            new SqlParameter("@Country", model.Country),
            new SqlParameter("@CulturalBackground", model.CulturalBackground),
            new SqlParameter("@Email", model.Email),
            new SqlParameter("@Password", model.Password)
        };

                // Call stored procedure and get the result
                var result = await _context.Set<StatusMessage>().FromSqlRaw(
                    "EXEC AddLearner @FirstName, @LastName, @Gender, @BirthDate, @Country, @CulturalBackground, @Email, @Password",
                    parameters
                ).ToListAsync();

                string message = result.FirstOrDefault()?.Message ?? "An unexpected error occurred.";
                TempData["StatusMessage"] = message;

                return RedirectToAction("Login");
            }

            // If form is invalid, return to view with model errors
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

        // This action displays the user's profile
        public IActionResult Profile(string email)
        {
            // Retrieve the user profile based on the email
            // For demonstration, we will just pass the email to the view
            ViewData["Email"] = email;
            return View();
        }



        public IActionResult Login(string email, string password)
        {
            // Query the Learner table to find a matching email and password
            var learner = _context.Learners
                .FirstOrDefault(l => l.Email == email && l.Password == password);

            if (learner != null)
            {
                // Successful login, redirect to profile or dashboard
                return RedirectToAction("Profile", new { email = learner.Email });
            }

            // If no match is found, show an error message
            ViewData["ErrorMessage"] = "Invalid email or password.";
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var learner = _context.Learners
                    .FirstOrDefault(l => l.Email == model.Email && l.Password == model.Password);

                if (learner != null)
                {
                    return RedirectToAction("Profile", new { email = learner.Email });
                }

                ViewData["ErrorMessage"] = "Invalid email or password.";
            }

            return View(model);
        }




    }
}
