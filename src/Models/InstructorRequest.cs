namespace ContosoUniversity.Models
{
    public class InstructorRequest : Instructor
    {
        public string[]? SelectedCourses { get; set; }
    }
}
