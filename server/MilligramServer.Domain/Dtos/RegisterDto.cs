#pragma warning disable CS8618

namespace MilligramServer.Domain.Dtos;

public class RegisterDto
{
    public string Login { get; set; }
    public string Password { get; set; }
}