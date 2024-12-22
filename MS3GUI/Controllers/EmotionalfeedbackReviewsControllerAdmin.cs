using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MS3GUI.Models;

namespace MS3GUI.Controllers
{
    public class EmotionalfeedbackReviewsControllerAdmin : Controller
    {
        private readonly DatabaseProjectContext _context;

        public EmotionalfeedbackReviewsControllerAdmin(DatabaseProjectContext context)
        {
            _context = context;
        }

        // GET: EmotionalfeedbackReviewsControllerAdmin
        public async Task<IActionResult> Index()
        {
            var databaseProjectContext = _context.EmotionalfeedbackReviews.Include(e => e.FeedbackNavigation).Include(e => e.Instructor);
            return View(await databaseProjectContext.ToListAsync());
        }

        // GET: EmotionalfeedbackReviewsControllerAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emotionalfeedbackReview = await _context.EmotionalfeedbackReviews
                .Include(e => e.FeedbackNavigation)
                .Include(e => e.Instructor)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (emotionalfeedbackReview == null)
            {
                return NotFound();
            }

            return View(emotionalfeedbackReview);
        }

        // GET: EmotionalfeedbackReviewsControllerAdmin/Create
        public IActionResult Create()
        {
            ViewData["FeedbackId"] = new SelectList(_context.EmotionalFeedbacks, "FeedbackId", "FeedbackId");
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "InstructorId");
            return View();
        }

        // POST: EmotionalfeedbackReviewsControllerAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FeedbackId,InstructorId,Feedback")] EmotionalfeedbackReview emotionalfeedbackReview)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emotionalfeedbackReview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FeedbackId"] = new SelectList(_context.EmotionalFeedbacks, "FeedbackId", "FeedbackId", emotionalfeedbackReview.FeedbackId);
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "InstructorId", emotionalfeedbackReview.InstructorId);
            return View(emotionalfeedbackReview);
        }

        // GET: EmotionalfeedbackReviewsControllerAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emotionalfeedbackReview = await _context.EmotionalfeedbackReviews.FindAsync(id);
            if (emotionalfeedbackReview == null)
            {
                return NotFound();
            }
            ViewData["FeedbackId"] = new SelectList(_context.EmotionalFeedbacks, "FeedbackId", "FeedbackId", emotionalfeedbackReview.FeedbackId);
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "InstructorId", emotionalfeedbackReview.InstructorId);
            return View(emotionalfeedbackReview);
        }

        // POST: EmotionalfeedbackReviewsControllerAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FeedbackId,InstructorId,Feedback")] EmotionalfeedbackReview emotionalfeedbackReview)
        {
            if (id != emotionalfeedbackReview.FeedbackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emotionalfeedbackReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmotionalfeedbackReviewExists(emotionalfeedbackReview.FeedbackId))
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
            ViewData["FeedbackId"] = new SelectList(_context.EmotionalFeedbacks, "FeedbackId", "FeedbackId", emotionalfeedbackReview.FeedbackId);
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "InstructorId", emotionalfeedbackReview.InstructorId);
            return View(emotionalfeedbackReview);
        }



        public async Task<IActionResult> ViewEmotionalReview(int instructorId)
        {
            var emotionalReviews = new List<EmotionalfeedbackReview>();

            try
            {
                // Execute stored procedure to get emotional feedback analysis for a specific instructor
                var instructorIdParam = new SqlParameter("@InstructorID", instructorId);

                emotionalReviews = await _context.EmotionalfeedbackReviews.FromSqlRaw(
                    "EXEC InstructorReview @InstructorID", instructorIdParam).ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle error, possibly log it
                Console.WriteLine(ex.Message);
            }

            // Return the view with the emotional feedback data
            return View(emotionalReviews);
        }

        // GET: EmotionalfeedbackReviewsControllerAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emotionalfeedbackReview = await _context.EmotionalfeedbackReviews
                .Include(e => e.FeedbackNavigation)
                .Include(e => e.Instructor)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (emotionalfeedbackReview == null)
            {
                return NotFound();
            }

            return View(emotionalfeedbackReview);
        }

        // POST: EmotionalfeedbackReviewsControllerAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emotionalfeedbackReview = await _context.EmotionalfeedbackReviews.FindAsync(id);
            if (emotionalfeedbackReview != null)
            {
                _context.EmotionalfeedbackReviews.Remove(emotionalfeedbackReview);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmotionalfeedbackReviewExists(int id)
        {
            return _context.EmotionalfeedbackReviews.Any(e => e.FeedbackId == id);
        }
        //    public async Task<IActionResult> ViewEmotionalReview(int instructorId)
        //    {
        //        var emotionalReviews = new List<EmotionalfeedbackReview>();

        //        try
        //        {
        //            // Execute stored procedure to get emotional feedback analysis for a specific instructor
        //            var instructorIdParam = new SqlParameter("@InstructorID", instructorId);

        //            emotionalReviews = await _context.EmotionalfeedbackReviews.FromSqlRaw(
        //                "EXEC InstructorReview @InstructorID", instructorIdParam).ToListAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            // Handle error, possibly log it
        //            Console.WriteLine(ex.Message);
        //        }

        //        // Return the view with the emotional feedback data
        //        return View(emotionalReviews);
        //    }
        //}
    }
}
