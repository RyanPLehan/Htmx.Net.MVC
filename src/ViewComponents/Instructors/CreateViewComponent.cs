using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.ViewComponents.Instructors
{
    public class CreateViewComponent : ViewComponent
    {
        private readonly SchoolContext _context;

        public CreateViewComponent(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Instructor instructor)
        {
            ViewData["IsReadOnly"] = false;
            return View(instructor);
        }
    }
}
