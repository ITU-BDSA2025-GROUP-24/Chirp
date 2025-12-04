using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace Chirp.Infrastructure;

public interface IProfileImageStorage
{
    Task<string> UploadProfileImageAsync(Stream imageStream, string contentType, string fileName);
}

public class AzureBlobProfileImageStorage : IProfileImageStorage
{
    private readonly BlobContainerClient _container;

    public AzureBlobProfileImageStorage(IConfiguration configuration)
    {
        var connectionString = configuration["AzureStorage:ConnectionString"];
        var containerName    = configuration["AzureStorage:ProfileImagesContainer"];

        Console.WriteLine($"[BlobStorage] AzureStorage:ConnectionString = '{connectionString}'");
        Console.WriteLine($"[BlobStorage] AzureStorage:ProfileImagesContainer = '{containerName}'");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "AzureStorage:ConnectionString is missing or empty. " +
                "On this machine you need to configure it in appsettings.Development.json or user-secrets.");

        if (string.IsNullOrWhiteSpace(containerName))
            containerName = "profile-images";

        var serviceClient = new BlobServiceClient(connectionString);
        _container = serviceClient.GetBlobContainerClient(containerName);
    }

    public async Task<string> UploadProfileImageAsync(Stream imageStream, string contentType, string fileName)
    {
        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var blob = _container.GetBlobClient(blobName);

        await blob.UploadAsync(imageStream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        });

        return blob.Uri.ToString();
    }
}