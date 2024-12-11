using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class TakenAssessment
{
    public int? Score { get; set; }

    public int LearnerId { get; set; }

    public int AssessmentId { get; set; }

    public virtual Assessment Assessment { get; set; } = null!;

    public virtual Learner Learner { get; set; } = null!;
}
