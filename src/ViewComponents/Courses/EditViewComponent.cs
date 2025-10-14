using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.ViewComponents.Courses
{
    public class EditViewComponent : ViewComponent
    {
        private readonly SchoolContext _context;

        public EditViewComponent(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Course course)
        {
            ViewData["IsReadOnly"] = false;
            return View(course);
        }
    }
}
