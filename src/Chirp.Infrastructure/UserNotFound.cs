namespace Chirp.Infrastructure;

//Exception thrown if user does not exist
public class UserNotFound : Exception
{
    public UserNotFound()
    { }
    public UserNotFound(string message) : base (message) {}
}