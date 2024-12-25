using System.ComponentModel.DataAnnotations.Schema;

namespace MS3GUI.Models
{
    public class ReceivedNotification
    {
        public int NotificationId { get; set; }
        public int LearnerId { get; set; }

        [ForeignKey("NotificationId")]
        public virtual Notification Notification { get; set; }

        [ForeignKey("LearnerId")]
        public virtual Learner Learner { get; set; }
    }
}
