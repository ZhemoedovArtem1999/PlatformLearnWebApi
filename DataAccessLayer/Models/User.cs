using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateTime BirthDate { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
