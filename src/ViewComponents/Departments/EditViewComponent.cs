using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.ViewComponents.Departments
{
    public class EditViewComponent : ViewComponent
    {
        private readonly SchoolContext _context;

        public EditViewComponent(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Department department)
        {
            ViewData["IsReadOnly"] = false;
            return View(department);
        }
    }
}
