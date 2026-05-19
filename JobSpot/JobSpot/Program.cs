using JobSpot.Constants;
using JobSpot.Data;
using JobSpot.Interfaces;
using JobSpot.Models;
using JobSpot.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Month)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped<IRepository<JobPosting>, JobPostingRepository>();
builder.Services.AddScoped<JobPostingRepository>(); // Register concrete type for custom methods
builder.Services.AddScoped<IUserManager, UserManagerAdapter>();

// Add razor pages support
builder.Services.AddRazorPages();

// Add ClaimsService for managing user claims
builder.Services.AddScoped<IClaimsService, ClaimsService>();

// Configure Authentication: Identity (default) + Google OAuth (alternative)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
    .AddGoogle("Google", options => // Configure Google OAuth - Storing Credentials Securely
    {
        var clientId = builder.Configuration["Authentication:Google:ClientId"];
        var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException("Google OAuth credentials are not configured.");
        }
        
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        // https://console.cloud.google.com/ -> after Y register your app ->
        // create OAuth 2.0 Client ID -> copy the client ID and client secret here
        // https://console.cloud.google.com/ -> register your app -> set the authorized redirect URI
        // to https://localhost:????/signin-google (or whatever your app URL is) -> use this path as CallbackPath
        options.CallbackPath = "/signin-google"; // This should match the authorized redirect URI you set in the Google Developer Console
        options.SignInScheme = IdentityConstants.ExternalScheme;
        options.SaveTokens = true; // Important: This saves access tokens as UserTokens
        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                // Store the access token and refresh token
                var accessToken = context.AccessToken;
                var refreshToken = context.RefreshToken;

                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Properties.StoreTokens(new[]
                    {
                        new AuthenticationToken { Name = "access_token", Value = accessToken },
                        new AuthenticationToken { Name = "refresh_token", Value = refreshToken ?? "" }
                    });
                }
            }
        };
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // correct the path to your error handling page if needed
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    RoleSeeder.SeedRolesAsync(services).GetAwaiter().GetResult();
    await UserSeeder.SeedUsersAsync(services);
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=JobPostings}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
