using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CaptureCamera.Interfaces;
using CaptureCamera.Models;
using CaptureCamera.Settings;

namespace CaptureCamera.Services;

/// <summary> Azure Blob Storage </summary>
public class FileStorage : IFileStorage
{
    private readonly BlobSettings _settings;

    private const char Separator = '/';

    public FileStorage(AppSettings settings) => _settings = settings.Storage;

    public async Task<FileDto> Upload(FileDto file, byte[] data, CancellationToken token = default)
    {
        var path = ToFilePath(file);
        var blob = await GetBlob(path, checkIfExists: false, token);

        await blob.UploadAsync(new BinaryData(data), new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
        }, token);

        return await GetFileDto(blob, path, token);
    }

    private static FilePath ToFilePath(FileDto file) => FilePath.From(new List<string>(file.Directory.Split(Separator, StringSplitOptions.RemoveEmptyEntries)) { file.FileName }.ToArray());

    public async Task Download(FileDto file, MemoryStream to, CancellationToken token = default)
    {
        var path = ToFilePath(file);
        var blob = await GetBlob(path, token: token);

        await using var stream = await blob.OpenReadAsync(new BlobOpenReadOptions(false), token);

        await stream.CopyToAsync(to, token);

        to.Position = 0;
    }

    public async Task<bool> Delete(FileDto file, CancellationToken token = default)
    {
        var path = ToFilePath(file);
        var blob = await GetBlob(path, checkIfExists: false, token);

        var deleted = await blob.DeleteIfExistsAsync(cancellationToken: token);

        return deleted.Value;
    }

    public async Task<FolderDto> GetFolder(string path, CancellationToken token = default)
    {
        var client = await ConfigureBlobClient(_settings, token);

        return await GetFilesTree(new FolderDto
        {
            Path = string.Empty,
            Name = _settings.DocumentContainer
        }, client, path, token);
    }

    private static async Task<FolderDto> GetFilesTree(FolderDto parent, BlobContainerClient client, string prefix, CancellationToken token = default)
    {
        try
        {
            var resultSegment = client.GetBlobsByHierarchyAsync(prefix:prefix, delimiter:$"{Separator}").AsPages();

            await foreach (Page<BlobHierarchyItem> page in resultSegment.WithCancellation(token))
            {
                foreach (var item in page.Values)
                {
                    switch (item)
                    {
                        case { IsBlob: true }:
                            parent.Files.Add(GetFileDto(item));
                            break;
                        case { IsPrefix: true }:
                        {
                            var child = GetFolderDto(item);
                            parent.Folders.Add(child);
                            await GetFilesTree(child, client, item.Prefix, token);
                            break;
                        }
                    }
                }
            }

            parent.Files = parent.Files.OrderByDescending(x => x.FileName).ToList();
            parent.Folders = parent.Folders.OrderBy(x => x.Name).ToList();
            return parent;
        }
        catch (Exception exception)
        {
            throw new Exception("Build files tree from Azure blob storage failed.", exception);
        }
    }

    private async Task<BlobClient> GetBlob(FilePath path, bool checkIfExists = true, CancellationToken token = default)
    {
        var blobNameParts = new List<string>(path.Parents) { path.Last };

        var client = await ConfigureBlobClient(_settings, token);
        var blobName = string.Join(Separator, blobNameParts);
        var blob = client.GetBlobClient(blobName);

        if (checkIfExists && !await blob.ExistsAsync(cancellationToken: token))
        {
            throw new KeyNotFoundException($"Blob {blobName} not found.");
        }

        return blob;
    }

    private static async Task<BlobContainerClient> ConfigureBlobClient(BlobSettings settings, CancellationToken token = default)
    {
        var connectionString = settings.ConnectionString;
        var documentContainer = settings.DocumentContainer;

        try
        {
            var client = new BlobServiceClient(connectionString).GetBlobContainerClient(documentContainer);

            return await client.ExistsAsync(token)
                ? client
                : throw new Exception($"Container '{documentContainer}' does not exist in Azure blob storage.");
        }
        catch (Exception exception)
        {
            throw new Exception("Cannot connect to Azure blob storage.", exception);
        }
    }

    private static async Task<FileDto> GetFileDto(BlobClient blob, FilePath path, CancellationToken token)
    {
        var props = await blob.GetPropertiesAsync(default, token);
        var properties = props.Value;

        return new FileDto
        {
            Directory = Path.Combine(path.Parents.ToArray()),
            FileName = path.Last,
            ContentType = properties.ContentType,
            Length = properties.ContentLength
        };
    }

    private static FileDto GetFileDto(BlobHierarchyItem item)
    {
        var path = FilePath.From(item.Blob.Name.Split(Separator, StringSplitOptions.RemoveEmptyEntries));

        return new FileDto
        {
            FileName = path.Last,
            Directory = string.Join(Separator, path.Parents),
            ContentType = item.Blob.Properties.ContentType,
            Length = item.Blob.Properties.ContentLength
        };
    }

    private static FolderDto GetFolderDto(BlobHierarchyItem item)
    {
        var path = FilePath.From(item.Prefix.Split(Separator, StringSplitOptions.RemoveEmptyEntries));

        return new FolderDto
        {
            Name= path.Last,
            Path = string.Join(Separator, path.Parents),
        };
    }
}