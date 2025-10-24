using System;
using System.Collections.Generic;

namespace Quill_API.Model;

public partial class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? PublishedAt { get; set; }

    public string? Status { get; set; }

    public int ArticleId { get; set; }

    public int AuthorId { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual User Author { get; set; } = null!;
}
