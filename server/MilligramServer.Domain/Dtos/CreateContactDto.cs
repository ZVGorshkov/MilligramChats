#pragma warning disable CS8618

namespace MilligramServer.Domain.Dtos;

public class CreateContactDto
{
    public string? Name { get; set; }
    public Guid AddedUserId { get; set; }
}