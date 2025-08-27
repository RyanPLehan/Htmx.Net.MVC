using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ContosoUniversity.ViewComponents.Students
{
    /// <summary>
    /// Display Privacy statement
    /// </summary>
    public class IndexViewComponent : ViewComponent
    {
        private readonly SchoolContext _context;

        public class ViewModel
        {
            public string CurrentSortOrder { get; set; }
            public string CurrentFilter { get; set; }
            public PaginatedList<Student> Students { get; set; }
        }

        public IndexViewComponent(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string sortOrder,
                                                            string currentFilter,
                                                            string searchString,
                                                            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (!String.IsNullOrWhiteSpace(searchString))
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            IQueryable<Student> query = _context.Students;
            query = ApplyFilter(query, searchString);
            query = ApplySortOrder(query, sortOrder);

            int pageSize = 3;
            PaginatedList<Student> students = await PaginatedList<Student>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

            ViewModel model = new ViewModel()
            {
                CurrentSortOrder = sortOrder,
                CurrentFilter = searchString,
                Students = students
            };

            return View(students);
        }

        private IQueryable<Student> ApplyFilter(IQueryable<Student> query, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(q => q.LastName.Contains(searchString) ||
                                         q.FirstName.Contains(searchString));
            }
            return query;
        }

        private IQueryable<Student> ApplySortOrder(IQueryable<Student> query, string sortOrder)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(q => q.LastName);
                    break;
                case "Date":
                    query = query.OrderBy(q => q.EnrollmentDate);
                    break;
                case "date_desc":
                    query = query.OrderByDescending(q => q.EnrollmentDate);
                    break;
                default:
                    query = query.OrderBy(q => q.LastName);
                    break;
            }

            return query;
        }
    }
}
