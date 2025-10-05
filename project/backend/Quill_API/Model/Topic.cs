using System;
using System.Collections.Generic;

namespace Quill_API.Model;

public partial class Topic
{
    public int IdTopics { get; set; }

    public string NameTopics { get; set; } = null!;

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
