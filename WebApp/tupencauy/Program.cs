using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tupencauy.Data;
using tupencauy.Middleware;
using tupencauy.Models;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString));
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";
});
builder.Services.AddIdentity<AppUser, IdentityRole>(
    options =>
    {
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireUppercase = false; //REQUERIR MAYUSCULA?
        options.Password.RequiredLength = 8; //TAMAÑO MINIMO
        options.Password.RequireNonAlphanumeric = false; //REQUERIR CARACTER ESPECIAL
        options.Password.RequireLowercase = false; // REQUERIR MINUSCULA?
    })
    .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

//builder.Services.AddAuthentication()
//    .AddGoogle(options =>
//    {
//        options.ClientId = configuration["GoogleKeys:ClientId"];
//        options.ClientSecret = configuration["GoogleKeys:ClientSecret"];
//    });

builder.Services.AddSession();
builder.Services.AddControllers(); // controladores de API
builder.Services.AddControllersWithViews();


var app = builder.Build();

app.UseSession();
// Crear los roles al inicio de la aplicación
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

    // Lista de roles que deseas agregar
    var roles = new[] { "SuperAdmin", "Admin", "Usuario" };

    foreach (var roleName in roles)
    {
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            // Crear el rol si no existe
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}


app.UseMiddleware<CustomUrlMiddleware>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
