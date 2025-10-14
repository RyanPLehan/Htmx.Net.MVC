using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.ViewComponents.Courses
{
    public class DetailViewComponent : ViewComponent
    {
        private readonly SchoolContext _context;

        public DetailViewComponent(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Course course)
        {
            ViewData["IsReadOnly"] = true;
            return View(course);
        }
    }
}
