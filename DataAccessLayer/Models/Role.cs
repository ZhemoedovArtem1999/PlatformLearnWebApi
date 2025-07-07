namespace DataAccessLayer.Models;

public partial class Role
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual User? User { get; set; }
}
