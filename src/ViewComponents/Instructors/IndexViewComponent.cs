using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ContosoUniversity.ViewComponents.Instructors
{
    /// <summary>
    /// Display Privacy statement
    /// </summary>
    public class IndexViewComponent : ViewComponent
    {
        private readonly SchoolContext _context;

        public IndexViewComponent(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? id, int? courseID)
        {
            // Get Courses by InstructorID
            if (id != null)
            {
                var instructor = await GetInstructor(id.GetValueOrDefault());
                if (instructor != null)
                {
                    ViewData["InstructorID"] = instructor.ID;
                    ViewData["InstructorName"] = $"{instructor.LastName}, {instructor.FirstName}";
                }
                else
                {
                    ViewData["InstructorID"] = 0;
                    ViewData["InstructorName"] = "Selected Instructor";
                }

                return View("Courses", await GetCourses(id.Value));
            }
            
            // Get Enrollments by CourseId
            else if (courseID != null)
            {
                var course = await GetCourse(courseID.GetValueOrDefault());
                if (course != null)
                {
                    ViewData["CourseID"] = course.CourseID;
                    ViewData["CourseTitle"] = course.Title;
                }
                else
                {
                    ViewData["CourseID"] = 0;
                    ViewData["CourseTitle"] = "Selected Course";
                }

                return View("Enrollments", await GetEnrollments(courseID.Value));
            }

            // Get the entire list all the instructors
            else
            {
                return View(await GetInstructors());
            }
        }

        private async Task<Instructor?> GetInstructor(int id)
        {
            return await _context.Instructors
                                 .AsNoTracking()
                                 .Where(x => x.ID == id)
                                 .FirstOrDefaultAsync();
        }

        private async Task<IEnumerable<Instructor>> GetInstructors()
        {
            return await _context.Instructors
                                 .AsNoTracking()
                                 .Include(i => i.OfficeAssignment)
                                 .Include(i => i.CourseAssignments)
                                    .ThenInclude(i => i.Course)
                                        .ThenInclude(i => i.Department)
                                 .OrderBy(i => i.LastName)
                                 .ToArrayAsync();
        }

        private async Task<Course?> GetCourse(int courseId)
        {
            return await _context.Courses
                                 .AsNoTracking()
                                 .Where(x => x.CourseID == courseId)
                                 .FirstOrDefaultAsync();
        }

        private async Task<IEnumerable<Course>> GetCourses(int instructorId)
        {
            var queryCourseId = _context.CourseAssignments
                            .Where(x => x.InstructorID == instructorId)
                            .Select(x => x.CourseID);

            return await _context.Courses
                                 .AsNoTracking()
                                 .Include(i => i.Department)
                                 .Where(x => queryCourseId.Contains(x.CourseID))
                                 .ToArrayAsync();
        }

        private async Task<IEnumerable<Enrollment>> GetEnrollments(int courseId)
        {
            return await _context.Enrollments
                                 .AsNoTracking()
                                 .Include(i => i.Student)
                                 .Where(x => x.CourseID == courseId)
                                 .ToArrayAsync();
        }
    }
}
