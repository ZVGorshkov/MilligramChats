#pragma warning disable CS8618

namespace MilligramServer.Domain.Dtos;

public class ChatDto
{
    public string Name { get; set; }
    public Guid? OwnerUserId { get; set; }
}