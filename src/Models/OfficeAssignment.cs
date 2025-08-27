using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ContosoUniversity.Models
{
    public class OfficeAssignment
    {
        public int InstructorID { get; set; }

        [StringLength(50)]
        [Display(Name = "Office Location")]
        public string Location { get; set; }

        [BindNever]
        [ValidateNever]
        public Instructor Instructor { get; set; }
    }
}