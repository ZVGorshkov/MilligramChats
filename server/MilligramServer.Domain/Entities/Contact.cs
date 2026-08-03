#pragma warning disable CS8618

namespace MilligramServer.Domain.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Guid OwnerUserId { get; set; }
    public User OwnerUser { get; set; }
    public Guid AddedUserId { get; set; }
    public User AddedUser { get; set; }
    public bool IsDeleted { get; set; }
}