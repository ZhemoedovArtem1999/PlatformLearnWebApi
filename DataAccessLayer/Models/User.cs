using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class User
{
    public long Id { get; set; }

    public long RoleId { get; set; }

    public string Email { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string MiddleName { get; set; } = null!;

    public DateOnly DateBirth { get; set; }

    public string Gender { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
