using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MS3GUI.Models;

namespace MS3GUI.Controllers
{
    public class TakenAssessmentsController : Controller
    {
        private readonly DatabaseProjectContext _context;

        public TakenAssessmentsController(DatabaseProjectContext context)
        {
            _context = context;
        }

        // GET: TakenAssessments
        public async Task<IActionResult> Index()
        {
            // Fetch all TakenAssessments, include associated Assessment and Learner
            var takenAssessments = await _context.TakenAssessments
                .Include(t => t.Assessment)
                .Include(t => t.Learner)
                .ToListAsync();

            return View(takenAssessments);  // Passing a collection of TakenAssessment objects to the view
        }

        // GET: TakenAssessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the TakenAssessment by its primary key (AssessmentId)
            var takenAssessment = await _context.TakenAssessments
                .Include(t => t.Assessment)
                .Include(t => t.Learner)
                .FirstOrDefaultAsync(m => m.AssessmentId == id);

            if (takenAssessment == null)
            {
                return NotFound();
            }

            return View(takenAssessment);
        }

        // GET: TakenAssessments/Create
        public IActionResult Create()
        {
            // Preparing the drop-down lists for AssessmentId and LearnerId
            ViewData["AssessmentId"] = new SelectList(_context.Assessments, "Id", "Title");
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "Name");
            return View();
        }

        // POST: TakenAssessments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Score,LearnerId,AssessmentId")] TakenAssessment takenAssessment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(takenAssessment);  // Add new TakenAssessment to the context
                await _context.SaveChangesAsync();  // Save changes to database
                return RedirectToAction(nameof(Index));  // Redirect to Index (list of TakenAssessments)
            }

            // If there is a problem with the model, reload the drop-down lists
            ViewData["AssessmentId"] = new SelectList(_context.Assessments, "Id", "Title", takenAssessment.AssessmentId);
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "Name", takenAssessment.LearnerId);
            return View(takenAssessment);  // Return to Create view with validation error
        }

        // GET: TakenAssessments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var takenAssessment = await _context.TakenAssessments.FindAsync(id);
            if (takenAssessment == null)
            {
                return NotFound();
            }

            // Prepare drop-down lists for editing
            ViewData["AssessmentId"] = new SelectList(_context.Assessments, "Id", "Title", takenAssessment.AssessmentId);
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "Name", takenAssessment.LearnerId);
            return View(takenAssessment);
        }

        // POST: TakenAssessments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Score,LearnerId,AssessmentId")] TakenAssessment takenAssessment)
        {
            if (id != takenAssessment.AssessmentId)  // Validate if the IDs match
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(takenAssessment);  // Update the TakenAssessment in the context
                    await _context.SaveChangesAsync();  // Save changes to database
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TakenAssessmentExists(takenAssessment.AssessmentId))  // Check if the assessment exists
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;  // Re-throw the exception if there's a concurrency issue
                    }
                }
                return RedirectToAction(nameof(Index));  // Redirect to Index if update was successful
            }

            // Reload the drop-down lists if validation fails
            ViewData["AssessmentId"] = new SelectList(_context.Assessments, "Id", "Title", takenAssessment.AssessmentId);
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "Name", takenAssessment.LearnerId);
            return View(takenAssessment);  // Return to the Edit view with validation errors
        }

        // GET: TakenAssessments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch TakenAssessment for deletion by primary key (AssessmentId)
            var takenAssessment = await _context.TakenAssessments
                .Include(t => t.Assessment)
                .Include(t => t.Learner)
                .FirstOrDefaultAsync(m => m.AssessmentId == id);

            if (takenAssessment == null)
            {
                return NotFound();
            }

            return View(takenAssessment);  // Return the Delete view
        }

        // POST: TakenAssessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find and remove the TakenAssessment from the database
            var takenAssessment = await _context.TakenAssessments.FindAsync(id);
            if (takenAssessment != null)
            {
                _context.TakenAssessments.Remove(takenAssessment);
            }

            await _context.SaveChangesAsync();  // Save changes to database
            return RedirectToAction(nameof(Index));  // Redirect to the Index (list of TakenAssessments)
        }

        // Helper method to check if TakenAssessment exists
        private bool TakenAssessmentExists(int id)
        {
            return _context.TakenAssessments.Any(e => e.AssessmentId == id);  // Check if the assessment exists in the database
        }
    }
}




/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MS3GUI.Models;

namespace MS3GUI.Controllers
{
    public class TakenAssessmentsController : Controller
    {
        private readonly DatabaseProjectContext _context;

        public TakenAssessmentsController(DatabaseProjectContext context)
        {
            _context = context;
        }

        // GET: TakenAssessments
        public async Task<IActionResult> Index()
        {
            var databaseProjectContext = _context.TakenAssessments.Include(t => t.Assessment).Include(t => t.Learner);
            return View(await databaseProjectContext.ToListAsync());
        }

        // GET: TakenAssessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var takenAssessment = await _context.TakenAssessments
                .Include(t => t.Assessment)
                .Include(t => t.Learner)
                .FirstOrDefaultAsync(m => m.AssessmentId == id);
            if (takenAssessment == null)
            {
                return NotFound();
            }

            return View(takenAssessment);
        }

        // GET: TakenAssessments/Create
        public IActionResult Create()
        {
            ViewData["AssessmentId"] = new SelectList(_context.Assessments, "Id", "Id");
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "LearnerId");
            return View();
        }

        // POST: TakenAssessments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Score,LearnerId,AssessmentId")] TakenAssessment takenAssessment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(takenAssessment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssessmentId"] = new SelectList(_context.Assessments, "Id", "Id", takenAssessment.AssessmentId);
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "LearnerId", takenAssessment.LearnerId);
            return View(takenAssessment);
        }

        // GET: TakenAssessments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var takenAssessment = await _context.TakenAssessments.FindAsync(id);
            if (takenAssessment == null)
            {
                return NotFound();
            }
            ViewData["AssessmentId"] = new SelectList(_context.Assessments, "Id", "Id", takenAssessment.AssessmentId);
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "LearnerId", takenAssessment.LearnerId);
            return View(takenAssessment);
        }

        // POST: TakenAssessments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Score,LearnerId,AssessmentId")] TakenAssessment takenAssessment)
        {
            if (id != takenAssessment.AssessmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(takenAssessment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TakenAssessmentExists(takenAssessment.AssessmentId))
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
            ViewData["AssessmentId"] = new SelectList(_context.Assessments, "Id", "Id", takenAssessment.AssessmentId);
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "LearnerId", takenAssessment.LearnerId);
            return View(takenAssessment);
        }

        // GET: TakenAssessments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var takenAssessment = await _context.TakenAssessments
                .Include(t => t.Assessment)
                .Include(t => t.Learner)
                .FirstOrDefaultAsync(m => m.AssessmentId == id);
            if (takenAssessment == null)
            {
                return NotFound();
            }

            return View(takenAssessment);
        }

        // POST: TakenAssessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var takenAssessment = await _context.TakenAssessments.FindAsync(id);
            if (takenAssessment != null)
            {
                _context.TakenAssessments.Remove(takenAssessment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TakenAssessmentExists(int id)
        {
            return _context.TakenAssessments.Any(e => e.AssessmentId == id);
        }
    }
}
*/
