using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace Chirp.Infrastructure;

public class AzureBlobProfileImageStorage : IProfileImageStorage
{
    private readonly BlobContainerClient _container;

    //Define connection string
    public AzureBlobProfileImageStorage(IConfiguration configuration)
    {
        //connectionString and containerName are retrieved from the configuration
        var connectionString = configuration["AzureStorage:ConnectionString"];
        var containerName    = configuration["AzureStorage:ProfileImagesContainer"];

        //Error handling - If connectionString is null or whitespace, throw exception
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "AzureStorage:ConnectionString is missing or empty. " +
                "On this machine you need to configure it in appsettings.Development.json or user-secrets.");

        //Same as error handling above, checks containerName instead
        if (string.IsNullOrWhiteSpace(containerName))
            containerName = "profile-images";

        //Creates an instance of the client and BlobContainer
        var serviceClient = new BlobServiceClient(connectionString);
        _container = serviceClient.GetBlobContainerClient(containerName);
    }

    // Uploads a profile image to the AzureBlobStorage container and returns its URL
    public async Task<string> UploadProfileImageAsync(Stream imageStream, string contentType, string fileName)
    {
        // Generate a blob name by combining a new GUID with the original file extension
        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        // Get a client reference to the blob using the generated name
        var blob = _container.GetBlobClient(blobName);

        // Upload the image stream to AzureBlobStorage with the appropriate HTTP headers
        await blob.UploadAsync(imageStream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        });

        // Return the publicly accessible URL of the uploaded blob
        return blob.Uri.ToString();
    }
}