using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.ViewComponents.Students;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ContosoUniversity.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(
            [FromQuery] string columnName,
            [FromQuery] string sortOrder,
            [FromQuery] string filter,
            [FromQuery] string searchString,
            [FromQuery] int? pageNumber)
        {
            var parameters = new {
                columnName = columnName,
                sortOrder = sortOrder,
                filter = filter,
                searchString = searchString,
                pageNumber = pageNumber,
            };

            return ViewComponent(typeof(IndexViewComponent), parameters);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var student = await _context.Students
                                        .AsNoTracking()
                                        .Where(x => x.ID == id.GetValueOrDefault())
                                        .Include(x => x.Enrollments)
                                            .ThenInclude(e => e.Course)
                                        .FirstOrDefaultAsync();

            if (student == null)
            {
                this.HttpContext.Response.Headers.Append("HX-Location", "/Students?pageNumber=1");
                this.HttpContext.Response.Headers.Append("HX-Retarget", "#shell-content");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return NotFound();
            }

            return ViewComponent(typeof(DetailViewComponent), student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return ViewComponent(typeof(CreateViewComponent), new Student());
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("EnrollmentDate,FirstName,LastName")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();

                this.HttpContext.Response.Headers.Append("HX-Trigger", "listChanged");
                return Ok();
            }
            else
            {
                string errorMsg = "The student you attempted to create is invalid.  Please make the appropriate corrections.";

                this.HttpContext.Response.Headers.Append("HX-Retarget", "#error-message");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return Ok(errorMsg);
            }
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var student = await _context.Students
                                        .AsNoTracking()
                                        .Where(x => x.ID == id.GetValueOrDefault())
                                        .Include(x => x.Enrollments)
                                            .ThenInclude(e => e.Course)
                                        .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound();
            }

            return ViewComponent(typeof(EditViewComponent), student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Student student)
        {
            if (student == null)
                return Ok();

            var studentToUpdate = await _context.Students
                                                .Where(x => x.ID == student.ID)
                                                .FirstOrDefaultAsync();

            if (studentToUpdate == null)
            {
                string errorMsg = "Unable to save changes. The student was deleted by another user.";

                this.HttpContext.Response.Headers.Append("HX-Retarget", "#error-message");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return Ok(errorMsg);
            }

            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "",
                s => s.LastName, s => s.FirstName, s => s.EnrollmentDate))
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

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var student = await _context.Students
                                        .AsNoTracking()
                                        .Where(x => x.ID == id.GetValueOrDefault())
                                        .Include(x => x.Enrollments)
                                            .ThenInclude(e => e.Course)
                                        .FirstOrDefaultAsync();


            if (student == null)
            {
                this.HttpContext.Response.Headers.Append("HX-Location", "/Students?pageNumber=1");
                this.HttpContext.Response.Headers.Append("HX-Retarget", "#shell-content");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return NotFound();
            }

            return ViewComponent(typeof(DeleteViewComponent), student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Student student)
        {
            try
            {
                var studentToDelete = await _context.Students
                                                    .Where(x => x.ID == student.ID)
                                                    .FirstOrDefaultAsync();

                if (studentToDelete != null)
                {
                    _context.Students.Remove(studentToDelete);
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
    }
}
