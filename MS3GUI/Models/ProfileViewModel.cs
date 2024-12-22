namespace MS3GUI.Models
{
    public class ProfileViewModel
    {

        // Common attributes
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? Country { get; set; }
        public string? CulturalBackground { get; set; }
        public string? Email { get; set; }

        // Learner-specific attributes
        public string? LatestQualification { get; set; }
        public string? ExpertiseArea { get; set; }

        // Instructor-specific attributes
        public string? InstructorName { get; set; }

        // Admin-specific attributes
        public string? AdminName { get; set; }
        // New property for LearnerId
        public int LearnerId { get; set; }
        public int InstructorId { get; set; }

        public int AdminId { get; set; }
    }
}
