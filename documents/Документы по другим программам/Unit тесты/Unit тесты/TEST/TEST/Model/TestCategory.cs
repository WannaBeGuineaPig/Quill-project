using System;
using System.Collections.Generic;

namespace TEST.Model;

public partial class TestCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
