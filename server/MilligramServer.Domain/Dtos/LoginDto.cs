#pragma warning disable CS8618

namespace MilligramServer.Domain.Dtos;

public class LoginDto
{
    public string Login { get; set; }
    public string Password { get; set; }
}