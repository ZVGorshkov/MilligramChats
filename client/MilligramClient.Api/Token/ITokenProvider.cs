using MilligramClient.Domain.Dtos;

namespace MilligramClient.Api.Token;

public interface ITokenProvider
{
	Task LoginAsync(LoginDto login);
	Task LoginAsync(string token);
	void Logout();

	string GetToken();
	string GetLoginFromToken();
    Guid GetUserIdFromToken();
    
	Task<TResult> ExecuteWithToken<TResult>(Func<string, Task<TResult>> action);
}