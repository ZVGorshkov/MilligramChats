using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MilligramClient.Wpf.Emoji;
using WpfAnimatedGif;

namespace MilligramClient.Wpf.Controls;

/// <summary>
/// Текст сообщения, в котором коды вида :smile: заменяются анимированными смайликами.
/// </summary>
public class EmojiTextBlock : TextBlock
{
	private static readonly Regex EmojiCodeRegex = new(@":[a-zA-Z0-9_+\-]+:", RegexOptions.Compiled);

	public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.Register(
		nameof(FormattedText),
		typeof(string),
		typeof(EmojiTextBlock),
		new PropertyMetadata(default(string), OnFormattedTextChanged));

	public static readonly DependencyProperty EmojiSizeProperty = DependencyProperty.Register(
		nameof(EmojiSize),
		typeof(double),
		typeof(EmojiTextBlock),
		new PropertyMetadata(22d, OnFormattedTextChanged));

	public string? FormattedText
	{
		get => (string?)GetValue(FormattedTextProperty);
		set => SetValue(FormattedTextProperty, value);
	}

	public double EmojiSize
	{
		get => (double)GetValue(EmojiSizeProperty);
		set => SetValue(EmojiSizeProperty, value);
	}

	private static void OnFormattedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((EmojiTextBlock)d).UpdateInlines();
	}

	private void UpdateInlines()
	{
		Inlines.Clear();

		var text = FormattedText;
		if (string.IsNullOrEmpty(text))
			return;

		var lastIndex = 0;

		foreach (var match in EmojiCodeRegex.Matches(text).Cast<Match>())
		{
			if (!EmojiCatalog.TryGetByCode(match.Value, out var emoji) || emoji == null)
				continue;

			if (match.Index > lastIndex)
				Inlines.Add(new Run(text.Substring(lastIndex, match.Index - lastIndex)));

			Inlines.Add(CreateEmojiInline(emoji));
			lastIndex = match.Index + match.Length;
		}

		if (lastIndex < text.Length)
			Inlines.Add(new Run(text.Substring(lastIndex)));
	}

	private Inline CreateEmojiInline(EmojiItem emoji)
	{
		var image = new Image
		{
			Width = EmojiSize,
			Height = EmojiSize,
			Stretch = System.Windows.Media.Stretch.Uniform,
			ToolTip = emoji.Code
		};

		ImageBehavior.SetRepeatBehavior(image, System.Windows.Media.Animation.RepeatBehavior.Forever);
		ImageBehavior.SetAnimatedSource(image, emoji.Source);

		return new InlineUIContainer(image) { BaselineAlignment = BaselineAlignment.Center };
	}
}
