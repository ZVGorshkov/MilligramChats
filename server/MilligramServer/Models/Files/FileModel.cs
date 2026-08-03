#pragma warning disable CS8618

namespace MilligramServer.Models.Files;

public class FileModel
{
    public Guid Id { get; set; }
    public byte[] Content { get; set; }
    public string Name { get; set; }
    public string Extension { get; set; }
    public long SizeBytes { get; set; }
    public bool IsImage { get; set; }
}