using ContosoUniversity.Data;
using ContosoUniversity.Enums;
using ContosoUniversity.Models;
using ContosoUniversity.ViewComponents.Students;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ContosoUniversity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Students
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? columnName,
                                                [FromQuery] string? sortOrder,
                                                [FromQuery] string? filter,
                                                [FromQuery] string? searchString,
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
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetDetail([FromRoute] int id, 
                                                   [FromQuery] ActionType action)
        {
            Student? entity = null;

            if (action == ActionType.View ||
                action == ActionType.Delete ||
                action == ActionType.Edit)
            {
                entity = await _context.Students
                                       .AsNoTracking()
                                       .Where(x => x.ID == id)
                                       .Include(x => x.Enrollments)
                                           .ThenInclude(e => e.Course)
                                       .FirstOrDefaultAsync();

                if (action == ActionType.Unknown ||
                    entity == null)
                {
                    this.HttpContext.Response.Headers.Append("HX-Location", "/Students?pageNumber=1");
                    this.HttpContext.Response.Headers.Append("HX-Retarget", "#detailList");
                    this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                    return NotFound();
                }
            }

            IActionResult actionResult = Ok();

            switch (action)
            {
                case ActionType.Create:
                    actionResult = ViewComponent(typeof(CreateViewComponent), new Student());
                    break;

                case ActionType.Delete:
                    actionResult = ViewComponent(typeof(DeleteViewComponent), entity);
                    break;

                case ActionType.Edit:
                    actionResult = ViewComponent(typeof(EditViewComponent), entity);
                    break;

                case ActionType.View:
                    actionResult = ViewComponent(typeof(DetailViewComponent), entity);
                    break;
            }

            return actionResult;

        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("EnrollmentDate,FirstName,LastName")][FromForm] Student student)
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

        // PUT: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPut]
        [Route("{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromRoute] int id, 
                                                [FromForm] Student student)
        {
            if (student == null)
                return Ok();

            var studentToUpdate = await _context.Students
                                                .Where(x => x.ID == id)
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

        // DELETE: Students/Delete/5
        [HttpDelete]
        [Route("{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var studentToDelete = await _context.Students
                                                    .Where(x => x.ID == id)
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
