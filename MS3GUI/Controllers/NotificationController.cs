using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MS3GUI.Models;
using System.Linq;
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
            var reminders = await _context.GoalReminders
                .FromSqlInterpolated($"EXEC GoalReminder @LearnerID = {learnerId}")
                .ToListAsync();

            return Ok(reminders);
        }

        //public async Task<IActionResult> GetGoalReminders(int learnerId)
        //{
        //    var learnerIdParam = new SqlParameter("@LearnerID", learnerId);

        //    var reminders = await _context.GoalReminders
        //        .FromSqlRaw("EXEC GoalReminder @LearnerID", learnerIdParam)
        //        .ToListAsync();

        //    return Json(reminders);
        //}

        // Existing method for viewing notifications
        public IActionResult Index()
        {
            var userId = User.Identity.Name; // Get the current user ID

            if (User.IsInRole("Admin"))
            {
                // Admin's notifications
                var notifications = _context.Notifications
                    .OrderByDescending(n => n.Ntimestamp)
                    .ToList();

                return View("Admin", notifications); // Return Admin view
            }
            else
            {
                // Learner's notifications filtered by their Email
                var notifications = _context.Notifications
                    .Where(n => n.Learners.Any(l => l.Email == userId))  // Using Any to check the Learners collection
                    .OrderByDescending(n => n.Ntimestamp)
                    .ToList();

                return View("Learner", notifications); // Return Learner view
            }
        }

        // Method to send goal reminder notifications
        public async Task<IActionResult> SendGoalReminderNotifications()
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now); // Convert DateTime to DateOnly
            var currentDateTime = currentDate.ToDateTime(TimeOnly.MinValue);

            // Fetch upcoming goal reminders
            var upcomingGoalReminders = await _context.GoalReminders
                .Where(g => g.Deadline >= currentDateTime && g.Deadline <= currentDateTime.AddDays(7))
                .ToListAsync();

            foreach (var goal in upcomingGoalReminders)
            {
                // Fetch the learner by LearnerId
                var learner = await _context.Learners
                    .FirstOrDefaultAsync(l => l.LearnerId == goal.LearnerId);  // Use LearnerId to find the learner

                if (learner != null)
                {
                    // Create the notification object
                    var notification = new Notification
                    {
                        Ntimestamp = DateOnly.FromDateTime(DateTime.Now),
                        Nmessage = "Your new goal is due soon.",
                        UrgencyLevel = "High",
                        ReadStatus = false, // Initially unread
                        Learners = new List<Learner> { learner } // Associate the learner with the notification
                    };

                    // Add the notification to the context and save changes
                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }
            }

            // Redirect to the Index action after sending notifications
            return RedirectToAction(nameof(Index));
        }

        // Mark a notification as read
        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.NotificationId == id);
            if (notification == null)
            {
                return NotFound();
            }

            notification.ReadStatus = true; // Mark as read
            _context.SaveChanges();

            return RedirectToAction(nameof(Index)); // Redirect back to the notifications list
        }

        // Notification Details view (Admin or Learner based on role)
        public IActionResult Details(int id)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.NotificationId == id);
            if (notification == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Admin"))
            {
                return View("AdminDetails", notification); // Admin's details view
            }
            else
            {
                return View("LearnerDetails", notification); // Learner's details view
            }
        }
    }
}


















//// File: Controllers/NotificationController.cs
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using MS3GUI.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace MS3GUI.Controllers
//{
//    public class NotificationController : Controller
//    {
//        private readonly DatabaseProjectContext _context;

//        public NotificationController(DatabaseProjectContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetGoalReminders(int learnerId)
//        {
//            var learnerIdParam = new SqlParameter("@LearnerID", learnerId);

//            var reminders = await _context.GoalReminders
//                .FromSqlRaw("EXEC GoalReminder @LearnerID", learnerIdParam)
//                .ToListAsync();

//            return Json(reminders);
//        }

//        [HttpGet]
//        public async Task<IActionResult> TestGoalReminders()
//        {
//            int testLearnerId = 5; // Replace with an actual learner ID for testing
//            var learnerIdParam = new SqlParameter("@LearnerID", testLearnerId);

//            var reminders = await _context.GoalReminders
//                .FromSqlRaw("EXEC GoalReminder @LearnerID", learnerIdParam)
//                .ToListAsync();

//            return View("GoalReminder", reminders);
//        }
//    }
//}
