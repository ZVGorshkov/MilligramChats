using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace MilligramClient.Common.Wpf.Behaviors;

public static class HyperlinkBehavior
{
    public static readonly DependencyProperty IsExternalProperty =
        DependencyProperty.RegisterAttached(
            "IsExternal",
            typeof(bool),
            typeof(HyperlinkBehavior),
            new UIPropertyMetadata(false, OnIsExternalChanged));

    public static bool GetIsExternal(DependencyObject obj)
    {
        return (bool) obj.GetValue(IsExternalProperty);
    }

    public static void SetIsExternal(DependencyObject obj, bool value)
    {
        obj.SetValue(IsExternalProperty, value);
    }

    private static void OnIsExternalChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not Hyperlink hyperlink)
            return;

        if ((bool) args.NewValue)
            hyperlink.RequestNavigate += HyperlinkRequestNavigate;
        else
            hyperlink.RequestNavigate -= HyperlinkRequestNavigate;
    }

    private static void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo(e.Uri.ToString()));
        }
        catch
        {
            // ignored
        }

        e.Handled = true;
    }
}