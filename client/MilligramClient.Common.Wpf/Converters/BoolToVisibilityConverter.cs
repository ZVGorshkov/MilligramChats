using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using MilligramClient.Common.Wpf.Converters.Base;

namespace MilligramClient.Common.Wpf.Converters;

public class BoolToVisibilityConverter : MarkupConverterBase
{
	[ConstructorArgument("TrueValue")]
	public Visibility TrueValue { get; set; }

	[ConstructorArgument("FalseValue")]
	public Visibility FalseValue { get; set; }

	[ConstructorArgument("NullValue")]
	public Visibility NullValue { get; set; }

	public BoolToVisibilityConverter()
	{
		TrueValue = Visibility.Visible;
		FalseValue = Visibility.Collapsed;
		NullValue = Visibility.Collapsed;
	}

	protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value == null)
			return NullValue;

		if (value is not bool valueBool)
			return Binding.DoNothing;

		return valueBool ? TrueValue : FalseValue;
	}

	protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (Equals(value, null))
			return null;

		if (Equals(value, TrueValue))
			return true;

		if (Equals(value, FalseValue))
			return false;

		return Binding.DoNothing;
	}
}