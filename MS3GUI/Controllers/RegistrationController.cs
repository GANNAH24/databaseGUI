using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MS3GUI.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly string _connectionString = "YourConnectionStringHere";
        public IActionResult Index()
        {
            return View();
        }

        // Submit the user form data
        [HttpPost]
        public IActionResult Submit(string role, string firstName, string lastName, string gender, string birthDate, string country, string culturalBackground, string qualification, string expertise, string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();

                if (role == "learner")
                {
                    // Call the stored procedure to add a learner
                    cmd = new SqlCommand("AddLearner", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@first_name", firstName);
                    cmd.Parameters.AddWithValue("@last_name", lastName);
                    cmd.Parameters.AddWithValue("@gender", gender);
                    cmd.Parameters.AddWithValue("@birth_date", birthDate);
                    cmd.Parameters.AddWithValue("@country", country);
                    cmd.Parameters.AddWithValue("@cultural_background", culturalBackground);
                }
                else if (role == "instructor")
                {
                    // Call the stored procedure to add an instructor
                    cmd = new SqlCommand("AddInstructor", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InstructorName", firstName);
                    cmd.Parameters.AddWithValue("@latest_qualification", qualification);
                    cmd.Parameters.AddWithValue("@expertise_area", expertise);
                    cmd.Parameters.AddWithValue("@Email", email);
                }

                try
                {
                    cmd.ExecuteNonQuery();
                    ViewBag.Message = "Registration successful!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error: " + ex.Message;
                }
            }

            return RedirectToAction("Index");
        }


    }
}
