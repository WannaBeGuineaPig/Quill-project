using System;
using System.Collections.Generic;
using TEST.Model;

namespace TEST;

public partial class Question
{
    public int Id { get; set; }

    public int TestId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string QuestionType { get; set; } = null!;

    public int Points { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();

    public virtual Test Test { get; set; } = null!;

    public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
