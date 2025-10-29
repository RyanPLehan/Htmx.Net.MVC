using ContosoUniversity.Data;
using ContosoUniversity.Enums;
using ContosoUniversity.Helpers;
using ContosoUniversity.Models;
using ContosoUniversity.ViewComponents.Departments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartmentsController : Controller
    {
        private readonly SchoolContext _context;

        public DepartmentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Departments
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? loadDetails)
        {
            var parameters = new { loadDetails = loadDetails };
            return ViewComponent(typeof(IndexViewComponent), parameters);
        }

        // GET: Departments/Details/5
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetDetail([FromRoute] int id, 
                                                   [FromQuery] ActionType action)
        {
            Department? entity = null;
            IEnumerable<Instructor> instructors = Enumerable.Empty<Instructor>();

            if (action == ActionType.Create ||
                action == ActionType.Edit)
            {
                instructors = await _context.Instructors.ToArrayAsync();
            }


            if (action == ActionType.View ||
                action == ActionType.Delete ||
                action == ActionType.Edit)
            {
                entity = await _context.Departments
                                       .AsNoTracking()
                                       .Include(x => x.Administrator)
                                       .Where(x => x.DepartmentID == id)
                                       .FirstOrDefaultAsync();

                if (action == ActionType.Unknown ||
                    entity == null)
                {
                    this.HttpContext.Response.Headers.Append("HX-Location", "/Departments?loadDetails=true");
                    this.HttpContext.Response.Headers.Append("HX-Retarget", "#detailList");
                    this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                    return NotFound();
                }
            }

            IActionResult actionResult = Ok();

            switch (action)
            {
                case ActionType.Create:
                    ViewData["Instructors"] = new SelectList(instructors, "ID", "FullName");
                    actionResult = ViewComponent(typeof(CreateViewComponent), new Department());
                    break;

                case ActionType.Delete:
                    actionResult = ViewComponent(typeof(DeleteViewComponent), entity);
                    break;

                case ActionType.Edit:
                    ViewData["Instructors"] = new SelectList(instructors, "ID", "FullName", entity.InstructorID);
                    actionResult = ViewComponent(typeof(EditViewComponent), entity);
                    break;

                case ActionType.View:
                    actionResult = ViewComponent(typeof(DetailViewComponent), entity);
                    break;
            }

            return actionResult;

        }


        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();

                this.HttpContext.Response.Headers.Append("HX-Trigger", "listChanged");
                return Ok();
            }
            else
            {
                string errorMsg = "The department you attempted to create is invalid.  Please make the appropriate corrections.";

                this.HttpContext.Response.Headers.Append("HX-Retarget", "#error-message");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return Ok(errorMsg);
            }
        }


        // PUT: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // For Route Attribute see: https://learn.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2
        [HttpPut]
        [Route("{id:int}/{version}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromRoute] int id, 
                                                [FromRoute] string version, 
                                                [FromForm] Department department)
        {
            if (department == null)
                return Ok();

            byte[] rowVersion = RowVersionHelper.FromHexString(version ?? string.Empty);
            var departmentToUpdate = await _context.Departments
                                                   .Where(x => x.DepartmentID == id)
                                                   .FirstOrDefaultAsync();

            if (departmentToUpdate == null)
            {
                string errorMsg = "Unable to save changes. The department was deleted by another user.";

                this.HttpContext.Response.Headers.Append("HX-Retarget", "#error-message");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return Ok(errorMsg);
            }

            _context.Entry(departmentToUpdate).Property("RowVersion").OriginalValue = rowVersion;
            //_context.Entry(departmentToUpdate).Property("RowVersion").OriginalValue = department.RowVersion;

            if (await TryUpdateModelAsync<Department>(
                departmentToUpdate,
                "",
                s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    this.HttpContext.Response.Headers.Append("HX-Trigger", "listChanged");
                    return Ok();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    string errorMsg = "The record you attempted to edit "
                                + "was modified by another user after you got the original value. "
                                + "Go back to the list to review new values.";

                    this.HttpContext.Response.Headers.Append("HX-Retarget", "#error-message");
                    this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                    return Ok(errorMsg);
                }
            }
            return Ok();
        }

        // DELETE: Departments/Delete/5
        [HttpDelete]
        [Route("{id:int}/{version}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, 
                                                [FromRoute] string version)
        {
            try
            {
                byte[] rowVersion = RowVersionHelper.FromHexString(version ?? string.Empty);
                var departmentToDelete = await _context.Departments
                                                       .Where(x => x.DepartmentID == id)
                                                       .FirstOrDefaultAsync();             
                
                if (departmentToDelete != null)
                {
                    _context.Entry(departmentToDelete).Property("RowVersion").OriginalValue = rowVersion;
                    _context.Departments.Remove(departmentToDelete);
                    await _context.SaveChangesAsync();
                }

                this.HttpContext.Response.Headers.Append("HX-Trigger", "listChanged");
                return Ok();
            }

            catch (DbUpdateConcurrencyException /* ex */)
            {
                string errorMsg = "The record you attempted to delete "
                        + "was modified by another user after you got the original values. "
                        + "The delete operation was canceled and the current values in the "
                        + "database have been displayed. If you still want to delete this "
                        + "record, click the Delete button again. Otherwise "
                        + "click the Back to List hyperlink.";

                this.HttpContext.Response.Headers.Append("HX-Retarget", "#error-message");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return Ok(errorMsg);
            }
        }
    }
}
