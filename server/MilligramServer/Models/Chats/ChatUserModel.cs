#pragma warning disable CS8618

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MilligramServer.Models.Chats;

public class ChatUserModel
{
    [DisplayName("Id пользователя")]
    public Guid UserId { get; set; }

    [DisplayName("Id чата")]
    public Guid ChatId { get; set; }

    [Required(ErrorMessage = "Не указано название")]
    [DisplayName("Название чата")]
    public string ChatName { get; set; }

    [DisplayName("Чат")]
    public ChatModel Chat { get; set; }
}