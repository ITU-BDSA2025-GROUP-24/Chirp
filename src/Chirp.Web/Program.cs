using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Determine database connection based on environment.
var connection = String.Empty;

if (builder.Environment.IsDevelopment())
{
    // For development, use local SQLite database
    var tempDirectory = Path.GetTempPath();
    connection = $"Data Source={Path.Join(tempDirectory, "Chirp.db")}";
}
else
{
    // For production, try to get connection string from configuration
    connection = builder.Configuration.GetConnectionString("DefaultConnection");
    
    // Use local database if connection string is not configured
    if (string.IsNullOrEmpty(connection))
    {
        var tempDirectory = Path.GetTempPath();
        connection = $"Data Source={Path.Join(tempDirectory, "Chirp.db")}";
    }
}
var connectionString = builder.Configuration["AzureStorage:ConnectionString"];
var containerName    = builder.Configuration["AzureStorage:ProfileImagesContainer"];
// Configures ChirpDBContext with database connection.
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connection));

// Add repositories and memory cache
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddMemoryCache();

// Add authentication services
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "GitHub";
    })
    .AddCookie(options =>
    {
        // Configure cookie for HTTP in development
        if (builder.Environment.IsDevelopment())
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        }
    })
    .AddGitHub(o =>
    {
        var clientId = builder.Configuration["GitHub:ClientID"];
        var clientSecret = builder.Configuration["GitHub:ClientSecret"];
        
        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException("GitHub ClientID or ClientSecret is not configured properly.");
        }
        
        o.ClientId = clientId; 
        o.ClientSecret = clientSecret; 
        o.CallbackPath = "/signin-github";
    });

// Configure Razor Pages
builder.Services.AddRazorPages();
builder.Services.AddControllers(); 

// Configure antiforgery to work with HTTP in development
builder.Services.AddAntiforgery(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    }
});

builder.Services.AddSingleton<IProfileImageStorage, AzureBlobProfileImageStorage>();
builder.Services.AddSession();

var app = builder.Build();

// Database migration and initialization
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ChirpDBContext>();

    // Applies database migrations
    context.Database.Migrate();

    // Seed the database with initial data
    DbInitializer.SeedDatabase(context);
}

// Configures the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    // In development, show detailed errors and don't force HTTPS
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

// Important! - Authentication and Authorization has to come after UseRouting and before MapRazorPages
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Map endpoints
app.MapControllers();
app.MapRazorPages();

// API endpoints for login/logout
app.MapGet("/Account/Login", () =>
{
    return Results.Challenge(
        new AuthenticationProperties { RedirectUri = "/" },
        authenticationSchemes: new List<string> { "GitHub" }
    );
}).AllowAnonymous();

app.MapGet("/Account/Logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.Run();