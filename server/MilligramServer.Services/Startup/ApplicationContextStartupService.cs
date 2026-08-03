using MilligramServer.Common.Extensions;
using MilligramServer.Domain;
using MilligramServer.Domain.Entities;
using MilligramServer.Services.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MilligramServer.Services.Startup;

public class ApplicationContextStartupService : IApplicationContextStartupService
{
    private readonly ApplicationContextUserManager _applicationContextUserManager;
    private readonly ApplicationContextRoleManager _applicationContextRoleManager;
    private readonly ApplicationContextStartupOptions _applicationContextStartupOptions;
    private readonly ILogger<ApplicationContextStartupService> _logger;

    public ApplicationContextStartupService(
        ApplicationContextUserManager applicationContextUserManager,
        ApplicationContextRoleManager applicationContextRoleManager,
        ApplicationContextStartupOptions applicationContextStartupOptions,
        ILogger<ApplicationContextStartupService> logger)
    {
        _applicationContextUserManager = applicationContextUserManager;
        _applicationContextRoleManager = applicationContextRoleManager;
        _applicationContextStartupOptions = applicationContextStartupOptions;
        _logger = logger;
    }

    public async Task InitializeUsersAndRolesAsync()
    {
        try
        {
            await InitializeRolesAsync();
            await InitializeUsersAsync();
        }
        catch (Exception exception)
        {
            const string message = "Error on initialize users and roles";

            _logger.LogCritical(exception, message);
            throw new Exception(message, exception);
        }
    }

    private async Task InitializeRolesAsync()
    {
        var adminRole = await _applicationContextRoleManager.FindByNameAsync(RoleTokens.AdminRole);
        var swaggerRole = await _applicationContextRoleManager.FindByNameAsync(RoleTokens.SwaggerRole);

        if (adminRole == null)
            await _applicationContextRoleManager.CreateAsync(new Role { Id = Guid.NewGuid(), Name = RoleTokens.AdminRole });
        if (swaggerRole == null)
            await _applicationContextRoleManager.CreateAsync(new Role { Id = Guid.NewGuid(), Name = RoleTokens.SwaggerRole });
    }

    private async Task InitializeUsersAsync()
    {
        var name = _applicationContextStartupOptions.InitializeUserLogin;
        var password = _applicationContextStartupOptions.InitializeUserPassword;
        if (name.IsNullOrEmpty() || password.IsNullOrEmpty())
            return;

        var hasUsers = await _applicationContextUserManager.Users.AnyAsync();
        if (hasUsers)
            return;

        var user = new User { Id = Guid.NewGuid(), Name = name, Nickname = name };
        await _applicationContextUserManager.CreateAsync(user, password);
        await _applicationContextUserManager.AddToRolesAsync(user, [RoleTokens.AdminRole, RoleTokens.SwaggerRole]);
    }
}