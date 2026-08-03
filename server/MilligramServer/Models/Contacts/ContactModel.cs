#pragma warning disable CS8618

using System.ComponentModel;

namespace MilligramServer.Models.Contacts;

public class ContactModel
{
    [DisplayName("Id")]
    public Guid Id { get; set; }

    [DisplayName("Id владельца")]
    public Guid OwnerUserId { get; set; }

    [DisplayName("Логин владельца")]
    public string OwnerUserName { get; set; }

    [DisplayName("Id контакта")]
    public Guid AddedUserId { get; set; }

    [DisplayName("Логин контакта")]
    public string AddedUserName { get; set; }

    [DisplayName("Имя")]
    public string? Name { get; set; }

    [DisplayName("Статус")]
    public bool IsDeleted { get; set; }
}