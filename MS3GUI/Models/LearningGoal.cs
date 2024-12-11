using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class LearningGoal
{
    public int GoalId { get; set; }

    public string? Status { get; set; }

    public DateOnly? Deadline { get; set; }

    public string? Gdescription { get; set; }

    public virtual ICollection<Learner> Learners { get; set; } = new List<Learner>();
}
