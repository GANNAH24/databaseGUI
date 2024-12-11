using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public DateOnly? Ntimestamp { get; set; }

    public string? Nmessage { get; set; }

    public string? UrgencyLevel { get; set; }

    public bool? ReadStatus { get; set; }

    public virtual ICollection<Learner> Learners { get; set; } = new List<Learner>();
}
