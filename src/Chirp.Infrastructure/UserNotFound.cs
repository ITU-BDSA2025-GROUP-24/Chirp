namespace Chirp.Infrastructure;

public class UserNotFound : Exception
{
    public UserNotFound()
    { }
    public UserNotFound(string message) : base (message) {}
}