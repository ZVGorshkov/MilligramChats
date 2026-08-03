namespace MilligramServer.Services.Startup;

public interface IApplicationContextStartupService
{
    Task InitializeUsersAndRolesAsync();
}