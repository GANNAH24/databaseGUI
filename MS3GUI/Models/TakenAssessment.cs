using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class TakenAssessment
{
    public int LearnerId { get; set; } // Part of composite primary key
    public int AssessmentId { get; set; } // Part of composite primary key
    public int? Score { get; set; }

    // Navigation properties
    public virtual Learner Learner { get; set; } = null!;
    public virtual Assessment Assessment { get; set; } = null!;
}
