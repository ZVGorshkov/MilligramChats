using System.ComponentModel;
using System.Windows;
using MilligramClient.Common.Wpf.Dispatcher;
using MilligramClient.Common.Wpf.View;

namespace MilligramClient.Wpf.Views.Login.Logic;

public class LoginWindowProvider : ILoginWindowProvider
{
	private readonly IViewService _viewService;
	private readonly IDispatcherHelper _dispatcherHelper;


	private Window _authWindow;

	public LoginWindowProvider(IViewService viewService, IDispatcherHelper dispatcherHelper)
	{
		_viewService = viewService;
		_dispatcherHelper = dispatcherHelper;
	}

	public void Show()
	{
		_dispatcherHelper.CheckBeginInvokeOnUI(() =>
		{
			_authWindow ??= CreateWindow();
			Application.Current.MainWindow = _authWindow;
			_authWindow.Show();
		});
	}

	public void CloseIfCreated()
	{
		_dispatcherHelper.CheckBeginInvokeOnUI(() => _authWindow?.Close());
	}

	private Window CreateWindow()
	{
		var window = _viewService.CreateWindow<LoginViewModel>(WindowMode.Window);
		window.Closing += OnWindowClosing;
		return window;
	}

	private void OnWindowClosing(object sender, CancelEventArgs e)
	{
		_authWindow.Closing -= OnWindowClosing;
		_authWindow = null;
	}
}