namespace MilligramClient.Wpf;

public static class DemoLocator
{
	public static TService Locate<TService>() where TService : class
	{
		if (typeof(TService).Name == Locator.FactoryName)
			return null;

		return Locator.Current.Locate<TService>();
	}
}