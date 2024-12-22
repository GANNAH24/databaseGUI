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
    public class EmotionalFeedbacksControllerLearner : Controller
    {
        private readonly DatabaseProjectContext _context;

        public EmotionalFeedbacksControllerLearner(DatabaseProjectContext context)
        {
            _context = context;
        }

        // GET: EmotionalFeedbacksControllerLearner
        public async Task<IActionResult> Index()
        {
            var databaseProjectContext = _context.EmotionalFeedbacks.Include(e => e.Learner);
            return View(await databaseProjectContext.ToListAsync());
        }

        // GET: EmotionalFeedbacksControllerLearner/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emotionalFeedback = await _context.EmotionalFeedbacks
                .Include(e => e.Learner)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (emotionalFeedback == null)
            {
                return NotFound();
            }

            return View(emotionalFeedback);
        }

        // GET: EmotionalFeedbacksControllerLearner/Create
        public IActionResult Create()
        {
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "LearnerId");
            return View();
        }

        // POST: EmotionalFeedbacksControllerLearner/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FeedbackId,Timestamp,EmotionalState,LearnerId")] EmotionalFeedback emotionalFeedback)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emotionalFeedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "LearnerId", emotionalFeedback.LearnerId);
            return View(emotionalFeedback);
        }

        // GET: EmotionalFeedbacksControllerLearner/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emotionalFeedback = await _context.EmotionalFeedbacks.FindAsync(id);
            if (emotionalFeedback == null)
            {
                return NotFound();
            }
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "LearnerId", emotionalFeedback.LearnerId);
            return View(emotionalFeedback);
        }

        // POST: EmotionalFeedbacksControllerLearner/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FeedbackId,Timestamp,EmotionalState,LearnerId")] EmotionalFeedback emotionalFeedback)
        {
            if (id != emotionalFeedback.FeedbackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emotionalFeedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmotionalFeedbackExists(emotionalFeedback.FeedbackId))
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
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "LearnerId", emotionalFeedback.LearnerId);
            return View(emotionalFeedback);
        }

        // GET: EmotionalFeedbacksControllerLearner/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emotionalFeedback = await _context.EmotionalFeedbacks
                .Include(e => e.Learner)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (emotionalFeedback == null)
            {
                return NotFound();
            }

            return View(emotionalFeedback);
        }

        // POST: EmotionalFeedbacksControllerLearner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emotionalFeedback = await _context.EmotionalFeedbacks.FindAsync(id);
            if (emotionalFeedback != null)
            {
                _context.EmotionalFeedbacks.Remove(emotionalFeedback);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmotionalFeedbackExists(int id)
        {
            return _context.EmotionalFeedbacks.Any(e => e.FeedbackId == id);
        }




        // Other action methods...

        // GET: EmotionalFeedbacksControllerLearner/SubmitFeedback
        public IActionResult SubmitFeedback()
        {
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "LearnerId");
            ViewData["ActivityId"] = new SelectList(_context.LearningActivities, "ActivityId", "ActivityId");
            return View();
        }

        // POST: EmotionalFeedbacksControllerLearner/SubmitFeedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback([Bind("ActivityId,LearnerId,Timestamp,EmotionalState")] EmotionalFeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine($"Received - ActivityId: {model.ActivityId}, LearnerId: {model.LearnerId}, Timestamp: {model.Timestamp}, EmotionalState: {model.EmotionalState}");
                var parameters = new[]
        {
        new SqlParameter("@ActivityID", model.ActivityId),
        new SqlParameter("@LearnerID", model.LearnerId),
        new SqlParameter("@Timestamp", model.Timestamp),
        new SqlParameter("@EmotionalState", model.EmotionalState)
    };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC ActivityEmotionalFeedback @ActivityID, @LearnerID, @Timestamp, @EmotionalState",
                    parameters
                );

                return RedirectToAction(nameof(Index));
            }
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "LearnerId", model.LearnerId);
            ViewData["ActivityId"] = new SelectList(_context.LearningActivities, "ActivityId", "ActivityId", model.ActivityId);
            return View(model);
        }

        // Other action methods...
    }
}

