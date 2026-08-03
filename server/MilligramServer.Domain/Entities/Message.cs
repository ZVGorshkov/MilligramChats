#pragma warning disable CS8618

namespace MilligramServer.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
    public Guid? FileId { get; set; }
    public File? File { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime LastChangeTime { get; set; }
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public bool IsDeleted { get; set; }
}