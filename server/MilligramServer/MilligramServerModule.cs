using MilligramServer.Database.Context.Factory;
using MilligramServer.Database.Helpers;
using MilligramServer.Domain.Entities;
using MilligramServer.Extensions;
using MilligramServer.Services.Managers;
using MilligramServer.Services.Migrations;
using MilligramServer.Services.Startup;
using MilligramServer.Services.Stores;
using Microsoft.AspNetCore.Identity;

namespace MilligramServer;

public static class MilligramServerModule
{
    public static void RegisterDependencies(IServiceCollection service, ConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        service.AddSingletonOptions<ApplicationContextStartupOptions>(configuration);

        service.AddSingleton<IApplicationContextFactory, ApplicationContextFactory>();
        service.AddSingleton(_ => ApplicationContextHelper.BuildOptions(connectionString));

        service.AddScoped<ApplicationContextUserManager>();
        service.AddScoped<ApplicationContextRoleManager>();
        service.AddScoped<ApplicationContextSignInManager>();

        service.AddScoped<UserManager<User>, ApplicationContextUserManager>();
        service.AddScoped<RoleManager<Role>, ApplicationContextRoleManager>();
        service.AddScoped<SignInManager<User>, ApplicationContextSignInManager>();

        service.AddScoped<IApplicationContextUserStore, ApplicationContextUserStore>();
        service.AddScoped<IApplicationContextRoleStore, ApplicationContextRoleStore>();
        service.AddScoped<IApplicationContextStartupService, ApplicationContextStartupService>();
        service.AddScoped<IApplicationContextMigrationsService, ApplicationContextMigrationsService>();

        service.AddIdentity<User, Role>()
            .AddUserStore<ApplicationContextUserStore>()
            .AddRoleStore<ApplicationContextRoleStore>();

        service.AddIdentityCore<User>(options =>
        {
            options.User.AllowedUserNameCharacters = Constants.AllowedUserNameCharacters;
        })
        .AddUserStore<ApplicationContextUserStore>()
        .AddDefaultTokenProviders();
    }
}