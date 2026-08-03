using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace MilligramClient.Common.Wpf.Messages;

public class RequestCloseMessage : MessageBase
{
	public ViewModelBase ViewModel { get; }
	public bool? DialogResult { get; }

	public RequestCloseMessage(ViewModelBase viewModel, bool? dialogResult = null)
	{
		ViewModel = viewModel;
		DialogResult = dialogResult;
	}
}