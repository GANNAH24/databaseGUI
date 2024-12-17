namespace MS3GUI.Models
{
    public class ModulesViewModel
    {
        public int CourseId { get; set; } // For redirection
        public string CourseTitle { get; set; } // Display course title
        public List<Module> Modules { get; set; } // List of modules for this course
    }

}
