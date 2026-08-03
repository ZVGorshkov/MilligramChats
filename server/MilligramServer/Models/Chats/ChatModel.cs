#pragma warning disable CS8618

using System.ComponentModel;

namespace MilligramServer.Models.Chats;

public class ChatModel
{
    [DisplayName("Id")]
    public Guid Id { get; set; }

    [DisplayName("Название")]
    public string Name { get; set; }

    [DisplayName("Владелец")]
    public Guid? OwnerUserId { get; set; }

    [DisplayName("Владелец")]
    public string? OwnerUserName { get; set; }

    [DisplayName("Статус")]
    public bool IsDeleted { get; set; }
}