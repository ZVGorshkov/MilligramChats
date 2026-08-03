using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MilligramClient.Common.Wpf.Behaviors;

public class DragMoveBehavior : Behavior<Window>
{
	protected override void OnAttached()
	{
		AssociatedObject.MouseMove += OnWindowMouseMove;
	}

	protected override void OnDetaching()
	{
		AssociatedObject.MouseMove -= OnWindowMouseMove;
	}

	private static void OnWindowMouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed || sender is not Window window)
			return;

		try
		{
			if (window.WindowState == WindowState.Maximized)
			{
				window.WindowState = WindowState.Normal;
				if (Application.Current.MainWindow != null)
					Application.Current.MainWindow.Top = 3;
			}

			window.DragMove();
		}
		catch
		{
			// ignored
		}
	}
}