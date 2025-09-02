using Hendry_Auto.Application.Contracts.Persistence;
using Hendry_Auto.Infrastructure.Common;
using Hendry_Auto.Infrastructure.Repositories;
using Hendry_Auto.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Hendry_Auto.Application.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Hendry_Auto.Application.Services.Interface;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDbContext<Hendry_Auto.Infrastructure.Common.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Host.UseSerilog((Context,config)=>
{
    config.WriteTo.File("Logs/Log.txt", rollingInterval: RollingInterval.Day);
    if(Context.HostingEnvironment.IsProduction()==false)
    {
        config.WriteTo.Console();
    }
});

builder.Services.AddScoped<IUserNameService, UserNameService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

#region configuration for seeding data to database

static async void UpdateDatabaseAsync(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();//context.Database.EnsureDeleted();
            if(context.Database.IsSqlServer())
            {
                context.Database.Migrate();
            }
            
            await SeedData.SeedDataAsync(context);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        }
    }       
    
}

#endregion

var app = builder.Build();

var ServiceProvider = app.Services;
await SeedData.SeedRole(ServiceProvider);

UpdateDatabaseAsync(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapRazorPages();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=customer}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
