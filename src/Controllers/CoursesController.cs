using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Enums;
using ContosoUniversity.Models;
using ContosoUniversity.ViewComponents.Courses;

namespace ContosoUniversity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoursesController : Controller
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? loadDetails)
        {
            var parameters = new { loadDetails = loadDetails };
            return ViewComponent(typeof(IndexViewComponent), parameters);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetDetail([FromRoute] int id, 
                                                   [FromQuery] ActionType action)
        {
            Course? entity = null;
            IEnumerable<Department> departments = Enumerable.Empty<Department>();

            if (action == ActionType.Create ||
                action == ActionType.Edit)
            {
                departments = await _context.Departments.ToArrayAsync();
            }

            if (action == ActionType.View ||
                action == ActionType.Delete ||
                action == ActionType.Edit)
            {
                entity = await _context.Courses
                                       .AsNoTracking()
                                       .Include(x => x.Department)
                                       .Where(x => x.CourseID == id)
                                       .FirstOrDefaultAsync();

                if (action == ActionType.Unknown ||
                    entity == null)
                {
                    this.HttpContext.Response.Headers.Append("HX-Location", "/Courses?loadDetails=true");
                    this.HttpContext.Response.Headers.Append("HX-Retarget", "#detailList");
                    this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                    return NotFound();
                }
            }

            IActionResult actionResult = Ok();

            switch (action)
            {
                case ActionType.Create:
                    ViewData["Departments"] = new SelectList(departments, "DepartmentID", "Name");
                    actionResult = ViewComponent(typeof(CreateViewComponent), new Course());
                    break;

                case ActionType.Delete:
                    actionResult = ViewComponent(typeof(DeleteViewComponent), entity);
                    break;

                case ActionType.Edit:
                    ViewData["Departments"] = new SelectList(departments, "DepartmentID", "Name", entity.DepartmentID);
                    actionResult = ViewComponent(typeof(EditViewComponent), entity);
                    break;

                case ActionType.View:
                    actionResult = ViewComponent(typeof(DetailViewComponent), entity);
                    break;
            }

            return actionResult;
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();

                this.HttpContext.Response.Headers.Append("HX-Trigger", "listChanged");
                return Ok();
            }
            else
            {
                string errorMsg = "The course you attempted to create is invalid.  Please make the appropriate corrections.";

                this.HttpContext.Response.Headers.Append("HX-Retarget", "#error-message");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return Ok(errorMsg);
            }
        }


        // PUT: Courses/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // For Validation and Binding issues see the following
        // https://github.com/dotnet/aspnetcore/issues/29877
        // https://andrewlock.net/preventing-mass-assignment-or-over-posting-in-asp-net-core/
        [HttpPut]
        [Route("{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromRoute] int id, 
                                                [FromForm] Course course)
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


        // DELETE: Courses/5
        [HttpDelete]
        [Route("{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var courseToDelete = await _context.Courses
                                                   .Where(x => x.CourseID == id)
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

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _context.Departments
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery.AsNoTracking(), "DepartmentID", "Name", selectedDepartment);
        }
    }
}
