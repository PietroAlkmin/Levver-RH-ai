namespace LevverRH.Domain.Interfaces;

public interface IStorageService
{
    Task<string> UploadLogoAsync(Guid tenantId, Stream fileStream, string fileName, string contentType);
    Task<string> UploadFaviconAsync(Guid tenantId, Stream fileStream, string fileName, string contentType);
    Task<string> UploadProfilePhotoAsync(Guid userId, Stream fileStream, string fileName, string contentType);
    Task<bool> DeleteFileAsync(string fileUrl);
}
