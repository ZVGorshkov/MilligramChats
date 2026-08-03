#pragma warning disable CS8618

namespace MilligramServer.Domain.Dtos;

public class MessageDetailsDto
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
    public FileDetailsDto? FileDetailsDto { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime LastChangeTime { get; set; }
    public Guid UserId { get; set; }
    public string UserNickname { get; set; }
    public bool IsDeleted { get; set; }
}