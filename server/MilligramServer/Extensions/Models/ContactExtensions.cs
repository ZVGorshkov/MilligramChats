using MilligramServer.Domain.Entities;
using MilligramServer.Models.Contacts;

namespace MilligramServer.Extensions.Models;

public static class ContactExtensions
{
    public static IEnumerable<ContactModel> ToModels(
        this IEnumerable<Contact> contacts)
    {
        foreach (var contact in contacts)
            yield return contact.ToModel();
    }

    public static ContactModel ToModel(this Contact contact)
    {
        return new ContactModel
        {
            Id = contact.Id,         
            OwnerUserId = contact.OwnerUserId,
            OwnerUserName = contact.OwnerUser?.Name!,
            AddedUserId = contact.AddedUserId,
            AddedUserName = contact.AddedUser?.Name!,
            Name = contact.Name,
            IsDeleted = contact.IsDeleted
        };
    }
}