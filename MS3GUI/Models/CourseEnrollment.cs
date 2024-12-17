using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class CourseEnrollment
{
    public int EnrollmentId { get; set; } // Primary Key

    public int? CourseId { get; set; } // Nullable Foreign key to Course
    public int? LearnerId { get; set; } // Nullable Foreign key to Learner

    public DateOnly? CompletionDate { get; set; } // Nullable CompletionDate
    public DateOnly? EnrollmentDate { get; set; } // Nullable EnrollmentDate

    public string? Status { get; set; } // Nullable Status

    // Navigation properties to related entities
    public virtual Course? Course { get; set; }
    public virtual Learner? Learner { get; set; }
}