using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.ViewComponents.Departments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly SchoolContext _context;

        public DepartmentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index(bool? loadDetails)
        {
            var parameters = new { loadDetails = loadDetails };
            return ViewComponent(typeof(IndexViewComponent), parameters);
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var department = await _context.Departments
                                           .AsNoTracking()
                                           .Include(x => x.Administrator)
                                           .Where(x => x.DepartmentID == id.GetValueOrDefault())
                                           .FirstOrDefaultAsync();

            if (department == null)
            {
                this.HttpContext.Response.Headers.Append("HX-Location", "/Departments?loadDetails=true");
                this.HttpContext.Response.Headers.Append("HX-Retarget", "#detailList");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return NotFound();
            }

            return ViewComponent(typeof(DetailViewComponent), department);
        }

        // GET: Departments/Create
        public async Task<IActionResult> Create()
        {
            var instructors = await _context.Instructors.ToArrayAsync();
            ViewData["Instructors"] = new SelectList(instructors, "ID", "FullName");
            return ViewComponent(typeof(CreateViewComponent), new Department());
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
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

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var department = await _context.Departments
                                           .AsNoTracking()
                                           .Include(i => i.Administrator)
                                           .Where(x => x.DepartmentID == id.GetValueOrDefault())
                                           .FirstOrDefaultAsync();

            if (department == null)
            {
                return NotFound();
            }

            var instructors = await _context.Instructors.ToArrayAsync();
            ViewData["Instructors"] = new SelectList(instructors, "ID", "FullName", department.InstructorID);
            return ViewComponent(typeof(EditViewComponent), department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Department department)
        {
            if (department == null)
                return Ok();

            var departmentToUpdate = await _context.Departments
                                                   .Where(x => x.DepartmentID == department.DepartmentID)
                                                   .FirstOrDefaultAsync();

            if (departmentToUpdate == null)
            {
                string errorMsg = "Unable to save changes. The department was deleted by another user.";

                this.HttpContext.Response.Headers.Append("HX-Retarget", "#error-message");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return Ok(errorMsg);
            }

            _context.Entry(departmentToUpdate).Property("RowVersion").OriginalValue = department.RowVersion;

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

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var department = await _context.Departments
                                           .AsNoTracking()
                                           .Include(x => x.Administrator)
                                           .Where(x => x.DepartmentID == id.GetValueOrDefault())
                                           .FirstOrDefaultAsync();

            if (department == null)
            {
                this.HttpContext.Response.Headers.Append("HX-Location", "/Departments?loadDetails=true");
                this.HttpContext.Response.Headers.Append("HX-Retarget", "#detailList");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return NotFound();
            }

            return ViewComponent(typeof(DeleteViewComponent), department);
        }


        // POST: Departments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Department department)
        {
            try
            {
                var departmentToDelete = await _context.Departments
                                                       .Where(x => x.DepartmentID == department.DepartmentID)
                                                       .FirstOrDefaultAsync();             
                
                if (departmentToDelete != null)
                {
                    _context.Entry(departmentToDelete).Property("RowVersion").OriginalValue = department.RowVersion;
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
