using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Load database connection via configuration
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    var tempDirectory = Path.GetTempPath();
    connectionString = $"Data Source={Path.Join(tempDirectory, "Chat.db")}";
}

// Register DbContext
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));
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
    .AddCookie()
    .AddGitHub(o =>
    {
        var clientId = builder.Configuration["GitHub:ClientID"];
        var clientSecret = builder.Configuration["GitHub:ClientSecret"];
        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new ArgumentNullException(null, "ClientID or ClientSecret is null");
        }
        o.ClientId = clientId; 
        o.ClientSecret = clientSecret; 
        o.CallbackPath = "/signin-github";
    });

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Public");
});

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

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