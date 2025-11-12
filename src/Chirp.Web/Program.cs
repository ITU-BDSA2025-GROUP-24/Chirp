using System.Data;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var tempDirectory = Path.GetTempPath();
var databasePath = Path.Join(tempDirectory, "Chat.db");

builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite($"Data Source={databasePath}"));
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

// Add services to the container.
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
    //options.Conventions.AuthorizeFolder("/");
    options.Conventions.AuthorizeFolder("/").AllowAnonymousToAreaPage("Public", "/");
    
});


// Load database connection via configuration
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.Run();
