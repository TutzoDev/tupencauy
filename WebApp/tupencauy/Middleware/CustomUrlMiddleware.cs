using Microsoft.EntityFrameworkCore;
using tupencauy.Data;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using tupencauy.Models;



namespace tupencauy.Middleware
{
    public class CustomUrlMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomUrlMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            var path = context.Request.Path.Value?.Trim('/');
            Console.WriteLine($"Interceptando la ruta: {path}");


            if (!string.IsNullOrEmpty(path))
            {

                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var customUrl = await dbContext.Sitios
                        .FirstOrDefaultAsync(c => c.Url == path);

                    if (customUrl != null)
                    {
                        context.Session.SetString("TenantId", customUrl.TenantId);                       
                        // Redirige internamente a la acción del controlador 
                        context.Request.Path = $"/Account/Login";
                    }
                    else
                    {
                        Console.WriteLine("No se encontró el sitio para la URL especificada.");
                    }
                }
         
            }
            await _next(context);
        }
    }
}
