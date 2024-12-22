namespace MS3GUI.Models
{
    public class EnrollInCourseViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Learner> Learners { get; set; }

        public int? CourseId { get; set; }
        public int? LearnerId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Status { get; set; }
    }
}
