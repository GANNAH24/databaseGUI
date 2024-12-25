// TAKEN ASSESSMENT CONTROLLOR 



using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MS3GUI.Models;

public class TakenAssessmentsController : Controller
{
    private readonly DatabaseProjectContext _context;

    public TakenAssessmentsController(DatabaseProjectContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var takenAssessments = _context.TakenAssessments.Include(t => t.Assessment).Include(t => t.Learner);
        return View(await takenAssessments.ToListAsync());
    }

    //public async Task<IActionResult> Details(int? assessmentId, int? learnerId)
    //{
    //    if (assessmentId == null || learnerId == null)
    //    {
    //        return NotFound();
    //    }

    //    var takenAssessment = await _context.TakenAssessments
    //        .Include(t => t.Assessment)
    //        .Include(t => t.Learner)
    //        .FirstOrDefaultAsync(m => m.AssessmentId == assessmentId && m.LearnerId == learnerId);

    //    if (takenAssessment == null)
    //    {
    //        return NotFound();
    //    }

    //    return View(takenAssessment);
    //}

    public async Task<IActionResult> Details(int? assessmentId, int? learnerId)
    {
        if (assessmentId == null || learnerId == null)
        {
            return NotFound();
        }

        var takenAssessment = await _context.TakenAssessments
            .Include(t => t.Assessment)
            .FirstOrDefaultAsync(m => m.AssessmentId == assessmentId && m.LearnerId == learnerId);

        if (takenAssessment == null)
        {
            return NotFound();
        }

        var performanceAnalysis = await _context.GetPerformanceAnalysisAsync(learnerId.Value);

        var viewModel = new TakenAssessmentDetailsViewModel
        {
            TakenAssessment = takenAssessment,
            PerformanceAnalysis = performanceAnalysis
        };

        return View(viewModel);
    }


    public IActionResult Create()
    {
        ViewData["AssessmentId"] = new SelectList(_context.Assessments, "Id", "Title");
        ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "Name");
        return View();
    }

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
        ViewData["AssessmentId"] = new SelectList(_context.Assessments, "Id", "Title", takenAssessment.AssessmentId);
        ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "Name", takenAssessment.LearnerId);
        return View(takenAssessment);
    }

    public async Task<IActionResult> Edit(int? assessmentId, int? learnerId)
    {
        if (assessmentId == null || learnerId == null)
        {
            return NotFound();
        }

        var takenAssessment = await _context.TakenAssessments.FindAsync(assessmentId, learnerId);
        if (takenAssessment == null)
        {
            return NotFound();
        }
        ViewData["AssessmentId"] = new SelectList(_context.Assessments, "Id", "Title", takenAssessment.AssessmentId);
        ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "Name", takenAssessment.LearnerId);
        return View(takenAssessment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int assessmentId, int learnerId, [Bind("Score,LearnerId,AssessmentId")] TakenAssessment takenAssessment)
    {
        if (assessmentId != takenAssessment.AssessmentId || learnerId != takenAssessment.LearnerId)
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
                if (!TakenAssessmentExists(takenAssessment.AssessmentId, takenAssessment.LearnerId))
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
        ViewData["AssessmentId"] = new SelectList(_context.Assessments, "Id", "Title", takenAssessment.AssessmentId);
        ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "Name", takenAssessment.LearnerId);
        return View(takenAssessment);
    }

    public async Task<IActionResult> Delete(int? assessmentId, int? learnerId)
    {
        if (assessmentId == null || learnerId == null)
        {
            return NotFound();
        }

        var takenAssessment = await _context.TakenAssessments
            .Include(t => t.Assessment)
            .Include(t => t.Learner)
            .FirstOrDefaultAsync(m => m.AssessmentId == assessmentId && m.LearnerId == learnerId);

        if (takenAssessment == null)
        {
            return NotFound();
        }

        return View(takenAssessment);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int assessmentId, int learnerId)
    {
        var takenAssessment = await _context.TakenAssessments.FindAsync(assessmentId, learnerId);
        _context.TakenAssessments.Remove(takenAssessment);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TakenAssessmentExists(int assessmentId, int learnerId)
    {
        return _context.TakenAssessments.Any(e => e.AssessmentId == assessmentId && e.LearnerId == learnerId);
    }


    public async Task<IActionResult> Review(int? learnerId, int? instructorId)
    {
        if (learnerId == null && instructorId == null)
        {
            return NotFound();
        }

        IQueryable<TakenAssessment> query = _context.TakenAssessments
            .Include(t => t.Assessment)
            .ThenInclude(a => a.Course)
            .ThenInclude(c => c.Instructors)
            .Include(t => t.Learner);

        if (learnerId != null)
        {
            query = query.Where(t => t.LearnerId == learnerId);
        }
        else if (instructorId != null)
        {
            query = query.Where(t => t.Assessment.Course.Instructors.Any(i => i.InstructorId == instructorId));
        }

        var takenAssessments = await query.ToListAsync();
        return View(takenAssessments);
    }

    public async Task<IActionResult> LearnerTakenAssessments(int learnerId)
    {
        var takenAssessments = _context.TakenAssessments
            .Include(t => t.Assessment)
            .Include(t => t.Learner)
            .Where(t => t.LearnerId == learnerId);
        return View(await takenAssessments.ToListAsync());
    }



    public async Task<IActionResult> ScoreBreakdown(int learnerId)
    {
        var scores = await _context.TakenAssessments
            .Where(t => t.LearnerId == learnerId)
            .Include(t => t.Assessment)
            .ToListAsync();

        return View(scores);
    }


    public async Task<IActionResult> ScoreAnalytics(int assessmentId)
    {
        var scores = await _context.TakenAssessments
            .Where(t => t.AssessmentId == assessmentId)
            .ToListAsync();

        // Calculate analytics (e.g., average score, highest score, etc.)
        var analytics = new
        {
            AverageScore = scores.Average(t => t.Score),
            HighestScore = scores.Max(t => t.Score),
            LowestScore = scores.Min(t => t.Score)
        };

        return View(analytics);
    }


}

