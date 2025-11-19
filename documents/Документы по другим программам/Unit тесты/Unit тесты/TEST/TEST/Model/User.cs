using System;
using System.Collections.Generic;

namespace TEST.Model;

public partial class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<FavoriteTest> FavoriteTests { get; set; } = new List<FavoriteTest>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TestCategory> TestCategories { get; set; } = new List<TestCategory>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();

    public virtual ICollection<UserTestResult> UserTestResults { get; set; } = new List<UserTestResult>();
}
