using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class Survey
{
    public int SurveyId { get; set; }

    public string? Title { get; set; }

    public virtual ICollection<SurveyQuestion> SurveyQuestions { get; set; } = new List<SurveyQuestion>();
}
