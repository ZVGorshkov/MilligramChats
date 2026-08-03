#pragma warning disable CS8618

namespace MilligramServer.Services.Startup;

public class ApplicationContextStartupOptions
{
    public string InitializeUserLogin { get; set; }
    public string InitializeUserPassword { get; set; }
}