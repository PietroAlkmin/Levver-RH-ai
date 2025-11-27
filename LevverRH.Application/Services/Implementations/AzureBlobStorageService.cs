using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LevverRH.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LevverRH.Application.Services.Implementations;

public class AzureBlobStorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string LogosContainer = "logos";
    private const string FaviconsContainer = "favicons";
    private const string ProfilePhotosContainer = "profile-photos";

    public AzureBlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage");
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadLogoAsync(Guid tenantId, Stream fileStream, string fileName, string contentType)
    {
        return await UploadFileAsync(LogosContainer, tenantId, fileStream, fileName, contentType);
    }

    public async Task<string> UploadFaviconAsync(Guid tenantId, Stream fileStream, string fileName, string contentType)
    {
        return await UploadFileAsync(FaviconsContainer, tenantId, fileStream, fileName, contentType);
    }

    public async Task<string> UploadProfilePhotoAsync(Guid userId, Stream fileStream, string fileName, string contentType)
    {
        return await UploadFileAsync(ProfilePhotosContainer, userId, fileStream, fileName, contentType);
    }

    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            var uri = new Uri(fileUrl);
            var blobName = uri.AbsolutePath.TrimStart('/');
            var segments = blobName.Split('/', 2);
            
            if (segments.Length != 2) return false;

            var containerName = segments[0];
            var fileName = segments[1];

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            return await blobClient.DeleteIfExistsAsync();
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> UploadFileAsync(string containerName, Guid tenantId, Stream fileStream, string fileName, string contentType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var extension = Path.GetExtension(fileName);
        var blobName = $"tenant-{tenantId}/{Guid.NewGuid()}{extension}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };
        
        await blobClient.UploadAsync(fileStream, new BlobUploadOptions 
        { 
            HttpHeaders = blobHttpHeaders 
        });

        return blobClient.Uri.ToString();
    }
}
