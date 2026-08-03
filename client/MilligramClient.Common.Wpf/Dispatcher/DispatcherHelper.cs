using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;

namespace MilligramClient.Common.Wpf.Dispatcher;

public class DispatcherHelperAdapter : IDispatcherHelper
{
	public System.Windows.Threading.Dispatcher UiDispatcher => DispatcherHelper.UIDispatcher;

	public DispatcherHelperAdapter()
	{
		DispatcherHelper.Initialize();
	}

	public void CheckBeginInvokeOnUI(Action action)
	{
		DispatcherHelper.CheckBeginInvokeOnUI(action);
	}

	public DispatcherOperation RunAsync(Action action)
	{
		return DispatcherHelper.RunAsync(action);
	}

	public void Reset()
	{
		DispatcherHelper.Reset();
	}
}