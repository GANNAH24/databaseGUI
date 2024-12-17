using System;

namespace MS3GUI.Models
{
    public class EnrolledCoursesViewModel
    {
        public int EnrollmentId { get; set; } // Primary key for enrollment
        public int LearnerId { get; set; } // Foreign key to Learner
        public int CourseId { get; set; } // Foreign key to Course
        public string CourseName { get; set; } // Course name for display
        public DateTime? EnrollmentDate { get; set; } // Changed to DateTime to match database type
        public DateTime? CompletionDate { get; set; } // Changed to DateTime to match database type
        public string Status { get; set; } // Course status (e.g., Completed, In Progress)
    }
}
