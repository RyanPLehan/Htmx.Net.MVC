using ContosoUniversity.Data;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.ViewComponents.Home
{
    /// <summary>
    /// Display Privacy statement
    /// </summary>
    public class AboutViewComponent : ViewComponent
    {
        private readonly SchoolContext _context;

        public AboutViewComponent(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<EnrollmentDateGroup> groups;

            groups = await _context.Students
                                   .GroupBy(x => x.EnrollmentDate)
                                   .Select(grp => new EnrollmentDateGroup()
                                   {
                                       EnrollmentDate = grp.Key,
                                       StudentCount = grp.Count()
                                   })
                                   .ToArrayAsync();

            return View(groups);
        }
    }
}
