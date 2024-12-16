using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
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
            else if (role == "Admin")
            {
                return RedirectToAction("RegisterAdmin"); // Redirect to Instructor registration form
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
                    new SqlParameter("@Email", model.Email),
                    new SqlParameter("@Password", model.Password)

                };

                var result = await _context.Set<StatusMessage>().FromSqlRaw(
                    "EXEC AddInstructor @InstructorName, @LatestQualification, @ExpertiseArea, @Email, @Password",
                    parameters
                ).ToListAsync();

                string message = result.FirstOrDefault()?.Message ?? "An unexpected error occurred.";
                TempData["StatusMessage"] = message;

                return RedirectToAction("Login");
            }

            // If the form is not valid, return to the same page with errors
            return View(model);
        }

        // This action displays the user's profile
        // Profile Page (GET)
        public IActionResult Profile(string email)
        {
            // Pass the email to the ViewData to display user-specific information on the profile page
            ViewData["Email"] = email;
            return View();
        }

        // Login Page (GET)
        [HttpGet]
        public IActionResult Login()
        {
            // Load an empty login form
            return View(new LoginViewModel());
        }

        // Login Submission (POST)




        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if the learner exists and the password matches
                    var learner = await _context.Learners
                        .FirstOrDefaultAsync(l => l.Email.ToLower() == model.Email.ToLower());

                    if (learner != null && learner.Password == model.Password)
                    {
                        // Successful login, redirect to the learner's profile
                        return RedirectToAction("Profile", new { email = learner.Email });
                    }

                    // Check if the instructor exists and the password matches
                    var instructor = await _context.Instructors
                        .FirstOrDefaultAsync(i => i.Email.ToLower() == model.Email.ToLower());

                    if (instructor != null && instructor.Password == model.Password)
                    {
                        // Successful login for instructor, redirect to the instructor's profile
                        return RedirectToAction("Profile", new { email = instructor.Email });
                    }

                    // Check if the admin exists and the password matches
                    var admin = await _context.Admin
                        .FirstOrDefaultAsync(a => a.Email.ToLower() == model.Email.ToLower());

                    if (admin != null && admin.Password == model.Password)
                    {
                        // Successful login for admin, redirect to admin's profile
                        return RedirectToAction("Profile", new { email = admin.Email });
                    }

                    // If no match, show an error message
                    ViewData["ErrorMessage"] = "Invalid email or password.";
                }
                catch (Exception ex)
                {
                    // Handle unexpected errors
                    ViewData["ErrorMessage"] = "An error occurred: " + ex.Message;
                }
            }

            // Return the login view with error message if validation fails
            return View(model);
        }







        /*   
           public async Task<IActionResult> Login(LoginViewModel model)
           {
               if (ModelState.IsValid)
               {
                   try
                   {
                       // Check if the learner exists and the password matches
                       var learner = await _context.Learners
                           .FirstOrDefaultAsync(l => l.Email.ToLower() == model.Email.ToLower());

                       if (learner != null && learner.Password == model.Password)
                       {
                           // Successful login, redirect to the learner's profile
                           return RedirectToAction("Profile", new { email = learner.Email });
                       }

                       // Check if the instructor exists and the password matches
                       var instructor = await _context.Instructors
                           .FirstOrDefaultAsync(i => i.Email.ToLower() == model.Email.ToLower());

                       if (instructor != null && instructor.Password == model.Password)
                       {
                           // Successful login for instructor, redirect to the instructor's dashboard
                           return RedirectToAction("Profile", new { email = instructor.Email });
                       }

                       var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email.ToLower() == model.Email.ToLower());

                       if (admin != null && admin.Password == model.Password)
                       {
                           // Successful login for admin, redirect to admin's dashboard
                           return RedirectToAction("Profile", new { email = admin.Email });
                       }


                       // If no match, show an error message
                       ViewData["ErrorMessage"] = "Invalid email or password.";
                   }
                   catch (Exception ex)
                   {
                       // Handle unexpected errors
                       ViewData["ErrorMessage"] = "An error occurred: " + ex.Message;
                   }
               }

               // Return the login view with error message if validation fails
               return View(model);
           }*/


        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Define parameters to pass to the stored procedure
                var parameters = new[] {
            new SqlParameter("@AdminName", model.AdminName),
            new SqlParameter("@Email", model.Email),
            new SqlParameter("@Password", model.Password)
        };

                // Call the stored procedure and capture the status message
                var result = await _context.Set<StatusMessage>().FromSqlRaw(
                    "EXEC AddAdmin @AdminName, @Email, @Password",
                    parameters
                ).ToListAsync();

                // Get the status message from the result
                string message = result.FirstOrDefault()?.Message ?? "An unexpected error occurred.";

                // Store the message in TempData
                TempData["StatusMessage"] = message;

                // Redirect based on the message
                if (message.Contains("Email already exists"))
                {
                    return RedirectToAction("Login"); // Redirect to the login page if email exists
                }

                return RedirectToAction("Login"); // Redirect to the login page after successful registration
            }

            return View(model); // Return to the registration view if the model is invalid
        }






        /*  public IActionResult Login(string email, string password)
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
          }*/




    }
}
