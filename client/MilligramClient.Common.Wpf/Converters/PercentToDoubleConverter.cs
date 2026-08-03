using System.Globalization;
using MilligramClient.Common.Wpf.Converters.Base;

namespace MilligramClient.Common.Wpf.Converters;

public class PercentToDoubleConverter : MarkupConverterBase
{
	protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is int intValue ? intValue / 100.0 : throw new ArgumentException($"{nameof(PercentToDoubleConverter)} only for int values");
	}

	protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}