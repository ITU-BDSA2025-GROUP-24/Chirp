namespace Chirp.Infrastructure;

public interface IProfileImageStorage
{
    Task<string> UploadProfileImageAsync(Stream imageStream, string contentType, string fileName);
}