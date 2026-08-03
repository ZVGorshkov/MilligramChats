using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using MilligramClient.Common.Wpf.Converters.Base;

namespace MilligramClient.Common.Wpf.Converters;

public class StringEmptyToVisibilityConverter : MarkupConverterBase
{
    [ConstructorArgument("Inverse")]
    public bool Inverse { get; set; }

    [ConstructorArgument("UseHidden")]
    public bool UseHidden { get; set; }

    public StringEmptyToVisibilityConverter()
    {
        Inverse = false;
    }

    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return string.IsNullOrEmpty(value?.ToString())
            ? Inverse
                ? Visibility.Visible
                : UseHidden
                    ? Visibility.Hidden
                    : Visibility.Collapsed
            : Inverse
                ? UseHidden
                    ? Visibility.Hidden
                    : Visibility.Collapsed
                : Visibility.Visible;
    }

    protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}