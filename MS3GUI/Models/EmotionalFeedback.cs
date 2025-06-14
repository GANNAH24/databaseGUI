﻿using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class EmotionalFeedback
{
    public int FeedbackId { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? EmotionalState { get; set; }

    public int? LearnerId { get; set; }

    public virtual ICollection<EmotionalfeedbackReview> EmotionalfeedbackReviews { get; set; } = new List<EmotionalfeedbackReview>();

    public virtual Learner? Learner { get; set; }
}
