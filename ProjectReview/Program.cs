using ProjectReview.Interfaces;
using ProjectReview.Models;
using ProjectReview.Repositories;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MyCnn") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
// Đăng ký các dịch vụ ở đây
builder.Services.AddScoped<PROJECTREVIEWContext>(); // Đăng ký DbContext
builder.Services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>)); // Đăng ký IRepository và RepositoryBase
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ILocationTypeRepository, LocationTypeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();


// Tích hợp dịch vụ xác thực
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})

 .AddGoogle("Google", options =>
 {
     options.ClientId = "23974197200-4tf5rk8tr24fuoh1punogepc7pnhl11p.apps.googleusercontent.com";
     options.ClientSecret = "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";
 });


var app = builder.Build();

// Cấu hình pipeline HTTP request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
