//ASSESSMENT CONTROLLOR

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MS3GUI.Models;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MS3GUI.Controllers
{
    public class AssessmentsController : Controller
    {
        private readonly DatabaseProjectContext _context;

        public AssessmentsController(DatabaseProjectContext context)
        {
            _context = context;
        }

        // GET: Assessments
        public async Task<IActionResult> Index()
        {
            try
            {
                var assessments = _context.Assessments.Include(a => a.Module);
                return View(await assessments.ToListAsync());
            }
            catch (Exception ex)
            {
                // Log error (implement logging as needed)
                Console.WriteLine(ex); // Added logging
                ViewBag.ErrorMessage = "An error occurred while retrieving assessments.";
                return View("Error");
            }
        }
        //// GET: Assessments/Create
        //public IActionResult Create()
        //{
        //    try
        //    {
        //        // Create a SelectList from the modules
        //        ViewData["Modules"] = new SelectList(_context.Modules, "ModuleId", "Title");
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log error
        //        Console.WriteLine(ex); // Added logging
        //        ViewBag.ErrorMessage = "An error occurred while preparing the create form.";
        //        return View("Error");
        //    }
        //}
        public IActionResult Create()
        {
            try
            {
                // Create a SelectList from the modules and courses with IDs as text
                ViewData["Modules"] = new SelectList(_context.Modules, "ModuleId", "ModuleId");
                ViewData["Courses"] = new SelectList(_context.Courses, "CourseId", "CourseId");
                return View();
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine(ex); // Added logging
                ViewBag.ErrorMessage = "An error occurred while preparing the create form.";
                return View("Error");
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Atype,TotalMarks,PassingMarks,Criteria,Weightage,Adescription,Title,ModuleId,CourseId")] Assessment assessment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(assessment);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewBag.Modules = new SelectList(_context.Modules, "ModuleId", "Title", assessment.ModuleId);
        //    return View(assessment);
        //}



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Atype,TotalMarks,PassingMarks,Criteria,Weightage,Adescription,Title,ModuleId,CourseId")] Assessment assessment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Log parameters
                    Console.WriteLine($"Creating assessment: {assessment.Atype}, {assessment.TotalMarks}, {assessment.PassingMarks}, {assessment.Criteria}, {assessment.Weightage}, {assessment.Adescription}, {assessment.Title}, {assessment.ModuleId}, {assessment.CourseId}");

                    var parameters = new[]
                    {
                new SqlParameter("@Atype", assessment.Atype ?? (object)DBNull.Value),
                new SqlParameter("@TotalMarks", assessment.TotalMarks ?? (object)DBNull.Value),
                new SqlParameter("@PassingMarks", assessment.PassingMarks ?? (object)DBNull.Value),
                new SqlParameter("@Criteria", assessment.Criteria ?? (object)DBNull.Value),
                new SqlParameter("@Weightage", assessment.Weightage ?? (object)DBNull.Value),
                new SqlParameter("@Adescription", assessment.Adescription ?? (object)DBNull.Value),
                new SqlParameter("@Title", assessment.Title ?? (object)DBNull.Value),
                new SqlParameter("@ModuleId", assessment.ModuleId ?? (object)DBNull.Value),
                new SqlParameter("@CourseId", assessment.CourseId ?? (object)DBNull.Value)
            };

                    var sql = "EXEC AddAssessment @Atype, @TotalMarks, @PassingMarks, @Criteria, @Weightage, @Adescription, @Title, @ModuleId, @CourseId";
                    Console.WriteLine($"Executing SQL: {sql}");

                    await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                    TempData["SuccessMessage"] = "Assessment created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"SQL Error creating assessment: {ex.Message}");
                    ViewBag.ErrorMessage = "An error occurred while creating the assessment. Please check the input data.";
                    return View("Error");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating assessment: {ex.Message}");
                    ViewBag.ErrorMessage = "An unexpected error occurred while creating the assessment.";
                    return View("Error");
                }
            }

            // Log ModelState errors
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"ModelState error: {error.ErrorMessage}");
            }

            ViewData["Modules"] = new SelectList(_context.Modules, "ModuleId", "ModuleId", assessment.ModuleId);
            ViewData["Courses"] = new SelectList(_context.Courses, "CourseId", "CourseId", assessment.CourseId);
            return View(assessment);
        }






        // GET: Assessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include the related Module and Course data using Include
            var assessment = await _context.Assessments
                .Include(a => a.Module)
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assessment == null)
            {
                return NotFound();
            }

            return View(assessment);
        }




        public IActionResult NotifyStudents(int id)
        {
            var viewModel = new NotifyStudentsViewModel
            {
                AssessmentId = id
            };
            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotifyStudents(NotifyStudentsViewModel model)
        {
            try
            {
                var assessment = await _context.Assessments.FindAsync(model.AssessmentId);
                if (assessment == null)
                {
                    return NotFound();
                }

                var learners = _context.CourseEnrollments
                    .Where(e => e.CourseId == assessment.CourseId)
                    .Select(e => e.LearnerId)
                    .ToList();

                foreach (var learnerId in learners)
                {
                    var notification = new Notification
                    {
                        Ntimestamp = DateOnly.FromDateTime(DateTime.Now),
                        Nmessage = model.Message,
                        UrgencyLevel = model.UrgencyLevel,
                        ReadStatus = false
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();

                    var receivedNotification = new ReceivedNotification
                    {
                        NotificationId = notification.NotificationId,
                        LearnerId = learnerId ?? 0
                    };

                    _context.ReceivedNotifications.Add(receivedNotification);
                    await _context.SaveChangesAsync();

                    var parameters = new[]
                    {
                new SqlParameter("@NotificationID", SqlDbType.Int) { Value = notification.NotificationId },
                new SqlParameter("@timestamp", SqlDbType.DateTime) { Value = notification.Ntimestamp },
                new SqlParameter("@message", SqlDbType.VarChar) { Value = notification.Nmessage },
                new SqlParameter("@Urgency_level", SqlDbType.VarChar) { Value = notification.UrgencyLevel },
                new SqlParameter("@LearnerID", SqlDbType.Int) { Value = learnerId }
            };

                    await _context.Database.ExecuteSqlRawAsync("EXEC AssessmentNot @NotificationID, @timestamp, @message, @Urgency_level, @LearnerID", parameters);
                }

                TempData["SuccessMessage"] = "Notifications sent successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notifications: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while sending notifications.";
                return View("Error");
            }
        }
        //public IActionResult Error()
        //{
        //    var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        //    var errorViewModel = new ErrorViewModel
        //    {
        //        RequestId = requestId,
        //        ShowRequestId = !string.IsNullOrEmpty(requestId)
        //    };
        //    return View(errorViewModel);
        //}


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var assessment = await _context.Assessments.FindAsync(id);
                if (assessment == null) return NotFound();

                // Populate dropdown lists with IDs as text
                ViewData["Modules"] = new SelectList(_context.Modules, "ModuleId", "ModuleId", assessment.ModuleId);
                ViewData["Courses"] = new SelectList(_context.Courses, "CourseId", "CourseId", assessment.CourseId);

                return View(assessment);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine(ex); // Added logging
                ViewBag.ErrorMessage = "An error occurred while retrieving the assessment for editing.";
                return View("Error");
            }
        }

        //// GET: Assessments/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null) return NotFound();

        //    try
        //    {
        //        var assessment = await _context.Assessments.FindAsync(id);
        //        if (assessment == null) return NotFound();

        //        // Populate dropdown lists correctly
        //        ViewData["Modules"] = new SelectList(_context.Modules, "ModuleId", "Title", assessment.ModuleId);
        //        ViewData["Courses"] = new SelectList(_context.Courses, "CourseId", "CourseName", assessment.CourseId);

        //        return View(assessment);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log error
        //        Console.WriteLine(ex); // Added logging
        //        ViewBag.ErrorMessage = "An error occurred while retrieving the assessment for editing.";
        //        return View("Error");
        //    }
        //}

        // POST: Assessments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Atype,TotalMarks,PassingMarks,Criteria,Weightage,Adescription,Title,ModuleId,CourseId")] Assessment assessment)
        {
            if (id != assessment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assessment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssessmentExists(assessment.Id)) return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    // Log error
                    Console.WriteLine(ex); // Added logging
                    ViewBag.ErrorMessage = "An error occurred while editing the assessment.";
                    return View("Error");
                }
            }
        ViewData["Modules"] = new SelectList(_context.Modules, "ModuleId", "ModuleId", assessment.ModuleId);
        ViewData["Courses"] = new SelectList(_context.Courses, "CourseId", "CourseId", assessment.CourseId);
         return View(assessment);
    }

        private bool AssessmentExists(int id)
        {
            return _context.Assessments.Any(e => e.Id == id);
        }

        // POST: TakenAssessments/UpdateScore
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateScore(int assessmentId, int learnerId, int newScore)
        {
            var takenAssessment = await _context.TakenAssessments
                .FirstOrDefaultAsync(t => t.AssessmentId == assessmentId && t.LearnerId == learnerId);

            if (takenAssessment == null)
            {
                return NotFound();
            }

            takenAssessment.Score = newScore;
            _context.Update(takenAssessment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> HighestScoreAssessment(int courseId)
        {
            var highestScoreAssessment = await _context.Assessments
                .Where(a => a.CourseId == courseId)
                .OrderByDescending(a => a.TakenAssessments.Max(t => t.Score))
                .FirstOrDefaultAsync();

            return View(highestScoreAssessment);
        }

        // New action method to display all available assessments
        public async Task<IActionResult> AvailableAssessments()
        {
            var assessments = await _context.Assessments.ToListAsync();
            return View(assessments);
        }

        public async Task<IActionResult> HighestGrades()
        {
            var highestGrades = await _context.HighestGrades.ToListAsync();
            return View(highestGrades);
        }

        // GET: Assessments/ReviewTakenAssessments/5
        public async Task<IActionResult> ReviewTakenAssessments()
        {
            var takenAssessments = await _context.TakenAssessments
                .Include(t => t.Assessment)
                .Include(t => t.Learner)
                .ToListAsync();

            if (takenAssessments == null || !takenAssessments.Any())
            {
                return NotFound();
            }

            return View(takenAssessments);
        }



        // POST: Assessments/UpdateTakenAssessmentScore
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTakenAssessmentScore(int takenAssessmentId, int learnerId, int newScore)
        {
            var takenAssessment = await _context.TakenAssessments
                .FirstOrDefaultAsync(t => t.AssessmentId == takenAssessmentId && t.LearnerId == learnerId);

            if (takenAssessment == null)
            {
                return NotFound();
            }

            takenAssessment.Score = newScore;
            _context.Update(takenAssessment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ReviewTakenAssessments));
        }

        public async Task<IActionResult> AssessmentAnalytics()
        {
            var analytics = await _context.AssessmentAnalytics
                .FromSqlRaw("EXEC AssessmentAnalytics")
                .ToListAsync();

            return View(analytics);
        }



    }


}










