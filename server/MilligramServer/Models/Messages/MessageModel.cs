#pragma warning disable CS8618

using MilligramServer.Models.Files;
using System.ComponentModel;

namespace MilligramServer.Models.Messages;

public class MessageModel
{
    [DisplayName("Id")]
    public Guid Id { get; set; }

    [DisplayName("Текст")]
    public string? Text { get; set; }

    [DisplayName("Файл")]
    public Guid? FileId { get; set; }

    [DisplayName("Файл")]
    public FileModel? File { get; set; }

    [DisplayName("Создано")]
    public DateTime CreationTime { get; set; }

    [DisplayName("Изменено")]
    public DateTime LastChangeTime { get; set; }

    [DisplayName("Чат")]
    public Guid ChatId { get; set; }

    [DisplayName("Чат")]
    public string ChatName { get; set; }

    [DisplayName("Автор")]
    public Guid UserId { get; set; }

    [DisplayName("Автор")]
    public string UserName { get; set; }

    [DisplayName("Статус")]
    public bool IsDeleted { get; set; }
}