using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MilligramClient.Wpf.Emoji;

/// <summary>
/// Набор смайликов из папки Resources/Emoji рядом с исполняемым файлом.
/// Имя файла задаёт код смайлика: smile.gif -> :smile:
/// </summary>
public static class EmojiCatalog
{
	private const string EmojiDirectoryName = "Emoji";

	private static readonly string[] SupportedExtensions = { ".gif", ".png", ".jpg", ".jpeg" };

	private static readonly Lazy<IReadOnlyList<EmojiItem>> LazyItems = new(Load);

	private static readonly Lazy<IReadOnlyDictionary<string, EmojiItem>> LazyItemsByCode =
		new(() => Items.ToDictionary(item => item.Code, StringComparer.OrdinalIgnoreCase));

	public static IReadOnlyList<EmojiItem> Items => LazyItems.Value;

	public static bool TryGetByCode(string code, out EmojiItem? item)
	{
		return LazyItemsByCode.Value.TryGetValue(code, out item);
	}

	private static IReadOnlyList<EmojiItem> Load()
	{
		var directory = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"Resources",
			EmojiDirectoryName);

		if (!Directory.Exists(directory))
			return Array.Empty<EmojiItem>();

		return Directory.EnumerateFiles(directory)
			.Where(file => SupportedExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
			.OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
			.Select(CreateItem)
			.Where(item => item != null)
			.Select(item => item!)
			.ToList();
	}

	private static EmojiItem? CreateItem(string filePath)
	{
		var name = Path.GetFileNameWithoutExtension(filePath);

		try
		{
			return new EmojiItem(name, $":{name}:", CreateSource(filePath));
		}
		catch (Exception)
		{
			return null;
		}
	}

	private static ImageSource CreateSource(string filePath)
	{
		var image = new BitmapImage();
		image.BeginInit();
		image.UriSource = new Uri(filePath, UriKind.Absolute);
		image.CacheOption = BitmapCacheOption.OnLoad;
		image.CreateOptions = BitmapCreateOptions.None;
		image.EndInit();
		image.Freeze();

		return image;
	}
}
