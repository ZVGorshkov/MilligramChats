using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using MilligramClient.Common.Wpf.Converters.Base;

namespace MilligramClient.Common.Wpf.Converters;

public class EnumToVisibilityConverter : MarkupConverterBase
{
    [ConstructorArgument("Inverse")]
    public bool Inverse { get; set; }

    [ConstructorArgument("UseHidden")]
    public bool UseHidden { get; set; }

    public EnumToVisibilityConverter()
    {
        Inverse = false;
        UseHidden = false;
    }

    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter == null)
            throw new InvalidOperationException("You must pass enum value as parameter");

        var isVisible = value != null && (value.Equals(parameter) || (parameter is ICollection collection && collection.Cast<object>().Contains(value))) ? !Inverse : Inverse;
        return isVisible ? Visibility.Visible : UseHidden ? Visibility.Hidden : Visibility.Collapsed;
    }

    protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}