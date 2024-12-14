using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class LearningActivity
{
    public int ActivityId { get; set; }

    public string ActivityType { get; set; } = null!; // Made required (non-nullable) 

    public string InstructionDetails { get; set; } = null!; // Made required (non-nullable) 

    public int MaxPoints { get; set; }

    public int ModuleId { get; set; }

    public int CourseId { get; set; }

    public virtual ICollection<InteractionLog> InteractionLogs { get; set; } = new List<InteractionLog>();

    public virtual Module Module { get; set; } = null!;  // Linked to Module (required)
}
