using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MS3GUI.Models;
using System;
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
                ViewBag.ErrorMessage = "An error occurred while retrieving assessments.";
                return View("Error");
            }
        }

        // GET: Assessments/Create
        public IActionResult Create()
        {
            try
            {
                // Create a SelectList from the modules
                ViewData["Modules"] = new SelectList(_context.Modules, "ModuleId", "Title");
                return View();
            }
            catch (Exception ex)
            {
                // Log error
                ViewBag.ErrorMessage = "An error occurred while preparing the create form.";
                return View("Error");
            }
        }

        // POST: Assessments/Create
        // POST: Assessments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Atype,TotalMarks,PassingMarks,Criteria,Weightage,Adescription,Title,ModuleId,CourseId")] Assessment assessment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(assessment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log error
                    ViewBag.ErrorMessage = "An error occurred while creating the assessment.";
                    return View("Error");
                }
            }
            // Populating the ViewData for Modules and Courses to pass to the Create view
            ViewData["Modules"] = new SelectList(_context.Modules, "ModuleId", "Title", assessment.ModuleId);
            ViewData["Courses"] = new SelectList(_context.Courses, "CourseId", "CourseName", assessment.CourseId); // Assuming you have a Courses table
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



        // GET: Assessments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var assessment = await _context.Assessments.FindAsync(id);
                if (assessment == null) return NotFound();

                // Populate dropdown lists correctly
                ViewData["Modules"] = new SelectList(_context.Modules, "ModuleId", "Title", assessment.ModuleId);
                ViewData["Courses"] = new SelectList(_context.Courses, "CourseId", "CourseName", assessment.CourseId);

                return View(assessment);
            }
            catch (Exception ex)
            {
                // Log error
                ViewBag.ErrorMessage = "An error occurred while retrieving the assessment for editing.";
                return View("Error");
            }
        }


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
                    ViewBag.ErrorMessage = "An error occurred while editing the assessment.";
                    return View("Error");
                }
            }
            ViewData["Modules"] = new SelectList(_context.Modules, "ModuleId", "Title", assessment.ModuleId);
            ViewData["Courses"] = new SelectList(_context.Courses, "CourseId", "CourseName", assessment.CourseId);
            return View(assessment);
        }


        private bool AssessmentExists(int id)
        {
            return _context.Assessments.Any(e => e.Id == id);
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
            var databaseProjectContext = _context.Assessments.Include(a => a.Module);
            return View(await databaseProjectContext.ToListAsync());
        }

        // GET: Assessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessments
                .Include(a => a.Module)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assessment == null)
            {
                return NotFound();
            }

            return View(assessment);
        }

        // GET: Assessments/Create
        public IActionResult Create()
        {
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "ModuleId");
            return View();
        }

        // POST: Assessments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Atype,TotalMarks,PassingMarks,Criteria,Weightage,Adescription,Title,ModuleId,CourseId")] Assessment assessment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assessment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "ModuleId", assessment.ModuleId);
            return View(assessment);
        }

        // GET: Assessments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
            {
                return NotFound();
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "ModuleId", assessment.ModuleId);
            return View(assessment);
        }

        // POST: Assessments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Atype,TotalMarks,PassingMarks,Criteria,Weightage,Adescription,Title,ModuleId,CourseId")] Assessment assessment)
        {
            if (id != assessment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assessment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssessmentExists(assessment.Id))
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
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "ModuleId", assessment.ModuleId);
            return View(assessment);
        }

        // GET: Assessments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessments
                .Include(a => a.Module)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assessment == null)
            {
                return NotFound();
            }

            return View(assessment);
        }

        // POST: Assessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment != null)
            {
                _context.Assessments.Remove(assessment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssessmentExists(int id)
        {
            return _context.Assessments.Any(e => e.Id == id);
        }
    }
}
*/
