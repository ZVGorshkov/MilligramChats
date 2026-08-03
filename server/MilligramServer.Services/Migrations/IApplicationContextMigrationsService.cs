namespace MilligramServer.Services.Migrations;

public interface IApplicationContextMigrationsService
{
    Task ApplyMigrationsAsync();
}