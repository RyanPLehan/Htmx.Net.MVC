using ContosoUniversity.Data;
using ContosoUniversity.Enums;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;
using ContosoUniversity.ViewComponents.Instructors;
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
    public class InstructorsController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Instructors
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? loadDetails, 
                                                [FromQuery] int? id, 
                                                [FromQuery] int? courseID)
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
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetDetail([FromRoute] int id, 
                                                   [FromQuery] ActionType action)
        {
            Instructor? entity = null;
            IEnumerable<AssignedCourseData> assignedCourseData = Enumerable.Empty<AssignedCourseData>();

            if (action == ActionType.Create ||
                action == ActionType.Edit)
            {
                assignedCourseData = await PopulateAssignedCourseData(id);
            }

            if (action == ActionType.View ||
                action == ActionType.Delete ||
                action == ActionType.Edit)
            {
                entity = await _context.Instructors
                                       .AsNoTracking()
                                       .Include(x => x.OfficeAssignment)
                                       .Include(x => x.CourseAssignments).ThenInclude(i => i.Course)
                                       .Where(x => x.ID == id)
                                       .FirstOrDefaultAsync();

                if (action == ActionType.Unknown ||
                    entity == null)
                {
                    this.HttpContext.Response.Headers.Append("HX-Location", "/Instructors?loadDetails=true");
                    this.HttpContext.Response.Headers.Append("HX-Retarget", "#detailList");
                    this.HttpContext.Response.Headers.Append("HX-Reswap", "innerHTML");
                    return NotFound();
                }
            }

            IActionResult actionResult = Ok();

            switch (action)
            {
                case ActionType.Create:
                    ViewData["Courses"] = assignedCourseData;
                    actionResult = ViewComponent(typeof(CreateViewComponent), new Instructor());
                    break;

                case ActionType.Delete:
                    actionResult = ViewComponent(typeof(DeleteViewComponent), entity);
                    break;

                case ActionType.Edit:
                    ViewData["Courses"] = assignedCourseData;
                    actionResult = ViewComponent(typeof(EditViewComponent), entity);
                    break;

                case ActionType.View:
                    actionResult = ViewComponent(typeof(DetailViewComponent), entity);
                    break;
            }

            return actionResult;

        }

        // POST: Instructors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] InstructorRequest request)
        {
            if (ModelState.IsValid)
            {
                UpdateInstructorCourses(request, request.SelectedCourses);
                _context.Add(request);
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


        // PUT: Instructors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPut]
        [Route("{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromRoute] int id,
                                                [FromForm] InstructorRequest request)
        {
            var instructorToUpdate = await _context.Instructors
                                                   .Include(i => i.OfficeAssignment)
                                                   .Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
                                                   .Where(x => x.ID == id)
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

                    UpdateInstructorCourses(instructorToUpdate, request.SelectedCourses);
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


        // DELETE: Instructors/Delete/5
        [HttpDelete]
        [Route("{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                Instructor instructorToDelete = await _context.Instructors
                                                              .Where(x => x.ID == id)
                                                              .FirstOrDefaultAsync();

                if (instructorToDelete != null)
                {
                    var departments = await _context.Departments
                                                    .Where(d => d.InstructorID == id)
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
            if (instructor.CourseAssignments == null)
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
    }
}
