using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.ViewComponents.Students
{
    public class EditViewComponent : ViewComponent
    {
        private readonly SchoolContext _context;

        public EditViewComponent(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Student student)
        {
            ViewData["IsReadOnly"] = false;
            return View(student);
        }
    }
}
