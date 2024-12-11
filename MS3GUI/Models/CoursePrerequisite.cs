using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class CoursePrerequisite
{
    public int CourseId { get; set; }

    public string Prereq { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}
