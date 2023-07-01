using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CaptureCamera.Models;

namespace CaptureCamera.Interfaces;

/// <summary> File storage management interface </summary>
public interface IFileStorage
{
    Task<FileDto> Upload(FileDto file, byte[] data, CancellationToken token = default);

    Task Download(FileDto file, MemoryStream to, CancellationToken token = default);

    Task<bool> Delete(FileDto file, CancellationToken token = default);

    Task<FolderDto> GetFolder(string path, CancellationToken token = default);
}

