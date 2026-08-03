#pragma warning disable CS8618

using MilligramServer.Models.Chats;
using MilligramServer.Models.Users;
using System.ComponentModel;

namespace MilligramServer.Models.Messages;

public class MessagesIndexModel : SortingPaginationModelBase
{
    [DisplayName("id")]
    public string? SearchString { get; set; }

    [DisplayName("ChatId")]
    public Guid ChatId { get; set; }

    [DisplayName("Чат")]
    public ChatModel Chat { get; set; }

    [DisplayName("UserId")]
    public Guid UserId { get; set; }

    [DisplayName("Пользователь")]
    public UserModel User { get; set; }

    [DisplayName("Сообщения")]
    public IReadOnlyCollection<MessageModel> Messages { get; set; }
}