using System.Linq;
using CaptureCamera.Extensions;
using CaptureCamera.Interfaces;

namespace CaptureCamera.Settings;

public class BlobSettings : ISelfCheck
{
    /// <summary> The connection string for Azure blob storage account </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// The container name <see href="https://docs.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata">must be a valid DNS name, lowercase</see>.
    /// </summary>
    public string DocumentContainer { get; set; } = string.Empty;

   
    void ISelfCheck.Check()
    {
        this.ThrowIfNullOrEmpty(ConnectionString, nameof(ConnectionString));
        this.ThrowIfNullOrEmpty(DocumentContainer, nameof(DocumentContainer));

        const string documentation = "https://docs.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata";

        this.ThrowIf(() =>
        {
            return DocumentContainer.Any(c => !char.IsLower(c) || !(char.IsLetterOrDigit(c) || c == '-'))
                   || !char.IsLetterOrDigit(DocumentContainer.First())
                   || !char.IsLetterOrDigit(DocumentContainer.Last());
        }, $"{nameof(DocumentContainer)} '{DocumentContainer}' is not valid, see: {documentation}");
    }
}