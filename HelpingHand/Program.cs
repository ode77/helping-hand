using HelpingHand.Data;
using HelpingHand.Models;
using HelpingHand.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Database ───────────────────────────────────────────────────────────────
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ── 2. Identity ───────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// ── 3. Repositories ───────────────────────────────────────────────────────────
builder.Services.AddScoped<IHelpRequestRepository,
    HelpRequestRepository>();
builder.Services.AddScoped<ICategoryRepository,
    CategoryRepository>();
builder.Services.AddScoped<INotificationRepository,
    NotificationRepository>();
builder.Services.AddScoped<IVolunteerApplicationRepository,
    VolunteerApplicationRepository>();
builder.Services.AddScoped<IRatingRepository,
    RatingRepository>();
builder.Services.AddScoped<ICommentRepository,
    CommentRepository>();
builder.Services.AddScoped<ITemplateRepository,
    TemplateRepository>();

// ── 4. MVC ────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// Allow file uploads up to 10MB
builder.Services.Configure<Microsoft.AspNetCore.Http.Features
    .FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
    });

// ── Build the app — everything above is service registration ──────────────────
var app = builder.Build();

// ── 5. Apply database migrations ──────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();
    try
    {
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration failed: {ex.Message}");
    }
}

// ── 6. Seed roles ─────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var role in new[] { "User", "Admin" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

// ── 6. Middleware pipeline ────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Ensure uploads folder exists for ID documents
var uploadsPath = Path.Combine(
    app.Environment.WebRootPath, "uploads", "ids");
if (!Directory.Exists(uploadsPath))
    Directory.CreateDirectory(uploadsPath);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();