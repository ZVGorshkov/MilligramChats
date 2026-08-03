using System.Windows.Media;

namespace MilligramClient.Wpf.Emoji;

public class EmojiItem
{
	public EmojiItem(string name, string code, ImageSource source)
	{
		Name = name;
		Code = code;
		Source = source;
	}

	public string Name { get; }
	public string Code { get; }
	public ImageSource Source { get; }
}
