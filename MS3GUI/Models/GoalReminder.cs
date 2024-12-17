namespace MS3GUI.Models
{
    public class GoalReminder
    {
        public int GoalReminderId { get; set; } // Unique identifier for each goal reminder
        public int LearnerId { get; set; } // The learner this reminder is associated with
        public string Description { get; set; }
        public string ReminderMessage { get; set; }
        public DateTime Deadline { get; set; }

        public virtual Learner Learner { get; set; } // Navigation property to Learner
    }
}





//namespace MS3GUI.Models
//{
//    public class GoalReminder
//    {
//        public string Description { get; set; }
//        public string ReminderMessage { get; set; }
//        public DateTime Deadline { get; set; }
//    }
//}
