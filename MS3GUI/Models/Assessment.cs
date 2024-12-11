using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class Assessment
{
    public int Id { get; set; }

    public string? Atype { get; set; }

    public int? TotalMarks { get; set; }

    public int? PassingMarks { get; set; }

    public string? Criteria { get; set; }

    public int? Weightage { get; set; }

    public string? Adescription { get; set; }

    public string? Title { get; set; }

    public int? ModuleId { get; set; }

    public int? CourseId { get; set; }
    public virtual Course Course { get; set; }

    public virtual Module? Module { get; set; }

    public virtual ICollection<TakenAssessment> TakenAssessments { get; set; } = new List<TakenAssessment>();
}
