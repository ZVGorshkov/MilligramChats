#pragma warning disable CS8618

namespace MilligramServer.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public string Nickname { get; set; }
    public string PasswordHash { get; set; }
    public ICollection<UserRole> UsersRoles { get; set; } = new List<UserRole>();
    public ICollection<Contact> AddedUserForContacts  { get; set; } = new List<Contact>();
    public ICollection<Contact> OwnerUserForContacts { get; set; } = new List<Contact>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<UserChat> UsersChats { get; set; } = new List<UserChat>();
    public ICollection<Chat> ChatsOwner { get; set; } = new List<Chat>();
    public bool IsDeleted { get; set; }
}