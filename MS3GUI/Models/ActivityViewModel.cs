using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MS3GUI.Models
{
    public class ActivityViewModel
    {
        [Required(ErrorMessage = "Please select a course.")]
        public int? CourseId { get; set; }

        [Required(ErrorMessage = "Please select a module.")]
        public int? ModuleId { get; set; }

        [Required(ErrorMessage = "Please select an activity type.")]
        public string ActivityType { get; set; }

        [Required(ErrorMessage = "Instruction details are required.")]
        public string InstructionDetails { get; set; }

        [Required(ErrorMessage = "Max points are required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Max points must be at least 1.")]
        public int MaxPoints { get; set; }

        public SelectList CoursesList { get; set; }
        public SelectList ModulesList { get; set; }
    }
}