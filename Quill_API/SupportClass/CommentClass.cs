using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Quill_API.SupportClass;

public partial class CommentClass
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? PublishedAt { get; set; }

    public string? Status { get; set; }

    public int ArticleId { get; set; }

    public int AuthorId { get; set; }
}
