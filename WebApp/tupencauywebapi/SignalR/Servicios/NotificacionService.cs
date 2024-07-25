using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using tupencauy.Data;
using tupencauy.Models;
using tupencauywebapi.SignalR.Hubs;

namespace tupencauywebapi.SignalR.Servicios
{
    public interface INotificacionService
    {
        Task CrearNotificacion(string usuarioId, string mensaje, int sitioId);
    }

    public class NotificacionService : INotificacionService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificacionHub> _hubContext;

        public NotificacionService(AppDbContext context, IHubContext<NotificacionHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task CrearNotificacion(string usuarioId, string mensaje, int sitioId)
        {
            try
            {
                var notificacion = new Notificacion
                {
                    UsuarioId = usuarioId,
                    Mensaje = mensaje,
                    FechaCreacion = DateTime.Now,
                    FechaEnvio = DateTime.Now,
                    Leida = false,
                    SitioId = sitioId
                };


                // Enviar notificación en tiempo real
                await _hubContext.Clients.All.SendAsync("RecibirNotificacion", mensaje);

                _context.Notificaciones.Add(notificacion);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error al actualizar la base de datos: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw; // Re-lanzar la excepción si es necesario
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> MarcarTodasComoLeidas()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var notificaciones = await _context.Notificaciones
        //        .Where(n => n.UsuarioId == userId && !n.Leida)
        //        .ToListAsync();

        //    foreach (var notificacion in notificaciones)
        //    {
        //        notificacion.Leida = true;
        //    }

        //    await _context.SaveChangesAsync();

        //    return Ok();
        //}
    }
}
