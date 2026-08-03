using System.Windows;

namespace MilligramClient.Common.Wpf.Base;

public interface IViewModel
{
	object Header { get; }
	FrameworkElement View { get; }
}