using MilligramServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MilligramServer.Services.Stores;

public interface IApplicationContextUserStore : IQueryableUserStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>
{
    Task<User?> FindByIdAndLoadRolesAsync(string userId, CancellationToken cancellationToken);
    Task<User?> FindByNameAndLoadRolesAsync(string normalizedUserName, CancellationToken cancellationToken);
    Task<IdentityResult> RestoreAsync(User user, CancellationToken cancellationToken);
    Task<string?> GetUserNicknameAsync(User user, CancellationToken cancellationToken);
    Task SetUserNicknameAsync(User user, string? nickname, CancellationToken cancellationToken);
    Task<User?> FindByNicknameAsync(string nickname, CancellationToken cancellationToken);
}