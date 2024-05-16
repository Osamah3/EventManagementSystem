using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // For IdentityUser
using Trion.Models;
using Trion.Data;
using static System.Formats.Asn1.AsnWriter;





var builder = WebApplication.CreateBuilder(args);




// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();



builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddDefaultUI()
     .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>(TokenOptions.DefaultProvider)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
});

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.
        GetRequiredService<UserManager<IdentityUser>>();

    var roleManager = scope.ServiceProvider.
        GetRequiredService<RoleManager<IdentityRole>>();

    if (!roleManager.RoleExistsAsync("Admin").Result)
    {
        IdentityRole role = new IdentityRole("Admin");
        IdentityResult roleResult = roleManager.CreateAsync(role).Result;
    }

    if (userManager.FindByEmailAsync("trion@trion.com").Result == null)
    {
        IdentityUser user = new IdentityUser
        {
            UserName = "trion@trion.com",
            Email = "trion@trion.com",
        };

        IdentityResult result = userManager.CreateAsync(user, "Trion@123").Result;

        if (!await userManager.IsInRoleAsync(user, "Admin"))
        {
            // Assign the "Admin" role to the user
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }

   
}












// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}




app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();



app.Run();
