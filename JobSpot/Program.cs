using JobSpot.Constants;
using JobSpot.Data;
using JobSpot.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JobSpot.Models;
using JobSpot.Interfaces;
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

//configure application cookie settings for authentication
builder.Services.AddAuthentication(defaultScheme: "cookie") // different authentication schemes can be added here if needed, for example, JWT Bearer, OAuth, etc. Default is cookie-based authentication, so we just can call AddAuthentication() without parameters.
    .AddCookie("cookie", options =>
    {
        options.Cookie.Name = "JobSpotAuthCookie";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath = "/Identity/Account/Logout";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // don`t forget to set this
    })
    .AddGoogle("Google", options =>
    {

    });

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
