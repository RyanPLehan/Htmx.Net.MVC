using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.ViewComponents.Departments
{
    public class DeleteViewComponent : ViewComponent
    {
        private readonly SchoolContext _context;

        public DeleteViewComponent(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Department department)
        {
            ViewData["Action"] = "Delete";
            ViewData["PostUrl"] = "/departments/delete";
            return View(department);
        }
    }
}
