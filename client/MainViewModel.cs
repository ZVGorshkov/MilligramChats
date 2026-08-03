using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MilligramClient.Api.Clients.Chats;
using MilligramClient.Api.Clients.Contacts;
using MilligramClient.Api.Clients.SendMessage;
using MilligramClient.Api.Token;
using MilligramClient.Common.Extensions;
using MilligramClient.Common.Wpf.Base;
using MilligramClient.Common.Wpf.Commands;
using MilligramClient.Common.Wpf.Dispatcher;
using MilligramClient.Common.Wpf.MessageBox;
using MilligramClient.Common.Wpf.Messages;
using MilligramClient.Domain.Dtos;
using MilligramClient.Domain.Extensions;
using MilligramClient.Domain.Model;
using MilligramClient.Services.Token;
using MilligramClient.Wpf.Views.Login.Logic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MilligramClient.Wpf.Views.Main;

public class MainViewModel : ViewModel<MainWindow>, INotifyPropertyChanged
{
	private string _login;
	private string _newMessageText;
	private string _statusMessage;
	private HamburgerMenuItem _selectedMenuItem;
	private ChatDto _selectedChat;
	private ContactDto _selectedContact;
    private UserDto _selectedFoundUser;
    private string _contactSearchName;
    private bool _isContactSearchVisible;
    private bool _isContactSearchInProgress;
    private MainWindowState _mainWindowState = MainWindowState.Chats;
    private CancellationTokenSource _requestMessagesCts;
	private DateTime _lastMessageRequestTime = DateTime.MinValue;


	private ICommand _contentRenderedCommand;
	private ICommand _logoutCommand;
	private ICommand _exitCommand;
	private ICommand _menuCommand;
	private ICommand _sendMessageCommand;
	private ICommand _attachFileCommand;
	private ICommand _getContactsCommand;
    private ICommand _findContactsCommand;
    private ICommand _addContactCommand;
    private ICommand _deleteContactCommand;
    private ICommand _closeContactSearchCommand;

    public override object Header => "Milligram";

	private readonly IMessenger _messenger;
	private readonly ITokenStorage _tokenStorage;
	private readonly ITokenProvider _tokenProvider;
	private readonly IMessageBoxService _messageBoxService;
	private readonly ILoginWindowProvider _loginWindowProvider;
	private readonly IChatsClient _chatsClient;
	private readonly IContactsClient _contactsClient;
	private readonly ISendMessageClient _sendMessageClient;
	private readonly IDispatcherHelper _dispatcherHelper;

	public HamburgerMenuItems Menu { get; } = new HamburgerMenuItems();
	public ObservableCollection<HamburgerMenuItem> OptionsItems { get; }
	public ObservableCollection<MessageModel> Messages { get; set; } = new ObservableCollection<MessageModel>();
	public ObservableCollection<ChatDto> Chats { get; set; } = new ObservableCollection<ChatDto>();
	public ObservableCollection<ContactDto> Contacts { get; set; } = new ObservableCollection<ContactDto>();
    public ObservableCollection<UserDto> FoundUsers { get; } = new ObservableCollection<UserDto>();

    public MainWindowState MainWindowState
	{
		get => _mainWindowState;
		set => Set(ref _mainWindowState, value);
	}

	public string Login
	{
		get => _login;
		set => Set(ref _login, value);
	}

	public string NewMessageText
	{
		get => _newMessageText;
		set => Set(ref _newMessageText, value);
	}

	public string StatusMessage
	{
		get => _statusMessage;
		set => Set(ref _statusMessage, value);
	}

	public HamburgerMenuItem SelectedMenuItem
	{
		get => _selectedMenuItem;
		set
		{
			Set(ref _selectedMenuItem, value);
			if (value != null)
			{
				OnMenuSelected(value.Tag.ToString());
			}
		}
	}

	public ChatDto SelectedChat
	{
		get => _selectedChat;
		set
		{
			Set(ref _selectedChat, value);
			OnChatSelected();
		}
	}

	public ContactDto SelectedContact
	{
		get => _selectedContact;
		set => Set(ref _selectedContact, value);
	}
    public UserDto SelectedFoundUser
    {
        get => _selectedFoundUser;
        set => Set(ref _selectedFoundUser, value);
    }

    public string ContactSearchName
    {
        get => _contactSearchName;
        set => Set(ref _contactSearchName, value);
    }

    public bool IsContactSearchVisible
    {
        get => _isContactSearchVisible;
        set => Set(ref _isContactSearchVisible, value);
    }

    public bool IsContactSearchInProgress
    {
        get => _isContactSearchInProgress;
        set => Set(ref _isContactSearchInProgress, value);
    }

    // Команды
    //public ICommand GetContactsCommand => _getContactsCommand ??= new RelayCommand(GetContacts);
    public ICommand SendMessageCommand => _sendMessageCommand ??= new RelayCommand(SendMessage);
	public ICommand AttachFileCommand => _attachFileCommand ??= new RelayCommand(AttachFile);
	public ICommand ContentRenderedCommand => _contentRenderedCommand ??= new RelayCommand(OnContentRendered);
	public ICommand MenuCommand => _menuCommand ??= new RelayCommand<string>(OnMenuSelected);
	public ICommand LogoutCommand => _logoutCommand ??= new RelayCommand(OnLogout);
	public ICommand ExitCommand => _exitCommand ??= new RelayCommand(OnExit);
	public ICommand SelectChatCommand => new RelayCommand(OnChatSelected);
    public ICommand FindContactsCommand => _findContactsCommand ??= new AsyncRelayCommand(SearchUsersAsync, CanSearchUsers);
    public ICommand AddContactCommand => _addContactCommand ??= new AsyncRelayCommand<UserDto>(AddContactAsync);
    public ICommand DeleteContactCommand => _deleteContactCommand ??= new AsyncRelayCommand(DeleteSelectedContactAsync);
    public ICommand CloseContactSearchCommand => _closeContactSearchCommand ??= new RelayCommand(CloseContactSearch);

    public MainViewModel(
		IMessenger messenger,
		ITokenStorage tokenStorage,
		ITokenProvider tokenProvider,
		IMessageBoxService messageBoxService,
		ILoginWindowProvider loginWindowProvider,
		IChatsClient chatsClient,
		IContactsClient contactsClient,
		ISendMessageClient sendMessageClient,
		IDispatcherHelper dispatcherHelper)
	{
		_messenger = messenger;
		_tokenStorage = tokenStorage;
		_tokenProvider = tokenProvider;
		_messageBoxService = messageBoxService;
		_loginWindowProvider = loginWindowProvider;
		_chatsClient = chatsClient;
		_contactsClient = contactsClient;
		_sendMessageClient = sendMessageClient;
		_dispatcherHelper = dispatcherHelper;

		// Пример сообщений (в реальном приложении будет загрузка из сервера)
		Messages.Add(new MessageModel
		{
			Sender = "Система",
			Text = "Добро пожаловать в Milligram!",
			Timestamp = DateTime.Now
		});
	}

	private void StartRequestMessages()
	{
		_requestMessagesCts?.Cancel();
		_requestMessagesCts = new CancellationTokenSource();

		_lastMessageRequestTime = DateTime.MinValue;

		_ = RequestMessagesAsync(_requestMessagesCts.Token);
	}

	private async Task RequestMessagesAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				if (SelectedChat == null)
					return;
				var newMessages = await _chatsClient.GetMessagesAsync(SelectedChat.Id, cancellationToken).ConfigureAwait(false);

				_dispatcherHelper.CheckBeginInvokeOnUI(() => { UpdateMessages(newMessages); });

				await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
			}

			catch (OperationCanceledException)
			{
				return;
			}

			catch (Exception ex)
			{
				StatusMessage = $"Ошибка загрузки сообщений: {ex.Message}";
				await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
			}
		}
	}

	private void UpdateMessages(IEnumerable<MessageDto> newMessages)
	{
		var currentMessages = Messages.ToDictionary(m => m.Id);

		foreach (var messageDto in newMessages)
		{
			if (messageDto.CreationTime <= _lastMessageRequestTime)
				continue;
			if (currentMessages.TryGetValue(messageDto.Id, out var existingMessage))
			{
				existingMessage.Text = messageDto.Text;
				existingMessage.IsDeleted = messageDto.IsDeleted;
				existingMessage.LastChangeTime = messageDto.LastChangeTime;
			}
			else
			{
				{
                    Messages.Add(new MessageModel
					{
						Id = messageDto.Id,
						Sender = messageDto.UserNickname,
						Text = messageDto.Text,
						Timestamp = messageDto.CreationTime,
						LastChangeTime = messageDto.LastChangeTime,
						IsDeleted = messageDto.IsDeleted
					});
				}
			}

			if (messageDto.CreationTime > _lastMessageRequestTime)
				_lastMessageRequestTime = messageDto.CreationTime;

			var newMessageIds = new HashSet<Guid>(newMessages.Select(m => m.Id));
			for (var i = Messages.Count - 1; i >= 0; i--)
			{
				if (!newMessageIds.Contains(Messages[i].Id))
					Messages.RemoveAt(i);
			}
		}
	}

	public void OnMenuSelected(string tag)
	{
		foreach (var item in Menu.MenuItems)
		{
			item.IsVisible = Visibility.Collapsed;
		}

		switch (tag)
		{
			case "contacts":
				Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "newContact").IsVisible = Visibility.Visible;
				Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "deleteContact").IsVisible = Visibility.Visible;
				Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "back").IsVisible = Visibility.Visible;

                MainWindowState = MainWindowState.Contacts;
                CloseContactSearch();
                if (!Contacts.Any())
                    GetAllContacts();

                break;
                break;
            case "newContact":
                Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "newContact").IsVisible = Visibility.Visible;
                Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "deleteContact").IsVisible = Visibility.Visible;
                Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "back").IsVisible = Visibility.Visible;

                MainWindowState = MainWindowState.Contacts;
                OpenContactSearch();

                break;
            case "deleteContact":
                Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "newContact").IsVisible = Visibility.Visible;
                Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "deleteContact").IsVisible = Visibility.Visible;
                Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "back").IsVisible = Visibility.Visible;

                MainWindowState = MainWindowState.Contacts;
                DeleteContactCommand.Execute(null);

                break;
            case "chats":
				Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "newChat").IsVisible = Visibility.Visible;
				Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "newPrivateChat").IsVisible = Visibility.Visible;
				Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "deleteChat").IsVisible = Visibility.Visible;
				Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "back").IsVisible = Visibility.Visible;

				MainWindowState = MainWindowState.Chats;
				if (!Chats.Any())
                    GetAllChats();

				break;
			case "settings":
				Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "back").IsVisible = Visibility.Visible;
				break;
			case "logOut":
				LogoutCommand.Execute(null);
				break;
			case "back":
				Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "contacts").IsVisible = Visibility.Visible;
				Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "chats").IsVisible = Visibility.Visible;
				Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "settings").IsVisible = Visibility.Visible;
				break;
		}
	}

	private async Task GetAllChats()
	{
		try
		{
			Chats.Clear(); // Очищаем предыдущие чаты
			Console.WriteLine("Запрос чатов...");
			var chats = await _chatsClient.GetChatsAsync().ConfigureAwait(false);
			Console.WriteLine($"Получено чатов: {chats?.Count() ?? 0}");
			// Обновляем коллекцию в UI-потоке
			_dispatcherHelper.CheckBeginInvokeOnUI(() =>
			{
				if (chats != null)
				{
					foreach (var chat in chats)
					{
						Console.WriteLine($"Чат: {chat.Name}, Owner: {chat.OwnerUserId}");
						Chats.Add(chat);
					}

					if (Chats.Any())
					{
						SelectedChat = Chats.First();
						Console.WriteLine($"Выбран чат: {SelectedChat.Name}");
					}
					else
					{
						StatusMessage = "Чаты не найдены";
					}
				}
				else
				{
					StatusMessage = "Сервер вернул пустой список чатов";
				}
			});
		}
		catch (Exception ex)
		{
			StatusMessage = $"Ошибка: {ex.Message}";
			Console.WriteLine($"Ошибка при получении чатов: {ex}");
		}
	}

	private async Task GetAllContacts()
	{
		try
		{
			Contacts.Clear();
			Console.WriteLine("Запрос контактов...");
			var contacts = await _contactsClient.GetContactsAsync().ConfigureAwait(false);
			Console.WriteLine($"Получено контактов: {contacts?.Count() ?? 0}");

			_dispatcherHelper.CheckBeginInvokeOnUI(() =>
			{
				if (contacts != null)
				{
					foreach (var contact in contacts)
					{
						Console.WriteLine($"Контакт: {contact.Name}");
						Contacts.Add(contact);
					}

					if (Contacts.Any())
					{
						SelectedContact = Contacts.First();
						Console.WriteLine($"Выбран контакт: {SelectedContact.Name}");
					}
					else
					{
						StatusMessage = "Контакты не найдены";
					}
				}
				else
				{
					StatusMessage = "Сервер вернул пустой список контактов";
				}
			});
		}
		catch (Exception ex)
		{
			StatusMessage = $"Ошибка: {ex.Message}";
			StatusMessage = $"Ошибка загрузки контактов: {ex.Message}";
		}
	}

    private void OpenContactSearch()
    {
        ContactSearchName = string.Empty;
        FoundUsers.Clear();
        SelectedFoundUser = null;
        IsContactSearchVisible = true;
    }

    private void CloseContactSearch()
    {
        IsContactSearchVisible = false;
        FoundUsers.Clear();
        SelectedFoundUser = null;
        ContactSearchName = string.Empty;
    }

    private bool CanSearchUsers()
    {
        return !ContactSearchName.IsNullOrWhiteSpace();
    }

    private async Task SearchUsersAsync()
    {
        var name = ContactSearchName;
        if (name.IsNullOrWhiteSpace())
            return;

        IsContactSearchInProgress = true;
        try
        {
            var foundUsers = await _contactsClient.SearchUsersAsync(name.Trim()).ConfigureAwait(false);

            _dispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                FoundUsers.Clear();

                foreach (var user in foundUsers.EmptyIfNull())
                    FoundUsers.Add(user);

                StatusMessage = FoundUsers.Any()
                    ? $"Найдено пользователей: {FoundUsers.Count}"
                    : $"Пользователи по запросу \"{name}\" не найдены";
            });
        }
        catch (Exception exception)
        {
            StatusMessage = $"Ошибка поиска пользователей: {exception.Message}";
        }
        finally
        {
            IsContactSearchInProgress = false;
        }
    }

    private async Task AddContactAsync(UserDto? user)
    {
        user ??= SelectedFoundUser;
        if (user == null)
            return;

        if (Contacts.Any(contact => contact.AddedUserNickname.IsEquals(user.Nickname)))
        {
            StatusMessage = $"{user.Nickname} уже есть в контактах";
            return;
        }

        try
        {
            var createContactDto = new ContactDto { Name = user.Name, AddedUserId = user.Id };
            var createdContact = await _contactsClient.CreateContactAsync(createContactDto).ConfigureAwait(false);

            _dispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Contacts.Add(createdContact);
                SelectedContact = createdContact;
                FoundUsers.Remove(user);
                StatusMessage = $"Контакт {user.Nickname} добавлен";
            });
        }
        catch (Exception exception)
        {
            StatusMessage = $"Ошибка добавления контакта: {exception.Message}";
        }
    }

    private async Task DeleteSelectedContactAsync()
    {
        var contact = SelectedContact;
        if (contact == null)
        {
            StatusMessage = "Выберите контакт для удаления";
            return;
        }

        try
        {
            await _contactsClient.DeleteContactsAsync(contact.Id).ConfigureAwait(false);

            _dispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Contacts.Remove(contact);
                SelectedContact = Contacts.FirstOrDefault();
                StatusMessage = $"Контакт {contact.AddedUserNickname} удалён";
            });
        }
        catch (Exception exception)
        {
            StatusMessage = $"Ошибка удаления контакта: {exception.Message}";
        }
    }

    private void OnChatSelected()
	{
		_requestMessagesCts?.Cancel();
		Messages.Clear();
		StartRequestMessages();
		GetNewMessages();
	}


	private void SendMessage()
	{
		SendMessages();
	}

	private async Task SendMessages()
	{
		if (string.IsNullOrWhiteSpace(NewMessageText)) return;

		var newMessage = new MessageModel
		{
			Sender = Login,
			Text = NewMessageText,
			Timestamp = DateTime.Now
		};

		_dispatcherHelper.CheckBeginInvokeOnUI(() => { Messages.Add(newMessage); });


		var sendMessage = await _chatsClient.AddMessageAsync(SelectedChat.Id, newMessage.ToDto()).ConfigureAwait(false);

		NewMessageText = string.Empty;
		StatusMessage = "Сообщение отправлено";
	}

	private void AttachFile()
	{
		var openFileDialog = new Microsoft.Win32.OpenFileDialog();
		if (openFileDialog.ShowDialog() == true)
		{
			StatusMessage = $"Прикреплен файл: {openFileDialog.FileName}";
			// Здесь можно добавить логику обработки файла
		}
	}

	private async Task GetNewMessages()
	{
		if (SelectedChat == null)
			return;
		try
		{
			var messages = await _chatsClient.GetMessagesAsync(SelectedChat.Id).ConfigureAwait(false);
			_dispatcherHelper.CheckBeginInvokeOnUI(() =>
			{
				foreach (var message in messages)
				{
					Messages.Add(new MessageModel
					{
						Id = message.Id,
						Sender = message.UserNickname,
						Text = message.Text,
						Timestamp = message.CreationTime,
						LastChangeTime = message.LastChangeTime,
						IsDeleted = message.IsDeleted
					});

					if (message.CreationTime > _lastMessageRequestTime)
						_lastMessageRequestTime = message.CreationTime;
				}
			});
		}
		catch (Exception ex)
		{
			StatusMessage = $"Ошибка загрузки истории сообщений: {ex.Message}";
		}
	}
	private void OnExit()
	{
		_messenger.Send(new RequestCloseMessage(this, null));
	}

	private void OnContentRendered()
	{
		_loginWindowProvider.CloseIfCreated();

		Login = _tokenProvider.GetLoginFromToken();
	}

	private void OnLogout()
	{
		_tokenProvider.Logout();
		_tokenStorage.SaveToken(null);
		_loginWindowProvider.Show();
	}

	public override void Cleanup()
	{
		_requestMessagesCts?.Cancel();
		base.Cleanup();
	}
}