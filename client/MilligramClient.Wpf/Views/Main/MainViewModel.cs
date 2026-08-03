using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

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
using MilligramClient.Wpf.Emoji;
using MilligramClient.Wpf.Views.Login.Logic;

namespace MilligramClient.Wpf.Views.Main;

public class MainViewModel : ViewModel<MainWindow>, INotifyPropertyChanged
{
    private string _login;
    private string _newMessageText;
    private string _statusMessage;
    private HamburgerMenuItem _selectedMenuItem;
    private ChatDto _selectedChat;
    private ChatDto _selectedFoundChat;
    private ContactDto _selectedContact;
    private UserDto _selectedFoundUser;
    private string _contactSearchName;
    private bool _isContactSearchVisible;
    private bool _isContactSearchInProgress;
    private bool _isEmojiPickerOpen;
    private string _chatSearchName;
    private bool _isChatSearchVisible;
    private bool _isChatOperationInProgress;
    private bool _isChatUsersPanelOpen;
    private string _chatUserSearchName;
    private bool _isChatUsersSearchInProgress;
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
    private ICommand _toggleEmojiPickerCommand;
    private ICommand _insertEmojiCommand;
    private ICommand _createChatCommand;
    private ICommand _createPrivateChatCommand;
    private ICommand _deleteChatCommand;
    private ICommand _closeChatSearchCommand;
    private ICommand _findChatCommand;
    private ICommand _toggleChatUsersPanelCommand;
    private ICommand _findChatUsersCommand;
    private ICommand _addUserToChatCommand;
    private ICommand _removeUserFromChatCommand;

    public override object Header => $"Milligram     {_login}";

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
    public IReadOnlyList<EmojiItem> Emojis => EmojiCatalog.Items;
    public ICollectionView ChatsView { get; }
    public ObservableCollection<UserDto> ChatUsers { get; } = new ObservableCollection<UserDto>();
    public ObservableCollection<UserDto> FoundChatUsers { get; } = new ObservableCollection<UserDto>();

    public MainWindowState MainWindowState
    {
        get => _mainWindowState;
        set => Set(ref _mainWindowState, value);
    }

    public string Login
    {
        get => _login;
        set
        {
            if (Set(ref _login, value))
                RaisePropertyChanged(nameof(Header));
        }
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

    public ChatDto SelectedFoundChat
    {
        get => _selectedFoundChat;
        set
        {
            Set(ref _selectedFoundChat, value);
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

    public bool IsEmojiPickerOpen
    {
        get => _isEmojiPickerOpen;
        set => Set(ref _isEmojiPickerOpen, value);
    }

    public string ChatSearchName
    {
        get => _chatSearchName;
        set
        {
            Set(ref _chatSearchName, value);
            ChatsView.Refresh();
        }
    }

    public bool IsChatSearchVisible
    {
        get => _isChatSearchVisible;
        set => Set(ref _isChatSearchVisible, value);
    }

    public bool IsChatOperationInProgress
    {
        get => _isChatOperationInProgress;
        set => Set(ref _isChatOperationInProgress, value);
    }

    public bool IsChatUsersPanelOpen
    {
        get => _isChatUsersPanelOpen;
        set => Set(ref _isChatUsersPanelOpen, value);
    }

    public string ChatUserSearchName
    {
        get => _chatUserSearchName;
        set => Set(ref _chatUserSearchName, value);
    }

    public bool IsChatUsersSearchInProgress
    {
        get => _isChatUsersSearchInProgress;
        set => Set(ref _isChatUsersSearchInProgress, value);
    }

    // Команды
    public ICommand GetContactsCommand => _getContactsCommand ??= new AsyncRelayCommand(GetAllContacts);
    public ICommand ContentRenderedCommand => _contentRenderedCommand ??= new RelayCommand(OnContentRendered);

    public ICommand MenuCommand => _menuCommand ??= new RelayCommand<string>(OnMenuSelected);
    public ICommand LogoutCommand => _logoutCommand ??= new RelayCommand(OnLogout);
    public ICommand ExitCommand => _exitCommand ??= new RelayCommand(OnExit);

    public ICommand SelectChatCommand => new RelayCommand(OnChatSelected);
    public ICommand CreateChatCommand => _createChatCommand ??= new AsyncRelayCommand(CreateChatAsync, CanCreateChat);
    public ICommand CreatePrivateChatCommand => _createPrivateChatCommand ??= new AsyncRelayCommand(CreatePrivateChatAsync);
    public ICommand DeleteChatCommand => _deleteChatCommand ??= new AsyncRelayCommand(DeleteSelectedChatAsync);
    public ICommand FindChatCommand => _findChatCommand ??= new RelayCommand(ToggleChatSearch);
    public ICommand CloseChatSearchCommand => _closeChatSearchCommand ??= new RelayCommand(CloseChatSearch);
    public ICommand ToggleChatUsersPanelCommand => _toggleChatUsersPanelCommand ??= new AsyncRelayCommand(ToggleChatUsersPanelAsync);
    public ICommand FindChatUsersCommand => _findChatUsersCommand ??= new AsyncRelayCommand(SearchChatUsersAsync);
    public ICommand AddUserToChatCommand => _addUserToChatCommand ??= new AsyncRelayCommand<UserDto>(AddUserToChatAsync);
    public ICommand RemoveUserFromChatCommand => _removeUserFromChatCommand ??= new AsyncRelayCommand<UserDto>(RemoveUserFromChatAsync);

    public ICommand FindContactsCommand => _findContactsCommand ??= new AsyncRelayCommand(SearchUsersAsync, CanSearchUsers);
    public ICommand AddContactCommand => _addContactCommand ??= new AsyncRelayCommand<UserDto>(AddContactAsync);
    public ICommand DeleteContactCommand => _deleteContactCommand ??= new AsyncRelayCommand(DeleteSelectedContactAsync);
    public ICommand CloseContactSearchCommand => _closeContactSearchCommand ??= new RelayCommand(CloseContactSearch);

    public ICommand SendMessageCommand => _sendMessageCommand ??= new RelayCommand(SendMessage);
    public ICommand AttachFileCommand => _attachFileCommand ??= new RelayCommand(AttachFile);
    public ICommand ToggleEmojiPickerCommand => _toggleEmojiPickerCommand ??= new RelayCommand(ToggleEmojiPicker);
    public ICommand InsertEmojiCommand => _insertEmojiCommand ??= new RelayCommand<EmojiItem>(InsertEmoji);

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

        ChatsView = CollectionViewSource.GetDefaultView(Chats);
        ChatsView.Filter = FilterChat;

        // Пример сообщения (в реальном приложении будет загрузка из сервера)
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

        var chat = SelectedChat;
        if (chat == null)
            return;

        _ = RequestMessagesAsync(chat.Id, _requestMessagesCts.Token);
    }

    private async Task RequestMessagesAsync(Guid chatId, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var newMessages = await _chatsClient.GetMessagesAsync(chatId, cancellationToken).ConfigureAwait(false);

                _dispatcherHelper.CheckBeginInvokeOnUI(() => { UpdateMessages(chatId, newMessages); });

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

    private void UpdateMessages(Guid chatId, IReadOnlyCollection<MessageDto> newMessages)
    {
        if (SelectedChat?.Id != chatId)
            return;

        var currentMessages = new Dictionary<Guid, MessageModel>();
        foreach (var message in Messages.Where(message => message.Id != Guid.Empty))
            currentMessages[message.Id] = message;

        foreach (var messageDto in newMessages.EmptyIfNull())
        {
            if (currentMessages.TryGetValue(messageDto.Id, out var existingMessage))
            {
                existingMessage.Text = messageDto.Text;
                existingMessage.IsDeleted = messageDto.IsDeleted;
                existingMessage.LastChangeTime = messageDto.LastChangeTime;
            }
            else
            {
                var message = new MessageModel
                {
                    Id = messageDto.Id,
                    Sender = messageDto.UserNickname,
                    Text = messageDto.Text,
                    Timestamp = messageDto.CreationTime,
                    LastChangeTime = messageDto.LastChangeTime,
                    IsDeleted = messageDto.IsDeleted
                };

                currentMessages.Add(message.Id, message);
                Messages.Add(message);
            }

            if (messageDto.CreationTime > _lastMessageRequestTime)
                _lastMessageRequestTime = messageDto.CreationTime;

            var actualMessageIds = newMessages.EmptyIfNull().Select(message => message.Id).ToHashSet();
            for (var i = Messages.Count - 1; i >= 0; i--)
            {
                var message = Messages[i];

                if (message.Id != Guid.Empty && !actualMessageIds.Contains(message.Id))
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
                CloseChatSearch();
                if (!Contacts.Any())
                    GetAllContacts();
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
                ShowChatsMenuItems();

                MainWindowState = MainWindowState.Chats;
                CloseContactSearch();
                CloseChatSearch();
                if (!Chats.Any())
                    GetAllChats();
                break;

            case "findChat":
                ShowChatsMenuItems();

                MainWindowState = MainWindowState.Chats;
                ToggleChatSearch();
                break;

            case "newChat":
                ShowChatsMenuItems();

                MainWindowState = MainWindowState.Chats;
                OpenChatSearch();
                break;

            case "newPrivateChat":
                ShowChatsMenuItems();

                MainWindowState = MainWindowState.Chats;
                CreatePrivateChatCommand.Execute(null);
                break;

            case "deleteChat":
                ShowChatsMenuItems();

                MainWindowState = MainWindowState.Chats;
                DeleteChatCommand.Execute(null);
                break;

            case "settings":
                Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "back").IsVisible = Visibility.Visible;
                break;

            case "logOut":
                LogoutCommand.Execute(null);
                break;

            case "exit":
                _messenger.Send(new RequestCloseMessage(this, null));
                break;

            case "back":
                Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "contacts").IsVisible = Visibility.Visible;
                Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "chats").IsVisible = Visibility.Visible;
                Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "settings").IsVisible = Visibility.Visible;
                break;
        }
    }

    private void ShowChatsMenuItems()
    {
        Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "findChat").IsVisible = Visibility.Visible;
        Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "newChat").IsVisible = Visibility.Visible;
        Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "newPrivateChat").IsVisible = Visibility.Visible;
        Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "deleteChat").IsVisible = Visibility.Visible;
        Menu.MenuItems.FirstOrDefault(i => i.Tag.ToString() == "back").IsVisible = Visibility.Visible;
    }

    private bool FilterChat(object item)
    {
        if (ChatSearchName.IsNullOrWhiteSpace())
            return true;

        return item is ChatDto chat &&
               chat.Name != null &&
               chat.Name.Contains(ChatSearchName.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private void OpenChatSearch()
    {
        ChatSearchName = string.Empty;
        SelectedFoundChat = null;
        FoundChatUsers.Clear();
        IsChatSearchVisible = true;
    }

    private void CloseChatSearch()
    {
        IsChatSearchVisible = false;
        FoundChatUsers.Clear();
        SelectedFoundChat = null;
        ChatSearchName = string.Empty;
    }

    private void ToggleChatSearch()
    {
        if (IsChatSearchVisible)
            CloseChatSearch();
        else
            OpenChatSearch();
    }

    private async Task ToggleChatUsersPanelAsync()
    {
        if (IsChatUsersPanelOpen)
        {
            IsChatUsersPanelOpen = false;
            return;
        }

        if (SelectedChat == null)
        {
            StatusMessage = "Выберите чат";
            return;
        }

        ChatUserSearchName = string.Empty;
        FoundChatUsers.Clear();
        IsChatUsersPanelOpen = true;

        await LoadChatUsersAsync().ConfigureAwait(false);
    }

    private async Task LoadChatUsersAsync()
    {
        var chat = SelectedChat;
        if (chat == null)
            return;

        try
        {
            var users = await _chatsClient.GetUsersAsync(chat.Id).ConfigureAwait(false);

            _dispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ChatUsers.Clear();

                foreach (var user in users.EmptyIfNull())
                    ChatUsers.Add(user);
            });
        }
        catch (Exception exception)
        {
            StatusMessage = $"Ошибка загрузки участников чата: {exception.Message}";
        }
    }

    private async Task SearchChatUsersAsync()
    {
        var name = ChatUserSearchName;
        if (name.IsNullOrWhiteSpace())
            return;

        IsChatUsersSearchInProgress = true;
        try
        {
            var foundUsers = await _contactsClient.SearchUsersAsync(name.Trim()).ConfigureAwait(false);

            _dispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                FoundChatUsers.Clear();

                var chatUserIds = ChatUsers.Select(user => user.Id).ToHashSet();

                foreach (var user in foundUsers.EmptyIfNull().Where(user => !chatUserIds.Contains(user.Id)))
                    FoundChatUsers.Add(user);

                StatusMessage = FoundChatUsers.Any()
                    ? $"Найдено пользователей: {FoundChatUsers.Count}"
                    : $"Пользователи по запросу \"{name}\" не найдены";
            });
        }
        catch (Exception exception)
        {
            StatusMessage = $"Ошибка поиска пользователей: {exception.Message}";
        }
        finally
        {
            IsChatUsersSearchInProgress = false;
        }
    }

    private async Task AddUserToChatAsync(UserDto? user)
    {
        var chat = SelectedChat;
        if (user == null || chat == null)
            return;

        try
        {
            await _chatsClient.AddUserAsync(chat.Id, user.Id).ConfigureAwait(false);

            _dispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ChatUsers.Add(user);
                FoundChatUsers.Remove(user);
                StatusMessage = $"{user.Nickname} добавлен в чат {chat.Name}";
            });
        }
        catch (Exception exception)
        {
            StatusMessage = $"Ошибка добавления пользователя в чат: {exception.Message}";
        }
    }

    private async Task RemoveUserFromChatAsync(UserDto? user)
    {
        var chat = SelectedChat;
        if (user == null || chat == null)
            return;

        try
        {
            await _chatsClient.DeleteUserAsync(chat.Id, user.Id).ConfigureAwait(false);

            _dispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ChatUsers.Remove(user);
                StatusMessage = $"{user.Nickname} удалён из чата {chat.Name}";
            });
        }
        catch (Exception exception)
        {
            StatusMessage = $"Ошибка удаления пользователя из чата: {exception.Message}";
        }
    }

    private bool CanCreateChat()
    {
        return !ChatSearchName.IsNullOrWhiteSpace();
    }

    private async Task CreateChatAsync()
    {
        var name = ChatSearchName;
        if (name.IsNullOrWhiteSpace())
            return;

        await CreateChatAsync(name.Trim(), Array.Empty<Guid>()).ConfigureAwait(false);
    }

    private async Task CreatePrivateChatAsync()
    {
        var contact = SelectedContact;
        if (contact == null)
        {
            StatusMessage = "Выберите контакт для личного чата";
            return;
        }

        try
        {
            var foundUsers = await _contactsClient.SearchUsersAsync(contact.AddedUserNickname).ConfigureAwait(false);
            var user = foundUsers.EmptyIfNull()
                .FirstOrDefault(foundUser => foundUser.Nickname.IsEquals(contact.AddedUserNickname));

            if (user == null)
            {
                StatusMessage = $"Не найден пользователь {contact.AddedUserNickname}";
                return;
            }

            var chatName = contact.Name.IsNullOrWhiteSpace()
                ? contact.AddedUserNickname
                : contact.Name!;

            await CreateChatAsync(chatName, new[] { user.Id }).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            StatusMessage = $"Ошибка создания личного чата: {exception.Message}";
        }
    }

    /// <summary>
    /// Создаёт чат и добавляет в него текущего пользователя:
    /// сервер возвращает только те чаты, в которых пользователь состоит.
    /// </summary>
    private async Task CreateChatAsync(string name, IReadOnlyCollection<Guid> participantIds)
    {
        IsChatOperationInProgress = true;
        try
        {
            var currentUserId = _tokenProvider.GetUserIdFromToken();
            var createdChat = await _chatsClient
                .CreateChatAsync(new ChatDto { Name = name, OwnerUserId = currentUserId })
                .ConfigureAwait(false);

            foreach (var userId in participantIds.Append(currentUserId).Distinct())
                await _chatsClient.AddUserAsync(createdChat.Id, userId).ConfigureAwait(false);

            _dispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Chats.Add(createdChat);
                CloseChatSearch();
                SelectedChat = createdChat;
                StatusMessage = $"Создан чат {createdChat.Name}";
            });
        }
        catch (Exception exception)
        {
            StatusMessage = $"Ошибка создания чата: {exception.Message}";
        }
        finally
        {
            IsChatOperationInProgress = false;
        }
    }

    private async Task DeleteSelectedChatAsync()
    {
        var chat = SelectedChat;
        if (chat == null)
        {
            StatusMessage = "Выберите чат для удаления";
            return;
        }

        try
        {
            await _chatsClient.DeleteChatAsync(chat.Id).ConfigureAwait(false);

            _dispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Chats.Remove(chat);
                SelectedChat = Chats.FirstOrDefault();
                StatusMessage = $"Чат {chat.Name} удалён";
            });
        }
        catch (Exception exception)
        {
            StatusMessage = $"Ошибка удаления чата: {exception.Message}";
        }
    }

    private async Task GetAllChats()
    {
        try
        {
            Chats.Clear(); // Очищаем предыдущие чаты
            var chats = await _chatsClient.GetChatsAsync().ConfigureAwait(false);
            // Обновляем коллекцию в UI-потоке
            _dispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (chats != null)
                {
                    foreach (var chat in chats)
                        Chats.Add(chat);

                    if (Chats.Any())
                        SelectedChat = Chats.First();
                    else
                        StatusMessage = "Чаты не найдены";
                }
                else
                    StatusMessage = "Сервер вернул пустой список чатов";
            });
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    private async Task GetAllContacts()
    {
        try
        {
            Contacts.Clear();
            var contacts = await _contactsClient.GetContactsAsync().ConfigureAwait(false);

            _dispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (contacts != null)
                {
                    foreach (var contact in contacts)
                        Contacts.Add(contact);

                    if (Contacts.Any())
                        SelectedContact = Contacts.First();
                    else
                        StatusMessage = "Контакты не найдены";
                }
                else
                    StatusMessage = "Сервер вернул пустой список контактов";
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
        IsChatUsersPanelOpen = false;
        ChatUsers.Clear();
        FoundChatUsers.Clear();
        _requestMessagesCts?.Cancel();
        Messages.Clear();
        StartRequestMessages();
    }


    private void SendMessage()
    {
        SendMessages();
    }

    private async Task SendMessages()
    {
        if (string.IsNullOrWhiteSpace(NewMessageText)) return;

        var chat = SelectedChat;
        if (chat == null)
        {
            StatusMessage = "Выберите чат";
            return;
        }
        
        var newMessage = new MessageModel
        {
            Sender = Login,
            Text = NewMessageText,
            Timestamp = DateTime.Now
        };

        _dispatcherHelper.CheckBeginInvokeOnUI(() => { Messages.Add(newMessage); });

        try
        {
            var sentMessage = await _chatsClient.AddMessageAsync(chat.Id, newMessage.ToDto()).ConfigureAwait(false);

            // Без Id локальное сообщение осталось бы дублем рядом с пришедшим с сервера.
            _dispatcherHelper.CheckBeginInvokeOnUI(() => { newMessage.Id = sentMessage.Id; });

            NewMessageText = string.Empty;
            StatusMessage = "Сообщение отправлено";
        }
        catch (Exception exception)
        {
            _dispatcherHelper.CheckBeginInvokeOnUI(() => { Messages.Remove(newMessage); });
            StatusMessage = $"Ошибка отправки сообщения: {exception.Message}";
        }
    }

    private void AttachFile()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog();
        if (openFileDialog.ShowDialog() == true)
        {
            StatusMessage = $"Прикреплен файл: {openFileDialog.FileName}";
            // to do Здесь надо добавить логику обработки файла
        }
    }

    private void ToggleEmojiPicker()
    {
        if (!Emojis.Any())
        {
            StatusMessage = "Смайлики не найдены: положите gif-файлы в папку Resources/Emoji";
            return;
        }

        IsEmojiPickerOpen = !IsEmojiPickerOpen;
    }

    private void InsertEmoji(EmojiItem? emoji)
    {
        if (emoji == null)
            return;

        var separator = NewMessageText.IsNullOrEmpty() || NewMessageText.EndsWith(' ')
            ? string.Empty
            : " ";

        NewMessageText = $"{NewMessageText}{separator}{emoji.Code} ";
        IsEmojiPickerOpen = false;
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
        _ = GetAllContacts();
        _ = GetAllChats();
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