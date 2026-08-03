using MilligramServer.Domain.Dtos;
using File = MilligramServer.Domain.Entities.File;

namespace MilligramServer.Domain.Extensions;

public static class FileExtensions
{
    public static FileDetailsDto ToDetailsDto(this File file)
    {
        return new FileDetailsDto
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