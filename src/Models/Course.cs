using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace ContosoUniversity.Models
{
    public class Course
    {
        [Display(Name = "Number")]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Range(0, 5)]
        public int Credits { get; set; }

        public int DepartmentID { get; set; }

        [BindNever]
        [ValidateNever]
        public Department Department { get; set; }

        [BindNever]
        [ValidateNever]
        public ICollection<Enrollment> Enrollments { get; set; }

        [BindNever]
        [ValidateNever]
        public ICollection<CourseAssignment> CourseAssignments { get; set; }
    }
}