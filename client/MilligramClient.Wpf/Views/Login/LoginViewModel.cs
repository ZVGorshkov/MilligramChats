using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MilligramClient.Common.Wpf.Base;
using MilligramClient.Common.Wpf.Messages;
using MilligramClient.Wpf.Messages;
using MilligramClient.Wpf.Views.Login.Controls.Login;
using MilligramClient.Wpf.Views.Login.Controls.Register;
using MilligramClient.Wpf.Views.Main.Logic;

namespace MilligramClient.Wpf.Views.Login;

public class LoginViewModel : ViewModel<LoginWindow>
{
	public override object Header => "Log in";

	private readonly IMessenger _messenger;
	private readonly IMainWindowProvider _mainWindowProvider;

    private LoginState _loginState;

    private ICommand? _contentRenderedCommand;

    public LoginState LoginState
    {
        get => _loginState;
        set => Set(ref _loginState, value);
    }

    public LoginControlViewModel? LoginControlViewModel { get; private set; }
    public RegisterControlViewModel? RegisterControlViewModel { get; private set; }

    public ICommand ContentRenderedCommand => _contentRenderedCommand ??= new RelayCommand(OnContentRendered);

    public LoginViewModel(
        IMessenger messenger,
        IMainWindowProvider mainWindowProvider,
        LoginControlViewModel loginControlViewModel,
        RegisterControlViewModel registerControlViewModel)
    {
        _messenger = messenger;
        _mainWindowProvider = mainWindowProvider;
        LoginControlViewModel = loginControlViewModel;
        RegisterControlViewModel = registerControlViewModel;

        _messenger.Register<CloseLoginWindowMessage>(this, OnCloseLoginWindow);
        _messenger.Register<ChangeLoginWindowMessage>(this, OnChangeLoginWindow);
        _messenger.Register<ExitLoginWindowMessage>(this, OnExitLoginWindow);

        LoginState = LoginState.Login;
    }

    private void OnContentRendered()
    {
        _mainWindowProvider.CloseIfCreated();
    }

    private void OnCloseLoginWindow(CloseLoginWindowMessage message)
    {
        _mainWindowProvider.Show();
    }

    private void OnChangeLoginWindow(ChangeLoginWindowMessage message)
    {
        ChangeLoginWindow(message.LoginState);
    }
    private void OnExitLoginWindow(ExitLoginWindowMessage message)
    {
		_messenger.Send(new RequestCloseMessage(this, null));
    }

    private void ChangeLoginWindow(LoginState loginState)
    {
        switch (loginState)
        {
            case LoginState.Login:
                LoginControlViewModel?.Refresh();
                break;
            case LoginState.Register:
                RegisterControlViewModel?.Refresh();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        LoginState = loginState;
    }

    public override void Cleanup()
    {
        LoginControlViewModel?.Cleanup();
        LoginControlViewModel = null;
        RegisterControlViewModel?.Cleanup();
        RegisterControlViewModel = null;
        _messenger.Unregister(this);
        base.Cleanup();
    }

    public interface IFactory
	{
		LoginViewModel Create();
	}
}