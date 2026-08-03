using MahApps.Metro.Controls;
using Microsoft.Xaml.Behaviors;

namespace MilligramClient.Wpf.Views.Main;

public class HamburgerMenuBehavior : Behavior<HamburgerMenu>
{
	protected override void OnAttached()
	{
		base.OnAttached();
		AssociatedObject.ItemInvoked += HamburgerMenu_ItemInvoked;
	}

	protected override void OnDetaching()
	{
		AssociatedObject.ItemInvoked -= HamburgerMenu_ItemInvoked;
		base.OnDetaching();
	}

	private void HamburgerMenu_ItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
	{
		var viewModel = AssociatedObject.DataContext as MainViewModel;
		if (viewModel == null) return;

		if (e.InvokedItem is HamburgerMenuItem item)
		{
			viewModel.OnMenuSelected(item.Tag.ToString());
		}
	}

}