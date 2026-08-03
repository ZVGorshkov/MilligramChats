using Microsoft.Win32;
using MilligramClient.Common;
using MilligramClient.Common.Extensions;

namespace MilligramClient.Services.Token;

public class TokenStorage : ITokenStorage
{
	private const string RegistryTokenKey = "Token";

	public string? GetToken()
	{
		try
		{
			using var key = Registry.CurrentUser.OpenSubKey(Constants.RegistrySettingsKey);
			return key?.GetValue(RegistryTokenKey)?.ToString()?.NullIfEmpty();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка чтения: {ex.Message}");
			return null;
		}
	}

	public void SaveToken(string? token)
	{
		try
		{
			using var key = Registry.CurrentUser.CreateSubKey(Constants.RegistrySettingsKey);

			if (token.IsSignificant())
				key.SetValue(RegistryTokenKey, token, RegistryValueKind.String);
			else
				key.DeleteValue(RegistryTokenKey, false);
		}
		catch
		{
			// ignore
		}
	}
}