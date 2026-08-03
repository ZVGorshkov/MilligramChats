using MilligramServer.Domain.Entities;
using MilligramServer.Services.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MilligramServer.Services.Managers;

public class ApplicationContextRoleManager : RoleManager<Role>
{
    public ApplicationContextRoleManager(
        IApplicationContextRoleStore store,
        IEnumerable<IRoleValidator<Role>> roleValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        ILogger<ApplicationContextRoleManager> logger)
        : base(
            store,
            roleValidators,
            keyNormalizer,
            errors,
            logger)
    {
    }

    public async Task Restore(string roleName)
    {
        var applicationContextRoleStore = (IApplicationContextRoleStore)Store;

        var role = applicationContextRoleStore.FindByNameAsync(roleName, CancellationToken).Result;
        if (role != null)
            await applicationContextRoleStore.RestoreAsync(role, CancellationToken).ConfigureAwait(false);
    }
}