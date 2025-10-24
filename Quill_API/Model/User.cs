using System;
using System.Collections.Generic;

namespace Quill_API.Model;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Nickname { get; set; } = null!;

    public string? Role { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
