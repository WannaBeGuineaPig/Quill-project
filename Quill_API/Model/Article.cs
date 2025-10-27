using System;
using System.Collections.Generic;

namespace Quill_API.Model;

public partial class Article
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? PublishedAt { get; set; }

    public string? Status { get; set; }

    public int AuthorId { get; set; }

    public int IdTopics { get; set; }
    public byte[]? Image { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Topic IdTopicsNavigation { get; set; } = null!;

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public int LikesAmount => Ratings.Count() != 0 ? Ratings.Count(x => x.Rating1 == 1) : 0;

    public int DislikesAmount => Ratings.Count() != 0 ? Ratings.Count(x => x.Rating1 == -1) : 0;
}
