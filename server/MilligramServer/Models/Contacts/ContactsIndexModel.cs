#pragma warning disable CS8618

using MilligramServer.Models.Users;
using System.ComponentModel;

namespace MilligramServer.Models.Contacts;

public class ContactsIndexModel : SortingPaginationModelBase
{
    [DisplayName("id, логин или имя")]
    public string? SearchString { get; set; }

    [DisplayName("Id")]
    public Guid UserId { get; set; }

    [DisplayName("Владелец")]
    public UserModel User { get; set; }

    [DisplayName("Контакты")]
    public IReadOnlyCollection<ContactModel> Contacts { get; set; }
}