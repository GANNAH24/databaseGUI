using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MS3GUI.Models;

namespace MS3GUI.Controllers
{
    public class CoursesController : Controller
    {
        private readonly DatabaseProjectContext _context;

        public CoursesController(DatabaseProjectContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            return View(await _context.Courses.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,Title,LearningObjective,CreditPoints,DifficultyLevel,Cdescription")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,Title,LearningObjective,CreditPoints,DifficultyLevel,Cdescription")] Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }

        // GET: Courses/Modules/5
        public async Task<IActionResult> Modules(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Modules) // Ensure modules are loaded
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound(); // If the course is not found
            }

            var model = new ModulesViewModel
            {
                CourseTitle = course.Title, // Set course title
                CourseId = id, // Pass the CourseId for redirection purposes
                Modules = course.Modules.ToList() // Load modules for this course
            };

            return View(model); // Return the view with model
        }


        // GET: Courses/AddActivity/1
        [HttpGet]

        public IActionResult AddActivity()
        {
            // Fetch data for dropdowns
            var courses = _context.Courses
                                  .Select(c => new { c.CourseId, c.Title })
                                  .ToList();
            var modules = _context.Modules
                                  .Select(m => new { m.ModuleId, m.Title })
                                  .ToList();

            // Pass data to the ViewModel
            var model = new ActivityViewModel
            {
                CoursesList = new SelectList(courses, "CourseId", "Title"),
                ModulesList = new SelectList(modules, "ModuleId", "Title")
            };

            return View(model);
        }





        // POST: Courses/AddActivity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivity(ActivityViewModel activityViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Log model errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(activityViewModel);
            }

            Console.WriteLine("AddActivity method called!"); // Check if this is logged
            Console.WriteLine($"CourseId: {activityViewModel.CourseId}, ModuleId: {activityViewModel.ModuleId}");

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC NewActivity @CourseID = {0}, @ModuleID = {1}, @activitytype = {2}, @instructiondetails = {3}, @maxpoints = {4}",
                activityViewModel.CourseId,
                activityViewModel.ModuleId,
                activityViewModel.ActivityType,
                activityViewModel.InstructionDetails,
                activityViewModel.MaxPoints
            );

            // Redirect back to Modules page
            return RedirectToAction("Modules", new { id = activityViewModel.CourseId });
        }





        public async Task<IActionResult> EnrolledCourses(int learnerId)
        {
            var learner = await _context.Learners
       .FirstOrDefaultAsync(l => l.LearnerId == learnerId);

            var enrolledCourses = await _context.Course_enrollment
                .Where(ce => ce.LearnerId == learnerId && ce.Status == "Completed")
                .Join(_context.Courses,
                      ce => ce.CourseId,
                      c => c.CourseId,
                      (ce, c) => new EnrolledCoursesViewModel
                      {
                          LearnerId = learner.LearnerId,
                          EnrollmentId = ce.EnrollmentId,
                          // LearnerId = ce.LearnerId ?? 0,
                          CourseId = c.CourseId,
                          CourseName = c.Title,
                          EnrollmentDate = ce.EnrollmentDate.HasValue
                                            ? (DateTime?)ce.EnrollmentDate.Value.ToDateTime(new TimeOnly(0, 0))
                                            : null,
                          CompletionDate = ce.CompletionDate.HasValue
                                            ? (DateTime?)ce.CompletionDate.Value.ToDateTime(new TimeOnly(0, 0))
                                            : null,
                          Status = ce.Status
                      })
                .ToListAsync();

            // Debug to check if the data is being populated
            if (enrolledCourses.Any())
            {
                foreach (var course in enrolledCourses)
                {
                    Debug.WriteLine($"Course: {course.CourseName}, EnrollmentDate: {course.EnrollmentDate}, CompletionDate: {course.CompletionDate}");
                }
            }
            else
            {
                Debug.WriteLine("No enrolled courses found.");
            }

            return View(enrolledCourses);
        }





        public async Task<IActionResult> ViewPreviousCourses(int courseId)
        {
            // Manually set the learnerId (you can change this to dynamic based on current user)
            int learnerId = 1;  // Set this to the actual learner ID, for testing use 1 or a valid ID

            // Get the selected course
            var selectedCourse = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (selectedCourse == null)
            {
                return Content("Course not found.");
            }

            // Query prerequisites for the course
            var previousCourses = await _context.CoursePrerequisites
                .Where(cp => cp.CourseId == courseId)
                .Join(_context.Courses,
                      cp => cp.Prereq,
                      c => c.Title,
                      (cp, c) => new
                      {
                          c.CourseId,
                          c.Title // Only selecting the course title
                      })
                .ToListAsync();

            if (previousCourses == null || !previousCourses.Any())
            {
                return Content("No prerequisites found for this course.");
            }

            // Call the Prerequisites stored procedure
            var result = await _context.Database
                .ExecuteSqlRawAsync("EXEC Prerequisites @LearnerID = {0}, @CourseID = {1}", learnerId, courseId);

            // Determine the result of the stored procedure (based on the print output, you can capture this in the view)
            string prerequisitesStatus = result > 0 ? "All prerequisites are completed." : "Not all prerequisites are completed.";

            // Pass the prerequisites and status to the view
            ViewData["PrerequisitesStatus"] = prerequisitesStatus;

            // Return the view displaying previous courses and prerequisites status
            return View(previousCourses);
        }



        /*        public async Task<IActionResult> EnrollInCourse()
                {
                    var model = new EnrollInCourseViewModel
                    {
                        Courses = await _context.Courses.ToListAsync(), // Get all courses
                        Learners = await _context.Learners.ToListAsync() // Get all learners
                    };

                    return View(model);
                }*/





        /*     var learner = await _context.Learners
        .FirstOrDefaultAsync(l => l.LearnerId == learnerId);

             var model = await _context.Course_enrollment
                 .Where(ce => ce.LearnerId == learnerId && ce.Status == "Completed")
                 .Join(_context.Courses,
                       ce => ce.CourseId,
                       c => c.CourseId,
                       (ce, c) => new EnrollInCourseViewModel
                       {
                           LearnerId = learner.LearnerId,
                           EnrollmentId = ce.EnrollmentId,
                           // LearnerId = ce.LearnerId ?? 0,
                           CourseId = c.CourseId,
                           CourseName = c.Title,
                           EnrollmentDate = ce.EnrollmentDate.HasValue
                                             ? (DateTime?)ce.EnrollmentDate.Value.ToDateTime(new TimeOnly(0, 0))
                                             : null,
                           CompletionDate = ce.CompletionDate.HasValue
                                             ? (DateTime?)ce.CompletionDate.Value.ToDateTime(new TimeOnly(0, 0))
                                             : null,
                           Status = ce.Status
                       }*/

        [HttpPost]
        public async Task<IActionResult> EnrollInCourse(int courseId)
        {
            // Assuming LearnerId is retrieved from session or authentication
            var learnerId = 18;

            var learner = await _context.Learners
                .FirstOrDefaultAsync(l => l.LearnerId == learnerId);

            if (learner == null)
            {
                return NotFound();
            }

            var courseEnrollment = new CourseEnrollment
            {
                CourseId = courseId,
                LearnerId = learnerId,
                EnrollmentDate = DateOnly.FromDateTime(DateTime.Now),
                Status = "Enrolled"
            };

            _context.Course_enrollment.Add(courseEnrollment);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }






    }






}









