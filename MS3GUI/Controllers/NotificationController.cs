// File: Controllers/NotificationController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MS3GUI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MS3GUI.Controllers
{
    public class NotificationController : Controller
    {
        private readonly DatabaseProjectContext _context;

        public NotificationController(DatabaseProjectContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetGoalReminders(int learnerId)
        {
            var learnerIdParam = new SqlParameter("@LearnerID", learnerId);

            var reminders = await _context.GoalReminders
                .FromSqlRaw("EXEC GoalReminder @LearnerID", learnerIdParam)
                .ToListAsync();

            return Json(reminders);
        }

        [HttpGet]
        public async Task<IActionResult> TestGoalReminders()
        {
            int testLearnerId = 5; // Replace with an actual learner ID for testing
            var learnerIdParam = new SqlParameter("@LearnerID", testLearnerId);

            var reminders = await _context.GoalReminders
                .FromSqlRaw("EXEC GoalReminder @LearnerID", learnerIdParam)
                .ToListAsync();

            return View("GoalReminder", reminders);
        }
    }
}
