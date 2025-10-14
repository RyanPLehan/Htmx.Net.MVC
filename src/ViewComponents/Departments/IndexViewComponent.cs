using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.ViewComponents.Departments
{
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


            var departments = await _context.Departments
                                            .Include(d => d.Administrator)
                                            .OrderBy(x => x.Name)
                                            .ToArrayAsync();
            return View("Details", departments);
        }
    }
}
