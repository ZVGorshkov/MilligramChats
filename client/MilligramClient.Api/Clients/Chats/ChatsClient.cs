using MilligramClient.Api.Extensions;
using MilligramClient.Api.Token;
using MilligramClient.Domain.Dtos;

using RestSharp;

namespace MilligramClient.Api.Clients.Chats;

public class ChatsClient : HttpClientBase, IChatsClient
{
    private readonly ITokenProvider _tokenProvider;

    public ChatsClient(ITokenProvider tokenProvider, string address, TimeSpan timeout)
        : base(address, timeout)
    {
        _tokenProvider = tokenProvider;
    }

    public Task<ChatDto[]> GetChatsAsync(
        CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<ChatDto[]>(
                Method.Get,
                "api/chats",
                token,
                cancellationToken));
    }

    public Task<ChatDto> GetChatAsync(Guid id,
                                      CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<ChatDto>(
                Method.Get,
                $"api/chats/{id}",
                token,
                cancellationToken));
    }

    public Task<ChatDto> CreateChatAsync(ChatDto newChat,
                                         CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<ChatDto>(
                Method.Post,
                "api/chats",
                token,
                request => request.AddBody(newChat),
                cancellationToken));
    }

    public Task<ChatDto> UpdateChatAsync(Guid id,
                                         ChatDto updatedChat,
                                         CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<ChatDto>(
                Method.Put,
                $"api/chats/{id}",
                token,
                request => request
                    .AddBody(updatedChat),
                cancellationToken));
    }

    public Task<ChatDto[]> SearchChatsAsync(string name,
                                           CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<ChatDto[]>(
                Method.Get,
                "api/chats/search",
                token,
                request => request
                    .AddQueryParameterIfNotNull("name", name),
                cancellationToken));
    }

    public Task DeleteChatAsync(Guid id,
                                CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync(
                Method.Delete,
                $"api/chats/{id}",
                token,
                cancellationToken));
    }

    public Task<UserDto[]> GetUsersAsync(Guid chatId,
                                         CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<UserDto[]>(
                Method.Get,
                $"api/chats/{chatId}/users",
                token,
                cancellationToken));
    }

    public Task<ChatDto> AddUserAsync(Guid chatId,
                                      Guid userId,
                                      CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<ChatDto>(
               Method.Post,
                $"api/chats/{chatId}/users/{userId}",
                token,
                cancellationToken));
    }

    public Task<ChatDto> DeleteUserAsync(Guid chatId,
                                         Guid userId,
                                         CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<ChatDto>(
                Method.Delete,
                $"api/chats/{chatId}/users/{userId}",
                token,
                cancellationToken));
    }

    public Task<MessageDto[]> GetMessagesAsync(Guid chatId,
                                               CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<MessageDto[]>(
                Method.Get,
                $"api/chats/{chatId}/messages",
                token,
                cancellationToken));
    }
    public Task<MessageDto> GetMessageAsync(Guid chatId,
                                            Guid messageId,
                                            CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<MessageDto>(
                Method.Get,
                $"api/chats/{chatId}/messages/{messageId}",
                token,
                cancellationToken));
    }

    public Task<MessageDto> AddMessageAsync(Guid chatId,
                                            MessageDto messageDto,
                                            CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<MessageDto>(
                Method.Post,
                $"api/chats/{chatId}/messages",
                token,
                request => request
                    .AddBody(messageDto),
                cancellationToken));
    }

    public Task<MessageDto> UpdateMessageAsync(Guid chatId,
                                               Guid messageId,
                                               MessageDto messageDto,
                                               CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync<MessageDto>(
                Method.Put,
                $"api/chats/{chatId}/messages/{messageId}",
                token,
                request => request
                    .AddBody(messageDto),
                cancellationToken));
    }

    public Task DeleteMessageAsync(Guid chatId,
                                   Guid messageId,
                                   CancellationToken cancellationToken = default)
    {
        return _tokenProvider.ExecuteWithToken(token =>
            SendRequestAsync(
                Method.Delete,
                $"api/chats/{chatId}/messages/{messageId}",
                token,
                cancellationToken));
    }

}