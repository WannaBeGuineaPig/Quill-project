using System;
using System.Collections.Generic;
using TEST.Model;

namespace TEST;

public partial class FavoriteTest
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TestId { get; set; }

    public virtual Test Test { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
