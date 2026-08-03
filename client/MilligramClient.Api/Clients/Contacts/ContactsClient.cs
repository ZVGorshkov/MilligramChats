using MilligramClient.Api.Extensions;
using MilligramClient.Api.Token;
using MilligramClient.Domain.Dtos;
using RestSharp;

namespace MilligramClient.Api.Clients.Contacts;

public class ContactsClient : HttpClientBase, IContactsClient
{
    private readonly ITokenProvider _tokenProvider;

    public ContactsClient(ITokenProvider tokenProvider, string address, TimeSpan timeout)
        : base(address, timeout)
    {
        _tokenProvider = tokenProvider;
    }

    public Task<ContactDto[]> GetContactsAsync(
        CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<ContactDto[]>(
                Method.Get,
                "api/contacts",
                token,
                cancellationToken));
    }

    public Task<ContactDto> GetContactAsync(Guid id,
                                            CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<ContactDto>(
                Method.Get,
                "api/contacts",
                token,
                cancellationToken));
    }

    public Task<ContactDto> CreateContactAsync(ContactDto newContact,
                                               CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<ContactDto>(
                Method.Post,
                "api/contacts",
                token,
                request => request
                    .AddBody(newContact),
                cancellationToken));
    }

    public Task<ContactDto> UpdateContactAsync(Guid id, ContactDto updatedContact,
                                               CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<ContactDto>(
                Method.Put,
                $"api/contacts/{id}",
                token,
                request => request
                    .AddBody(updatedContact),
                cancellationToken));
    }

    public Task<UserDto[]> SearchUsersAsync(string name,
                                               CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<UserDto[]>(
                Method.Get,
                "api/contacts/search",
                token,
                request => request
                    .AddQueryParameterIfNotNull("name", name),
                cancellationToken));
    }

    public Task DeleteContactsAsync(Guid id,
                                    CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync(
                Method.Delete,
                $"api/contacts/{id}",
                token,
                cancellationToken));
    }
}