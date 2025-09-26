namespace Chirp.CLI;
public static class Config
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("CHIRP_DB_URL")?.TrimEnd('/') ?? "http://localhost:5000";
}
