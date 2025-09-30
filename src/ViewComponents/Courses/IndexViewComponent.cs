using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ContosoUniversity.ViewComponents.Courses
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

        public async Task<IViewComponentResult> InvokeAsync(bool? loadDetails)
        {
            // Shell should be loaded only once
            if (!loadDetails.GetValueOrDefault())
                return View("Master");


            var courses = await _context.Courses
                                        .AsNoTracking()
                                        .Include(c => c.Department)
                                        .ToArrayAsync();
            return View("Details", courses);
        }

    }
}
