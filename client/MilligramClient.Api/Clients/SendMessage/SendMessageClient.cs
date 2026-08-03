using MilligramClient.Api.Token;
using RestSharp;

namespace MilligramClient.Api.Clients.SendMessage;

public class SendMessageClient : HttpClientBase, ISendMessageClient
{
	private readonly ITokenProvider _tokenProvider;

	public SendMessageClient(ITokenProvider tokenProvider, string address, TimeSpan timeout)
		: base(address, timeout)
	{
		_tokenProvider = tokenProvider;
	}

	public Task<string> SendMessageAsync(
		string message,
		CancellationToken cancellationToken = default)
	{
		return _tokenProvider.ExecuteWithToken(token =>
			SendRequestAsync<string>(
				Method.Post,
				"api/sendMessages",
				token,
				cancellationToken));
	}

	public Task<string> GetChatMessagesAsync(
		Guid idChat,
		CancellationToken cancellationToken = default)
	{
		return _tokenProvider.ExecuteWithToken(token =>
			SendRequestAsync<string>(
				Method.Get,
				"api/getMessages",
				token,
				cancellationToken));
	}

	public Task<string> DeleteMessagesAsync(
		string message,
		CancellationToken cancellationToken = default)
	{
		return _tokenProvider.ExecuteWithToken(token =>
			SendRequestAsync<string>(
				Method.Get,
				"api/deleteMessage",
				token,
				cancellationToken));
	}
}