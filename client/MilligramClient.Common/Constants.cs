namespace MilligramClient.Common;

public static class Constants
{
	public const string ServerAddress = "http://92.55.7.199:8181";
	public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(30);
	public const int RefreshSlidingTokenBeforeExpirationInPercent = 50;
	public const string RegistryApplicationName = "MilligramClientAuthorizeExample";
	public const string RegistrySettingsKey = $@"SOFTWARE\{RegistryApplicationName}";
}