using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Chirp.Infrastructure;
using Chirp.Core;
using Microsoft.Extensions.Configuration;

namespace Chirp.Web.Controllers;

[Route("profile-image")]
public class ProfileImageController : Controller
{
    private readonly IAuthorRepository _authorRepository;
    private readonly BlobContainerClient _container;

    public ProfileImageController(IAuthorRepository authorRepository, IConfiguration configuration)
    {
        _authorRepository = authorRepository;

        var connectionString = configuration["AzureStorage:ConnectionString"]
            ?? throw new InvalidOperationException("AzureStorage:ConnectionString is missing");

        var containerName = configuration["AzureStorage:ProfileImagesContainer"] ?? "profile-images";

        _container = new BlobContainerClient(connectionString, containerName);
    }
    
    
    //Fetches picture from azure blob container
    [HttpGet("{authorName}")]
    public async Task<IActionResult> Get(string authorName)
    {
        Console.WriteLine($"[ProfileImageController] Request for {authorName}");
        AuthorDTO author;

        try
        {
            author = await _authorRepository.GetAuthorByName(authorName);
        }
        catch (UserNotFound)
        {
            return File("/images/default-user.png", "image/png");
        }

        if (string.IsNullOrWhiteSpace(author.ProfileImageUrl))
        {
            return File("/images/default-user.png", "image/png");
        }
        
        string blobName;
        try
        {
            var uri = new Uri(author.ProfileImageUrl);
            blobName = Path.GetFileName(uri.AbsolutePath); 
        }
        catch
        {
            return File("/images/default-user.png", "image/png");
        }

        var blobClient = _container.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            return File("/images/default-user.png", "image/png");
        }

        var download = await blobClient.DownloadStreamingAsync();
        var contentType = download.Value.Details.ContentType ?? "image/png";

        return File(download.Value.Content, contentType);
    }
}
