using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class HighestGrade
{
    public int? CourseId { get; set; }

    public int Id { get; set; }

    public string? Title { get; set; }

    public int? HighestMaxPoints { get; set; }
}
