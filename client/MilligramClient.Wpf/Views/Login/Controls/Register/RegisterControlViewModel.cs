using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MilligramClient.Api.Clients.Account;
using MilligramClient.Api.Token;
using MilligramClient.Common;
using MilligramClient.Common.Extensions;
using MilligramClient.Common.Wpf.Base;
using MilligramClient.Common.Wpf.Commands;
using MilligramClient.Common.Wpf.MessageBox;
using MilligramClient.Domain.Dtos;
using MilligramClient.Services.Token;
using MilligramClient.Wpf.Messages;

namespace MilligramClient.Wpf.Views.Login.Controls.Register;

public class RegisterControlViewModel : ViewModel<RegisterControl>, IDataErrorInfo
{
    private readonly IMessenger _messenger;
	private readonly ITokenStorage _tokenStorage;
    private readonly IAccountClient _accountClient;
    private readonly ITokenProvider _tokenProvider;
	private readonly IMessageBoxService _messageBoxService;

    private readonly ExecutionTracker _executionTracker;

    private CancellationTokenSource _cancellationTokenSource;

    private bool _isBusy;
    private string _login;
    private string _password;
    private string _confirmPassword;

    private ICommand _loginCommand;
    private ICommand _registerCommand;

    public override object Header => string.Empty;

    public bool IsBusy
    {
        get => _isBusy;
        set => Set(ref _isBusy, value);
    }

    public string Login
    {
        get => _login;
        set => Set(ref _login, value.Trim());
    }

    public string Password
    {
        get => _password;
        set => Set(ref _password, value);
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => Set(ref _confirmPassword, value);
    }

    public ICommand LoginCommand => _loginCommand ??= new RelayCommand(OnLogin);
    public ICommand RegisterCommand => _registerCommand ??= new AsyncRelayCommand(OnRegisterAsync, CanRegister);

    public RegisterControlViewModel(
        IMessenger messenger,
		ITokenStorage tokenStorage,
		IAccountClient accountClient,
        ITokenProvider tokenProvider,
		IMessageBoxService messageBoxService)
    {
        _messenger = messenger;
		_tokenStorage = tokenStorage;
        _accountClient = accountClient;
        _tokenProvider = tokenProvider;
		_messageBoxService = messageBoxService;

        _executionTracker = new ExecutionTracker(() => IsBusy = true, () => IsBusy = false);

        Refresh();
    }

    public void Refresh()
    {
        Login = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
    }

    private void OnLogin()
    {
        ChangeLoginWindow(LoginState.Login);
    }

    private void ChangeLoginWindow(LoginState loginState)
    {
        _messenger.Send(new ChangeLoginWindowMessage(loginState));
    }

    private async Task OnRegisterAsync()
    {
		using (_executionTracker.TrackExecution())
		{
			if (await TryRegisterAsync().ConfigureAwait(false))
				_messenger.Send(new CloseLoginWindowMessage());
		}
    }

	private async Task<bool> TryRegisterAsync()
	{
		try
		{
			var registerDto = new RegisterDto { Login = Login, Password = Password };
			await _accountClient.RegisterAsync(registerDto).ConfigureAwait(false);

			var loginDto = new LoginDto { Login = Login, Password = Password };
			await _tokenProvider.LoginAsync(loginDto).ConfigureAwait(false);
			_tokenStorage.SaveToken(_tokenProvider.GetToken());

			return true;
		}
		catch
		{
			_messageBoxService.Show("Ошибка регистрации", "Ошибка");

			return false;
		}
	}

    private bool CanRegister()
    {
        return Login.IsSignificant() && Password.IsSignificant() && ConfirmPassword.IsSignificant() && !HasError;
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
            error = this[nameof(ConfirmPassword)];
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
                    if (string.IsNullOrWhiteSpace(Login))
                        break;

                    break;
                }

                case nameof(Password):
                {
                    if (string.IsNullOrEmpty(Password))
                        break;

                    if (Password.Length < 5)
                        error = "Пароль должен состоять минимум из 5 символов";
                    else
                        RaisePropertyChanged(nameof(ConfirmPassword));

                    break;
                }

                case nameof(ConfirmPassword):
                {
                    if (string.IsNullOrEmpty(ConfirmPassword))
                        break;

                    if (ConfirmPassword != Password)
                        error = "Пароли не совпадают";

                    break;
                }
            }

            return error;
        }
    }
}