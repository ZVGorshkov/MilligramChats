using System.Windows;
using MilligramClient.Api.Token;
using MilligramClient.Common.Extensions;
using MilligramClient.Services.Token;
using MilligramClient.Wpf.Views.Login.Logic;
using MilligramClient.Wpf.Views.Main.Logic;


namespace MilligramClient.Wpf;

public partial class App
{
	private readonly ITokenStorage _tokenStorage;
	private readonly ITokenProvider _tokenProvider;
	private readonly IMainWindowProvider _mainWindowProvider;
	private readonly ILoginWindowProvider _loginWindowProvider;

	public App(
		ITokenStorage tokenStorage,
		ITokenProvider tokenProvider,
		IMainWindowProvider mainWindowProvider,
		ILoginWindowProvider loginWindowProvider)
	{
		_tokenStorage = tokenStorage;
		_tokenProvider = tokenProvider;
		_mainWindowProvider = mainWindowProvider;
		_loginWindowProvider = loginWindowProvider;

		InitializeComponent();
	}

	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		ShowStartupWindow().FireAndForgetSafeAsync();
	}

	private async Task ShowStartupWindow()
	{
		if (await TryRestoreTokenAsync().ConfigureAwait(false))
			_mainWindowProvider.Show();
		else
			_loginWindowProvider.Show();
	}

	private async Task<bool> TryRestoreTokenAsync()
	{
		var token = _tokenStorage.GetToken();
		if (token.IsNullOrEmpty())
			return false;

		try
		{
			await _tokenProvider.LoginAsync(token).ConfigureAwait(false);
		}
		catch
		{
			return false;
		}

		return true;
	}
}