using System;
using System.Collections.Generic;

namespace Quill_API.SupportClass;

public partial class RatingClass
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public string Rating1 { get; set; } = null!;
}
