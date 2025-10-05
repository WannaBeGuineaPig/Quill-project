using System;
using System.Collections.Generic;

namespace Quill_API.Model;

public partial class Rating
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public string Rating1 { get; set; } = null!;

    public virtual Article Article { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
