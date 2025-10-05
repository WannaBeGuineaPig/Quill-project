using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Quill_API.Model;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Nickname { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Status { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    [JsonIgnore]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [JsonIgnore]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
