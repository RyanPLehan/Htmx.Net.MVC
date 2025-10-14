using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.ViewComponents.Departments
{
    public class DetailViewComponent : ViewComponent
    {
        private readonly SchoolContext _context;

        public DetailViewComponent(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Department department)
        {
            ViewData["IsReadOnly"] = true;
            return View(department);
        }
    }
}
