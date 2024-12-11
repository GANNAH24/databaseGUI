using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class SkillProgression
{
    public int SkillProgressionId { get; set; }

    public string? ProficiencyLevel { get; set; }

    public DateOnly? Skptimestamp { get; set; }

    public int? LearnerId { get; set; }

    public string? SkillName { get; set; }

    public virtual Skill? Skill { get; set; }
}
