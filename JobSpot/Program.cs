using JobSpot.Constants;
using JobSpot.Data;
using JobSpot.Interfaces;
using JobSpot.Models;
using JobSpot.Repositories;
using Microsoft.AspNetCore.Authentication;
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

builder.Services.AddScoped<IUserManager, UserManagerAdapter>();

// Add razor pages support
builder.Services.AddRazorPages();

// Add ClaimsService for managing user claims
builder.Services.AddScoped<IClaimsService, ClaimsService>();

// Add Claims-based Authorization Policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanCreateJobPosting", policy =>
        policy.RequireClaim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.CreateJobPosting))
    .AddPolicy("CanEditJobPosting", policy =>
        policy.RequireClaim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.EditJobPosting))
    .AddPolicy("CanDeleteJobPosting", policy =>
        policy.RequireClaim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.DeleteJobPosting))
    .AddPolicy("CanViewJobPosting", policy =>
        policy.RequireClaim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.ViewJobPosting))
    .AddPolicy("CanManageUsers", policy =>
        policy.RequireClaim(ApplicationClaimTypes.Permission, ApplicationClaimTypes.ManageUsers))
    .AddPolicy("IsEmployer", policy =>
        policy.RequireRole(UserRoles.Employer))
    .AddPolicy("IsAdmin", policy =>
        policy.RequireRole(UserRoles.Admin))
    .AddPolicy("EmployerOrAdmin", policy =>
        policy.RequireRole(UserRoles.Employer, UserRoles.Admin));

// Configure Authentication with Google OAuth and Custom Claims
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
    options.DefaultSignInScheme = "Cookies";
})
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "JobSpotAuthCookie";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath = "/Identity/Account/Logout";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        options.SlidingExpiration = true;
    })
    .AddGoogle("Google", options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? ""; // https://console.cloud.google.com/ -> after Y register your app ->
                                                                                          // create OAuth 2.0 Client ID -> copy the client ID and client secret here
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? ""; // https://console.cloud.google.com/ -> register your app -> set the authorized redirect URI
                                                                                                  // to https://localhost:????/signin-google (or whatever your app URL is) -> use this path as CallbackPath
        options.CallbackPath = "/signin-google"; // This should match the authorized redirect URI you set in the Google Developer Console
        options.SignInScheme = "Cookies";
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
    await ClaimSeeder.SeedClaimsAsync(services);
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
