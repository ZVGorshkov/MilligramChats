#pragma warning disable CS8618

namespace MilligramServer.Domain.Dtos;

public class MessageDto
{
    public string? Text { get; set; }
    public FileDto? FileDto { get; set; }
}