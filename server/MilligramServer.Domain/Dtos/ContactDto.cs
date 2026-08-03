#pragma warning disable CS8618

namespace MilligramServer.Domain.Dtos;

public class ContactDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string AddedUserNickname { get; set; }
}