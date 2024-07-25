// NotificacionHub.cs
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using tupencauy.Data;
using tupencauy.Models;

namespace tupencauywebapi.SignalR.Hubs
{
    public class NotificacionHub : Hub
    {
        private readonly AppDbContext _context;

        public NotificacionHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task EnviarNotificacion(string usuarioId)
        {
            var notificacionAEnviarAlFrontend = await _context.Notificaciones.Where(n => n.UsuarioId == usuarioId).ToListAsync();

            foreach(var notificacion in notificacionAEnviarAlFrontend)
            {
                await Clients.All.SendAsync("RecibirNotificacion", notificacion.Mensaje);
            }
        }
    }
}