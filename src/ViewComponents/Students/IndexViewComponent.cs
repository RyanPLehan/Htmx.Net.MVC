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

        public IndexViewComponent(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string columnName,
                                                            string sortOrder,
                                                            string filter,
                                                            string searchString,
                                                            int? pageNumber)
        {
            ViewData["ColumnName"] = columnName;
            ViewData["SortOrder"] = sortOrder;

            // Shell should be loaded only once
            if (String.IsNullOrWhiteSpace(sortOrder) &&
                String.IsNullOrWhiteSpace(filter) &&
                String.IsNullOrWhiteSpace(searchString) &&
                pageNumber.GetValueOrDefault() == 0)
            {
                return View("Master"); 
            }


            if (!String.IsNullOrWhiteSpace(searchString))
            {
                pageNumber = 1;
            }
            else
            {
                searchString = filter;
            }

            ViewData["Filter"] = searchString;

            IQueryable<Student> query = _context.Students;
            query = ApplyFilter(query, searchString);
            query = ApplySortOrder(query, columnName, sortOrder);

            int pageSize = 3;
            PaginatedList<Student> students = await PaginatedList<Student>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize);

            return View("Details", students);
        }

        private IQueryable<Student> ApplyFilter(IQueryable<Student> query, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
            {
                // EF Sqlite Client will generate INSTR() which is case-sensitive
                //query = query.Where(q => q.LastName.Contains(searchString) ||
                //                         q.FirstName.Contains(searchString));

                // EF Sqlite Client will generate LIKE which is case-insensitive
                string likePattern = $"%{searchString}%";
                query = query.Where(q => EF.Functions.Like(q.LastName, likePattern) ||
                                         EF.Functions.Like(q.FirstName, likePattern));
            }
            return query;
        }

        private IQueryable<Student> ApplySortOrder(IQueryable<Student> query, string columnName, string sortOrder)
        {
            bool sortAscending = (String.IsNullOrWhiteSpace(sortOrder) ||
                                  sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase) 
                                  ? true : false);

            string column = (String.IsNullOrWhiteSpace(columnName) ? String.Empty : columnName.ToLower());

            switch (column)
            {
                case "lastname":
                    query = (sortAscending ? query.OrderBy(q => q.LastName) : query.OrderByDescending(q => q.LastName));
                    break;
                case "enrollmentdate":
                    query = (sortAscending ? query.OrderBy(q => q.EnrollmentDate) : query.OrderByDescending(q => q.EnrollmentDate));
                    break;
                default:
                    query = query.OrderBy(q => q.LastName);
                    break;
            }

            return query;
        }
    }
}
