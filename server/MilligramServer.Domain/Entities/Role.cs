#pragma warning disable CS8618

namespace MilligramServer.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public ICollection<UserRole> UsersRoles { get; set; } = new List<UserRole>();
    public bool IsDeleted { get; set; }
}