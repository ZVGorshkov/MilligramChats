using MilligramServer.Models.Files;
using File = MilligramServer.Domain.Entities.File;

namespace MilligramServer.Extensions.Models;

public static class FileExtensions
{
    public static IEnumerable<FileModel> ToModels(
        this IEnumerable<File> files)
    {
        foreach (var file in files)
            yield return file.ToModel();
    }

    public static FileModel ToModel(this File file)
    {
        return new FileModel
        {
            Id = file.Id,
            Content = file.Content,
            Name = file.Name,
            Extension = file.Extension,
            IsImage = file.IsImage,
            SizeBytes = file.SizeBytes
        };
    }
}