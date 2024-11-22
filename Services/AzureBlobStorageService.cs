using Azure.Storage.Blobs;

namespace EchiBackendServices.Services;

public class AzureBlobStorageService
{
    private readonly string? _storageConnectionString = Environment.GetEnvironmentVariable("AzureBlobStorageConnectionString");

    public async Task<string> UploadFileToAzureStorage(Stream fileStream, string containerName, string fileName)
    {
        var blobContainerClient = new BlobContainerClient(_storageConnectionString, containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        fileStream.Position = 0;
        await blobClient.UploadAsync(fileStream, true);
        var blobUri = blobClient.Uri.AbsoluteUri;
        return blobUri;
    }

    public async Task<byte[]> DownloadFileFromAzureStorage(string containerName, string fileName)
    {
        try
        {
            var blobContainerClient = new BlobContainerClient(_storageConnectionString, containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            // Download the blob into a memory stream
            var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin); // Reset the stream position to the beginning

            // Read the memory stream into a byte array
            byte[] imageData = memoryStream.ToArray();

            return imageData;
        }
        catch (Exception ex)
        {
            // Handle exceptions according to your application's requirements
            Console.WriteLine($"Error downloading blob: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteFileFromAzureStorage(string containerName, string fileName)
    {
        try
        {
            var blobContainerClient = new BlobContainerClient(_storageConnectionString, containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            // Delete the blob
            await blobClient.DeleteIfExistsAsync();

            return true;
        }
        catch (Exception ex)
        {
            // Handle exceptions according to your application's requirements
            Console.WriteLine($"Error deleting file: {ex.Message}");
            return false;
        }
    }
}