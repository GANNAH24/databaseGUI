using System;
using System.ComponentModel.DataAnnotations;

namespace MS3GUI.Models
{
    public class EmotionalFeedbackViewModel
    {
        public int ActivityId { get; set; }
        public int LearnerId { get; set; }
        public DateTime Timestamp { get; set; }
        public string EmotionalState { get; set; }
    }
}

