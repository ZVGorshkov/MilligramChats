#pragma warning disable CS8618

namespace MilligramServer.Domain.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Nickname { get; set; }
}