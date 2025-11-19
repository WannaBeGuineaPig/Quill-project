using System;
using System.Collections.Generic;

namespace TEST.Model;

public partial class UserAnswer
{
    public int Id { get; set; }

    public int TestResultId { get; set; }

    public int QuestionId { get; set; }

    public string? AnswerText { get; set; }

    public bool? IsCorrect { get; set; }

    public int? PointsEarned { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual UserTestResult TestResult { get; set; } = null!;
}
