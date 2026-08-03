using MilligramClient.Domain.Dtos;

namespace MilligramClient.Api.Clients.Account;

public interface IAccountClient
{
    Task<TokenDto> LoginAsync(
        LoginDto login,
        CancellationToken cancellationToken = default);

    Task RegisterAsync(
        RegisterDto register,
        CancellationToken cancellationToken = default);

    Task<TokenDto> RefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(
        string token,
        ChangePasswordDto changePassword,
        CancellationToken cancellationToken = default);
}