namespace MilligramClient.Services.Token;

public interface ITokenStorage
{
	string? GetToken();
	void SaveToken(string? token);
}