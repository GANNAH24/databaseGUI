namespace MS3GUI.Models
{
    public class EnrollInCourseViewModel
    {
        public int LearnerId { get; set; } // The ID of the learner
        public List<Course> Courses { get; set; } // List of available courses to enroll in
    }
}
