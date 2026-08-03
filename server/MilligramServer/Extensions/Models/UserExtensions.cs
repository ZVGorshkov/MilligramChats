using MilligramServer.Domain;
using MilligramServer.Domain.Entities;
using MilligramServer.Models.Users;
using MilligramServer.Services.Managers;

namespace MilligramServer.Extensions.Models;

public static class UserExtensions
{
    public static async IAsyncEnumerable<UserModel> ToModelsAsync(
        this IEnumerable<User> users,
        ApplicationContextUserManager applicationContextUserManager)
    {
        foreach (var user in users)
            yield return await user.ToModelAsync(applicationContextUserManager);
    }

    public static async Task<UserModel> ToModelAsync(
        this User user,
        ApplicationContextUserManager applicationContextUserManager)
    {
        var roles = await applicationContextUserManager.GetRolesAsync(user);
        return user.ToModel(roles.Contains(RoleTokens.AdminRole), roles.Contains(RoleTokens.SwaggerRole));
    }

    public static UserModel ToModel(this User user, bool hasAdminRole, bool hasSwaggerRole)
    {
        return new UserModel
        {
            Id = user.Id,
            Name = user.Name,
            Nickname = user.Nickname,
            HasAdminRole = hasAdminRole,
            HasSwaggerRole = hasSwaggerRole,
            IsDeleted = user.IsDeleted
        };
    }
}