using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.ViewComponents.Courses;

namespace ContosoUniversity.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index(bool? loadDetails)
        {
            var parameters = new { loadDetails = loadDetails };
            return ViewComponent(typeof(IndexViewComponent), parameters);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var course = await _context.Courses
                                       .AsNoTracking()
                                       .Include(x => x.Department)
                                       .FirstOrDefaultAsync(x => x.CourseID == id.GetValueOrDefault());

            if (course == null)
            {
                this.HttpContext.Response.Headers.Append("HX-Location", "/Courses?loadDetails=true");
                this.HttpContext.Response.Headers.Append("HX-Retarget", "#detailList");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return NotFound();
            }

            return ViewComponent(typeof(DetailViewComponent), course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Credits,DepartmentID,Title")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var course = await _context.Courses
                                       .AsNoTracking()
                                       .Include(i => i.Department)
                                       .Where(x => x.CourseID == id.GetValueOrDefault())
                                       .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            var deparments = await _context.Departments.ToArrayAsync();
            ViewData["Departments"] = new SelectList(deparments, "DepartmentID", "Name", course.DepartmentID);
            return ViewComponent(typeof(EditViewComponent), course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // For Validation and Binding issues see the following
        // https://github.com/dotnet/aspnetcore/issues/29877
        // https://andrewlock.net/preventing-mass-assignment-or-over-posting-in-asp-net-core/
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Course course)
        {
            if (course == null)
                return Ok();

            var courseToUpdate = await _context.Courses
                                               .Where(x => x.CourseID == course.CourseID)
                                               .FirstOrDefaultAsync();

            if (courseToUpdate == null)
            {
                string errorMsg = "Unable to save changes. The course was deleted by another user.";

                this.HttpContext.Response.Headers.Append("HX-Retarget", "#error-message");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return Ok(errorMsg);
            }

            if (await TryUpdateModelAsync<Course>(
                courseToUpdate,
                "",
                s => s.Title, s => s.Credits, s => s.DepartmentID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    this.HttpContext.Response.Headers.Append("HX-Trigger", "listChanged");
                    return Ok();
                }
                catch (DbUpdateException ex)
                {
                    string errorMsg = "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.";

                    this.HttpContext.Response.Headers.Append("HX-Retarget", "#error-message");
                    this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                    return Ok(errorMsg);
                }
            }
            return Ok();
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _context.Departments
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery.AsNoTracking(), "DepartmentID", "Name", selectedDepartment);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var course = await _context.Courses
                                        .Include(c => c.Department)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(m => m.CourseID == id.GetValueOrDefault());


            if (course == null)
            {
                this.HttpContext.Response.Headers.Append("HX-Location", "/Students?pageNumber=1");
                this.HttpContext.Response.Headers.Append("HX-Retarget", "#detailList");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return NotFound();
            }

            return ViewComponent(typeof(DeleteViewComponent), course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Course course)
        {
            try
            {
                var courseToDelete = await _context.Courses
                                                   .Where(x => x.CourseID == course.CourseID)
                                                   .FirstOrDefaultAsync();

                if (courseToDelete != null)
                {
                    _context.Courses.Remove(courseToDelete);
                    await _context.SaveChangesAsync();
                }

                this.HttpContext.Response.Headers.Append("HX-Trigger", "listChanged");
                return Ok();
            }

            catch (DbUpdateException /* ex */)
            {
                string errorMsg = "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";

                this.HttpContext.Response.Headers.Append("HX-Retarget", "#error-message");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return Ok(errorMsg);
            }
        }

        public IActionResult UpdateCourseCredits()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCourseCredits(int? multiplier)
        {
            if (multiplier != null)
            {
                ViewData["RowsAffected"] =
                    await _context.Courses
                                  .ExecuteUpdateAsync(x => x.SetProperty(c => c.Credits, c => c.Credits * multiplier));

                    /*
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE Course SET Credits = Credits * {0}",
                        parameters: multiplier);
                    */
            }
            return View();
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseID == id);
        }
    }
}
