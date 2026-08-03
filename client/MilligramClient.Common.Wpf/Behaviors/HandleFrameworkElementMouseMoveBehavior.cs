using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MilligramClient.Common.Wpf.Behaviors;

public class HandleFrameworkElementMouseMoveBehavior : Behavior<FrameworkElement>
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
        e.Handled = true;
    }
}