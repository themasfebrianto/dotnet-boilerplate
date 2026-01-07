namespace Boilerplate.Application.Interfaces.Infrastructure;

/// <summary>
/// Image storage service interface.
/// Implemented in Infrastructure layer (e.g., local storage, S3, Azure Blob).
/// </summary>
public interface IImageStorageService
{
    /// <summary>
    /// Upload an image and return its URL.
    /// </summary>
    Task<string> UploadAsync(Stream imageStream, string fileName, string contentType);

    /// <summary>
    /// Delete an image by its URL or path.
    /// </summary>
    Task DeleteAsync(string imageUrl);
}
