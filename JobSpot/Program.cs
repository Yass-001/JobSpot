using JobSpot.Constants;
using JobSpot.Data;
using JobSpot.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JobSpot.Models;
using JobSpot.Interfaces;
using Serilog;
using Microsoft.AspNetCore.Authentication.OAuth;

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

// remove the default cookie authentication configuration added by AddDefaultIdentity and configure it manually to set custom options like cookie name, expiration time, login path, etc. This is necessary because AddDefaultIdentity adds cookie authentication with default settings, and we want to customize those settings for our application. By calling AddAuthentication() and AddCookie() after AddDefaultIdentity(), we can override the default cookie authentication configuration with our custom settings.
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme; // This is the default scheme used for authentication, which is cookie-based authentication configured by AddDefaultIdentity. You can specify a different scheme if you have multiple authentication methods.
//    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
//    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
//})
//    .AddIdentityCookies(options =>
//    {
//        // Configure the actual CookieAuthenticationOptions via the OptionsBuilder
//        options.ApplicationCookie?.Configure(o =>
//        {
//            o.ExpireTimeSpan = TimeSpan.FromHours(8);
//            o.AccessDeniedPath = "/Identity/Account/AccessDenied";
//            // other cookie options on `o`...
//        });
//    });

builder.Services.AddScoped<IRepository<JobPosting>, JobPostingRepository>();

builder.Services.AddScoped<IUserManager, UserManagerAdapter>();

// Add razor pages support
builder.Services.AddRazorPages();

// AddDefaultIdentity<IdentityUser>() already configures cookie authentication internally
//configure application cookie settings for authentication
//builder.Services.AddAuthentication(defaultScheme: "cookie") // different authentication schemes can be added here if needed, for example, JWT Bearer, OAuth, etc. Default is cookie-based authentication, so we just can call AddAuthentication() without parameters.
//    .AddCookie("cookie", options =>
//    {
//        options.Cookie.Name = "JobSpotAuthCookie";
//        options.ExpireTimeSpan = TimeSpan.FromHours(8);
//        options.LoginPath = "/Identity/Account/Login";
//        options.LogoutPath = "/Identity/Account/Logout";
//        options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // don`t forget to set this
//    });
//.AddGoogle("Google", options =>
//{
//    options.ClientId = ""; // https://console.cloud.google.com/ -> after Y register your app ->
//                           // create OAuth 2.0 Client ID -> copy the client ID and client secret here
//    options.ClientSecret = "";
//    //options.CallbackPath = "/signin-google"; // https://console.cloud.google.com/ -> register your app -> set the authorized redirect URI to https://localhost:????/signin-google (or whatever your app URL is) -> use this path as CallbackPath
//    options.SignInScheme = "cookie"; // specify the sign-in scheme to use after successful authentication

//options.Events = new OAuthEvents
//{
//    OnCreatingTicket = e =>
//    {
//        e.Principal. // Here you can add custom claims or perform additional processing after successful authentication
//    }
//};       
//});

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
