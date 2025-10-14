using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;
using ContosoUniversity.ViewComponents.Instructors;
using System.Security.Cryptography.X509Certificates;

namespace ContosoUniversity.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Instructors
        public async Task<IActionResult> Index(bool? loadDetails, int? id, int? courseID)
        {
            var parameters = new
            {
                loadDetails = loadDetails,
                id = id,
                courseID = courseID,
            };

            return ViewComponent(typeof(IndexViewComponent), parameters);
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var instructor = await _context.Instructors
                                           .AsNoTracking()
                                           .Include(x => x.OfficeAssignment)
                                           .Include(x => x.CourseAssignments).ThenInclude(i => i.Course)
                                           .Where(x => x.ID == id.GetValueOrDefault())
                                           .FirstOrDefaultAsync();

            if (instructor == null)
            {
                this.HttpContext.Response.Headers.Append("HX-Location", "/Instructors?loadDetails=true");
                this.HttpContext.Response.Headers.Append("HX-Retarget", "#detailList");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return NotFound();
            }

            return ViewComponent(typeof(DetailViewComponent), instructor);
        }

        // GET: Instructors/Create
        public async Task<IActionResult> Create()
        {
            var instructor = new Instructor();
            ViewData["Courses"] = await PopulateAssignedCourseData(instructor.ID);
            return ViewComponent(typeof(CreateViewComponent), new Instructor());
        }

        // POST: Instructors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Instructor instructor, string[] selectedCourses)
        {
            if (ModelState.IsValid)
            {
                UpdateInstructorCourses(instructor, selectedCourses);
                _context.Add(instructor);
                await _context.SaveChangesAsync();

                this.HttpContext.Response.Headers.Append("HX-Trigger", "listChanged");
                return Ok();
            }
            else
            {
                string errorMsg = "The instructor you attempted to create is invalid.  Please make the appropriate corrections.";

                this.HttpContext.Response.Headers.Append("HX-Retarget", "#error-message");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return Ok(errorMsg);
            }
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var instructor = await _context.Instructors
                                           .AsNoTracking()
                                           .Include(i => i.OfficeAssignment)
                                           .Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
                                           .Where(x => x.ID == id.GetValueOrDefault())
                                           .FirstOrDefaultAsync();

            if (instructor == null)
            {
                this.HttpContext.Response.Headers.Append("HX-Location", "/Instructors?loadDetails=true");
                this.HttpContext.Response.Headers.Append("HX-Retarget", "#detailList");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return NotFound();
            }

            ViewData["Courses"] = await PopulateAssignedCourseData(id.GetValueOrDefault());
            return ViewComponent(typeof(EditViewComponent), instructor);
        }


        // POST: Instructors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Instructor instructor, string[] selectedCourses)
        {
            var instructorToUpdate = await _context.Instructors
                                                   .Include(i => i.OfficeAssignment)
                                                   .Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
                                                   .Where(x => x.ID == instructor.ID)
                                                   .FirstOrDefaultAsync();

            if (await TryUpdateModelAsync<Instructor>(
                instructorToUpdate,
                "",
                i => i.FirstName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment))
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location))
                    {
                        instructorToUpdate.OfficeAssignment = null;
                    }

                    UpdateInstructorCourses(instructorToUpdate, selectedCourses);
                    await _context.SaveChangesAsync();
                    this.HttpContext.Response.Headers.Append("HX-Trigger", "listChanged");
                    return Ok();
                }

                catch (DbUpdateException /* ex */)
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

        private async Task<IEnumerable<AssignedCourseData>> PopulateAssignedCourseData(int instructorId)
        {
            return await
                (
                    from c in _context.Courses
                    join ca in _context.CourseAssignments on
                    new { CourseID = c.CourseID, InstructorID = instructorId }
                    equals
                    new { CourseID = ca.CourseID, InstructorID = ca.InstructorID }
                    into joinResult
                    from jr in joinResult.DefaultIfEmpty()
                    select new AssignedCourseData
                    {
                        CourseID = c.CourseID,
                        Title = c.Title,
                        Assigned = (jr.CourseID == null ? false : true)
                    }
                ).ToArrayAsync();

            /* Use sub-select first
            return await (
                             from c in _context.Courses
                             join sub in
                             (
                                from ca in _context.CourseAssignments
                                where ca.InstructorID == instructorId
                                select new { CourseID = ca.CourseID }
                             ) on c.CourseID equals sub.CourseID
                             into joinResult
                             from jr in joinResult.DefaultIfEmpty()
                             select new AssignedCourseData
                             {
                                 CourseID = c.CourseID,
                                 Title = c.Title,
                                 Assigned = (jr.CourseID == null ? false : true)
                             }
                        ).ToArrayAsync();
            */
        }


        private void UpdateInstructorCourses(Instructor instructor, string[] selectedCourses)
        {
            // Easiest way is to remove all assigned courses, then add in only the selected ones
            if (instructor.CourseAssignments ==  null)
                instructor.CourseAssignments = new List<CourseAssignment>();

            instructor.CourseAssignments.Clear();

            if (selectedCourses == null)
                return;

            instructor.CourseAssignments = selectedCourses.Select(x => new CourseAssignment()
                                                                        {
                                                                            InstructorID = instructor.ID,
                                                                            CourseID = Int32.Parse(x)
                                                                        }).ToList();
        }

        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var instructor = await _context.Instructors
                                           .AsNoTracking()
                                           .Include(x => x.OfficeAssignment)
                                           .Include(x => x.CourseAssignments).ThenInclude(i => i.Course)
                                           .Where(x => x.ID == id.GetValueOrDefault())
                                           .FirstOrDefaultAsync();

            if (instructor == null)
            {
                this.HttpContext.Response.Headers.Append("HX-Location", "/Instructors?loadDetails=true");
                this.HttpContext.Response.Headers.Append("HX-Retarget", "#detailList");
                this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                return NotFound();
            }

            return ViewComponent(typeof(DeleteViewComponent), instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Instructor instructor)
        {
            try
            {
                Instructor instructorToDelete = await _context.Instructors
                                                              .Where(x => x.ID == instructor.ID)
                                                              .FirstOrDefaultAsync();

                if (instructorToDelete != null)
                {
                    var departments = await _context.Departments
                                                    .Where(d => d.InstructorID == instructor.ID)
                                                    .ToListAsync();
                    departments.ForEach(d => d.InstructorID = null);

                    _context.Instructors.Remove(instructorToDelete);
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
