using System;
using System.Collections.Generic;
using TEST.Model;

namespace TEST;

public partial class Test
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public int PassingScore { get; set; }

    public int? TimeLimitMinutes { get; set; }

    public virtual TestCategory Category { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<FavoriteTest> FavoriteTests { get; set; } = new List<FavoriteTest>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();

    public virtual ICollection<UserTestResult> UserTestResults { get; set; } = new List<UserTestResult>();
}
