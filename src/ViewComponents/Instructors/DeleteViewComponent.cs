using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.ViewComponents.Instructors
{
    public class DeleteViewComponent : ViewComponent
    {
        private readonly SchoolContext _context;

        public DeleteViewComponent(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Instructor instructor)
        {
            ViewData["IsReadOnly"] = true;
            return View(instructor);
        }
    }
}
