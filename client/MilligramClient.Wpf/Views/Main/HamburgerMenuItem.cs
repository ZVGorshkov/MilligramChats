using System.ComponentModel;
using System.Windows;
using MahApps.Metro.IconPacks;
using MilligramClient.Common;

namespace MilligramClient.Wpf.Views.Main;

public class HamburgerMenuItem : PropertyChangedBase, INotifyPropertyChanged
{
	private string _label;
	private object _tag;
	private Visibility _isVisible;
	private PackIconIoniconsKind _icon;

	public string Label
	{
		get => _label;
		set => Set(ref _label, value);
	}

	public object Tag
	{
		get => _tag;
		set => Set(ref _tag, value);
	}

	public PackIconIoniconsKind Icon
	{
		get => _icon;
		set => Set(ref _icon, value);
	}

	public Visibility IsVisible
	{
		get => _isVisible;
		set => Set(ref _isVisible, value);
	}
}