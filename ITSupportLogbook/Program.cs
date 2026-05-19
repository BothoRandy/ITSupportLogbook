using ITSupportLogbook.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using ITSupportLogbook.Models.Entities;
using ITSupportLogbook.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var provider = builder.Configuration["DatabaseProvider"];

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (provider == "Postgres")
    {
        var conn = builder.Configuration.GetConnectionString("Postgres");

        options.UseNpgsql(conn);
    }
    else
    {
        var conn = builder.Configuration.GetConnectionString("SqlServer");

        options.UseSqlServer(conn, b =>
            b.MigrationsAssembly("ITSupportLogbook.SqlServerMigrations"));
    }
});
// Identity (Login/Register)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie redirect settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

await DbInitializer.SeedAdminAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Default route goes to Login page first
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();