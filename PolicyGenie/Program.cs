using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OIF.Cams.Business.AutoMapping.Account;
using OIF.Cams.Business.AutoMapping.Lead;
using OIF.Cams.Business.AutoMapping.MasterDataManagement;
using OIF.Cams.Business.AutoMapping.ServiceRequest;
using OIF.Cams.Data.DAC;
using OIF.Cams.Data.Repository.Account;
using OIF.Cams.Data.Repository.Lead;
using OIF.Cams.Data.Repository.MasterDataManagement;
using OIF.Cams.Data.Repository.ServiceRequest;
using OIF.Cams.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CamsDbContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.SignIn.RequireConfirmedAccount = false; // Set to true in production for security
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.User.RequireUniqueEmail = true; 
}).AddEntityFrameworkStores<CamsDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IAccountRepo, AccountRepo>();
builder.Services.AddScoped<IServiceRequestRepo, ServiceRequestRepo>();
builder.Services.AddScoped<IMasterDataManagementRepo, MasterDataManagementRepo>();
builder.Services.AddScoped<ILeadRepo, LeadRepo>();
builder.Services.AddScoped<IAccountAutoMapper, AccountAutoMapper>();
builder.Services.AddScoped<IServiceRequestAutoMapper, ServiceRequestAutoMapper>();
builder.Services.AddScoped<IMasterDataManagementAutoMapper, MasterDataManagementAutoMapper>();
builder.Services.AddScoped<ILeadAutoMapper, LeadAutoMapper>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10); 
    options.SlidingExpiration = true; 
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});
builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.Password.RequireDigit = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireNonAlphanumeric = true;
    opt.Password.RequiredLength = 8;
    opt.User.RequireUniqueEmail = true;
    // ? Lockout settings
    opt.Lockout.MaxFailedAccessAttempts = 3;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(365 * 100); // ~forever
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<CamsDbContext>();
    if (!context.Users.Any()) // Or any other check
    {
        //await SeedService.SeedDataBase(services);
        // Run async code in sync context
        SeedService.SeedDataBase(services).GetAwaiter().GetResult();
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
