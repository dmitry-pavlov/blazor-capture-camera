using System.Collections.Generic;

namespace CaptureCamera.Models;

public record FileDto 
{
    public string Directory { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long? Length { get; set; }
}

public class FolderDto 
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public List<FileDto> Files { get; set; } = new();
    public List<FolderDto> Folders { get; set; } = new();
}