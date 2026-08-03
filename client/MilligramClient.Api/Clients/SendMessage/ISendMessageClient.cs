namespace MilligramClient.Api.Clients.SendMessage;

public interface ISendMessageClient
{
    Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default);
    Task<string> GetChatMessagesAsync(Guid idChat, CancellationToken cancellationToken = default);
    Task<string> DeleteMessagesAsync(string message, CancellationToken cancellationToken = default);
}