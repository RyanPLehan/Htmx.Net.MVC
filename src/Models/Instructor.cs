using ContosoUniversity.Models.Binders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ContosoUniversity.Models
{
    public class Instructor : Person
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        [BindNever]
        [ValidateNever]
        public ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();

        [ValidateNever]
        public OfficeAssignment? OfficeAssignment { get; set; }
    }
}