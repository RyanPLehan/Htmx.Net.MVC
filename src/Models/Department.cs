using ContosoUniversity.Models.Binders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace ContosoUniversity.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [ModelBinder(typeof(CurrencyModelBinder))]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        public int? InstructorID { get; set; }

        public string Version
        {
            get { return Convert.ToBase64String(RowVersion ?? Array.Empty<byte>()); }
            set { RowVersion = Convert.FromBase64String(value ?? string.Empty); }
        }

        [Timestamp]
        [BindNever]
        [ValidateNever]
        internal byte[] RowVersion { get; set; }

        [BindNever]
        [ValidateNever]
        public Instructor Administrator { get; set; }

        [BindNever]
        [ValidateNever]
        public ICollection<Course> Courses { get; set; }
    }
}