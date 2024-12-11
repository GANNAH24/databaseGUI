using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class Achievement
{
    public int AchievementId { get; set; }

    public string? Adescription { get; set; }

    public DateOnly? DateEarned { get; set; }

    public string? Atype { get; set; }

    public int? LearnerId { get; set; }

    public int? BadgeId { get; set; }

    public virtual Badge? Badge { get; set; }

    public virtual Learner? Learner { get; set; }
}
