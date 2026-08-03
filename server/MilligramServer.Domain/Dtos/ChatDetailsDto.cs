#pragma warning disable CS8618

namespace MilligramServer.Domain.Dtos;

public class ChatDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? OwnerUserId { get; set; }
}