using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using tupencauywebapi.Services;
using tupencauy.Data;
using tupencauy.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;
using tupencauywebapi.SignalR.Hubs;
using tupencauywebapi.SignalR.Servicios;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
services.AddEndpointsApiExplorer();
services.AddControllers();
services.AddSignalR();
services.AddHttpClient(); // Registrar IHttpClientFactory

services.AddScoped<IServicioNotificacionPush, ServicioNotificacionPush>();
services.AddScoped<IServicioAuth, ServicioAuth>();
services.AddScoped<IServicioPenca, ServicioPenca>();
services.AddScoped<IServicioEventoDeportivo, ServicioEventoDeportivo>();
services.AddScoped<IServicioUser, ServicioUser>();
services.AddScoped<IServicioSitio, ServicioSitio>();
services.AddScoped<IServicioPrediccion, ServicioPrediccion>();

services.AddScoped<INotificacionService, NotificacionService>();
services.AddScoped<IAvisoService, AvisoService>();
services.AddHostedService<RevisionNotificaciones>();

services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString));

services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Add authentication
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
});
//.AddGoogle(options =>
//{
//    options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
//    options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
//});

// Configuración de CORS para permitir cualquier origen
services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAllOrigins"); // Mover CORS aquí
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(); // Agrega esta línea para servir archivos estáticos

app.MapHub<NotificacionHub>("/notificacionHub");
app.MapHub<ChatHub>("/chatHub");

app.MapControllers();

app.Run("http://0.0.0.0:5234");
