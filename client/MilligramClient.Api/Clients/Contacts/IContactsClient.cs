using MilligramClient.Domain.Dtos;

namespace MilligramClient.Api.Clients.Contacts;

public interface IContactsClient
{
    Task<ContactDto[]> GetContactsAsync(CancellationToken cancellationToken = default);
    Task<ContactDto> GetContactAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto[]> SearchUsersAsync(string name, CancellationToken cancellationToken = default);
    Task<ContactDto> CreateContactAsync(ContactDto newContact, CancellationToken cancellationToken = default);
    Task<ContactDto> UpdateContactAsync(Guid id, ContactDto updatedContact, CancellationToken cancellationToken = default);
    Task DeleteContactsAsync(Guid id, CancellationToken cancellationToken = default);
}