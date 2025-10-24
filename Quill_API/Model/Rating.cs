using System;
using System.Collections.Generic;

namespace Quill_API.Model;

public partial class Rating
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public short Rating1 { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
