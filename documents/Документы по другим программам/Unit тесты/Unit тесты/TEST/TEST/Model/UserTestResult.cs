using System;
using System.Collections.Generic;

namespace TEST.Model;

public partial class UserTestResult
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TestId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public int? ScoreAchieved { get; set; }

    public int MaxScore { get; set; }

    public bool? IsPassed { get; set; }

    public virtual Test Test { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
