using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class Skill
{
    public int LearnerId { get; set; }

    public string SkillName { get; set; } = null!;

    public virtual Learner Learner { get; set; } = null!;

    public virtual ICollection<SkillProgression> SkillProgressions { get; set; } = new List<SkillProgression>();
}
