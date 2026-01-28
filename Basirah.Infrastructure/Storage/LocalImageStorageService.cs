using Basirah.Application.Interfaces.Infrastructure;

namespace Basirah.Infrastructure.Storage;

/// <summary>
/// Local file system image storage implementation.
/// For production, consider using cloud storage (S3, Azure Blob, etc.)
/// </summary>
public class LocalImageStorageService : IImageStorageService
{
    private readonly string _uploadPath = Path.Combine("wwwroot", "uploads");
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public async Task<string> UploadAsync(Stream imageStream, string fileName, string contentType)
    {
        // Validate file size
        if (imageStream.Length > MaxFileSize)
            throw new InvalidOperationException($"File size exceeds {MaxFileSize / 1024 / 1024}MB limit.");

        // Validate content type
        if (!IsValidImageType(contentType))
            throw new InvalidOperationException("Invalid image type. Allowed: jpeg, png, gif, webp.");

        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);

        // Generate unique filename
        var extension = GetExtensionFromContentType(contentType);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadPath, uniqueFileName);

        // Save file
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await imageStream.CopyToAsync(fileStream);

        // Return relative URL
        return $"/uploads/{uniqueFileName}";
    }

    public Task DeleteAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return Task.CompletedTask;

        try
        {
            // Extract filename from URL
            var fileName = Path.GetFileName(new Uri(imageUrl, UriKind.RelativeOrAbsolute).LocalPath);
            var filePath = Path.Combine(_uploadPath, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        catch
        {
            // Log but don't throw - file deletion shouldn't fail the operation
        }

        return Task.CompletedTask;
    }

    private static bool IsValidImageType(string contentType)
    {
        var validTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        return validTypes.Contains(contentType.ToLowerInvariant());
    }

    private static string GetExtensionFromContentType(string contentType) => contentType.ToLowerInvariant() switch
    {
        "image/jpeg" => ".jpg",
        "image/png" => ".png",
        "image/gif" => ".gif",
        "image/webp" => ".webp",
        _ => ".bin"
    };
}
