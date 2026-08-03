using MilligramServer.Domain.Dtos;
using MilligramServer.Domain.Entities;

namespace MilligramServer.Domain.Extensions;

public static class ContactExtensions
{
    public static ContactDto ToDto(this Contact contact)
    {
        return new ContactDto
        {
            Id = contact.Id,
            Name = contact.Name,
            AddedUserNickname = contact.AddedUser.Nickname
        };
    }
}