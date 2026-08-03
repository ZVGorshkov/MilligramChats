#pragma warning disable CS8618

using MilligramServer.Models.Users;
using System.ComponentModel;

namespace MilligramServer.Models.Chats;

public class ChatsIndexModel : SortingPaginationModelBase
{
    [DisplayName("Название или id")]
    public string? SearchString { get; set; }

    [DisplayName("UserId")]
    public Guid UserId { get; set; }

    [DisplayName("Пользователь")]
    public UserModel User { get; set; }

    [DisplayName("Чаты")]
    public IReadOnlyCollection<ChatModel> Chats { get; set; }
}