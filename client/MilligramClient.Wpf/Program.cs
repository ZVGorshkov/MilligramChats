namespace MilligramClient.Wpf;

public static class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		Locator.Current.Locate<App>().Run();
	}
}