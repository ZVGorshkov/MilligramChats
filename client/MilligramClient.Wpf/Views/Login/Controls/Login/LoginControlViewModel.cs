using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MilligramClient.Api.Exceptions;
using MilligramClient.Api.Token;
using MilligramClient.Common;
using MilligramClient.Common.Extensions;
using MilligramClient.Common.Wpf.Base;
using MilligramClient.Common.Wpf.Commands;
using MilligramClient.Common.Wpf.MessageBox;
using MilligramClient.Domain.Dtos;
using MilligramClient.Services.Token;
using MilligramClient.Wpf.Messages;

namespace MilligramClient.Wpf.Views.Login.Controls.Login;

public class LoginControlViewModel : ViewModel<LoginControl>, IDataErrorInfo
{
    private readonly IMessenger _messenger;
    private readonly ITokenProvider _tokenProvider;
    private readonly IMessageBoxService _messageBoxService;

    private readonly ExecutionTracker _executionTracker;

    private bool _isBusy;
    private string _login;
    private string _password;
	private bool _isRememberMe;

    private ICommand _cleanLoginCommand;
    private ICommand _cleanPasswordCommand;
    private ICommand _loginCommand;
    private ICommand _registerCommand;
	private ICommand _onExitCommand;

    public override object Header => string.Empty;

    public bool IsBusy
    {
        get => _isBusy;
        set => Set(ref _isBusy, value);
    }

    public string Login
    {
        get => _login;
        set => Set(ref _login, value);
    }

    public string Password
    {
        get => _password;
        set => Set(ref _password, value);
    }

	public bool IsRememberMe
	{
		get => _isRememberMe;
		set => Set(ref _isRememberMe, value);
	}

    public ICommand CleanLoginCommand => _cleanLoginCommand ??= new RelayCommand(OnCleanLogin);
    public ICommand CleanPasswordCommand => _cleanPasswordCommand ??= new RelayCommand(OnCleanPassword);
    public ICommand LoginCommand => _loginCommand ??= new AsyncRelayCommand(OnLoginAsync, CanLogin);
    public ICommand RegisterCommand => _registerCommand ??= new RelayCommand(OnRegister);
	public ICommand OnExitCommand => _onExitCommand ??= new RelayCommand(OnExit);

    public LoginControlViewModel(
        IMessenger messenger,
		ITokenStorage tokenStorage, 
		ITokenProvider tokenProvider,
        IMessageBoxService messageBoxService)
    {
        _messenger = messenger;
        _tokenProvider = tokenProvider;
        _messageBoxService = messageBoxService;

        _executionTracker = new ExecutionTracker(() => IsBusy = true, () => IsBusy = false);

        Refresh();
    }

    public void Refresh()
    {
        Login = string.Empty;
        Password = string.Empty;
    }

    private void OnCleanLogin()
    {
        Login = string.Empty;
    }
    private void OnCleanPassword()
    {
        Password = string.Empty;
    }

    private async Task OnLoginAsync()
    {
        using (_executionTracker.TrackExecution())
        {
            await LoginAsync().ConfigureAwait(false);

            _messenger.Send(new CloseLoginWindowMessage());
        }
    }

    private async Task LoginAsync()
    {
        try
        {
            var loginDto = new LoginDto { Login = Login, Password = Password };
            await _tokenProvider.LoginAsync(loginDto).ConfigureAwait(false);
        }
        catch (SendRequestException exception) when (exception.StatusCode == HttpStatusCode.Unauthorized)
        {
            _messageBoxService.Show("Некорректные логин и(или) пароль", "Ошибка");
        }
    }

    private bool CanLogin()
    {
        return Login.IsSignificant() && Password.IsSignificant() && !HasError;
    }

    private void OnRegister()
    {
        ChangeLoginWindow(LoginState.Register);
    }

	private void OnExit()
	{
		_messenger.Send(new ExitLoginWindowMessage());
    }

    private void ChangeLoginWindow(LoginState loginState)
    {
        _messenger.Send(new ChangeLoginWindowMessage(loginState));
    }

    public string Error => string.Empty;

    private ObservableCollection<string> _errors;

    public virtual ObservableCollection<string> Errors
    {
        get
        {
            _errors = new ObservableCollection<string>();
            var error = this[nameof(Login)];
            if (!string.IsNullOrEmpty(error))
                _errors.Add(error);
            error = this[nameof(Password)];
            if (!string.IsNullOrEmpty(error))
                _errors.Add(error);
            return _errors;
        }
    }

    public virtual bool HasError => Errors?.Count > 0;

    public string this[string columnName]
    {
        get
        {
            var error = string.Empty;
            switch (columnName)
            {
                case nameof(Login):
                {
                    if (string.IsNullOrEmpty(Login))
                        break;

                    break;
                }

                case nameof(Password):
                {
                    if (string.IsNullOrEmpty(Password))
                        break;

                    if (Password.Length < 8)
                        error = "Пароль должен состоять минимум из 8 символов";

                    break;
                }
            }

            return error;
        }
    }
}