using GalaSoft.MvvmLight;
using MilligramClient.Wpf.Enums;

namespace MilligramClient.Wpf.Models;

public class CellModel : ObservableObject, ICloneable
{
	public CellValues _value;

	public CellValues Value
	{
		get => _value;
		set => Set(ref _value, value);
	}

	public bool IsNone() => Value == CellValues.None;
	public object Clone()
	{
		var clone = (CellModel) MemberwiseClone();
		return clone;
	}
}