using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

    public virtual User Author { get; set; } = null!;
    
    [JsonIgnore]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [JsonIgnore]
    public virtual Topic IdTopicsNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
