using MilligramClient.Domain.Dtos;
using MilligramClient.Domain.Model;

namespace MilligramClient.Domain.Extensions;

public static class ContactExtensions
{
    public static ContactDto ToDto (this ContactModel contact)
    {
        return new ContactDto
        {
            Name = contact.Name,
            
        };
    }
}
