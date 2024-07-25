using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tupencauy.Data;
using tupencauywebapi.DTOs;

namespace tupencauywebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificacionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Notificacion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificacionDTO>>> GetNotificaciones()
        {
            var notificacionesRet = _context.Notificaciones;

            if(notificacionesRet == null)
            {
                return NotFound(); 
            }
            return Ok(notificacionesRet);
        }

        // GET: api/Notificacion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificacionDTO>> GetNotificacion(int id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);

            if (notificacion == null)
            {
                return NotFound();
            }

            NotificacionDTO notificacionRet = new NotificacionDTO
            {
                Id = notificacion.Id,
                UsuarioId = notificacion.UsuarioId,
                Mensaje = notificacion.Mensaje,
                FechaCreacion = notificacion.FechaCreacion,
                Leida = notificacion.Leida
            };

            return notificacionRet;
        }

        [HttpGet("{userId}/Notificaciones")]
        public async Task<ActionResult<IEnumerable<NotificacionDTO>>> GetNotificacionesUser(string userId)
        {
            var notificaciones = await _context.Notificaciones
                .Where(n => n.UsuarioId == userId)
                .ToListAsync();

            return Ok(notificaciones);
        }


        // DELETE: api/Notificacion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotificacion(int id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion == null)
            {
                return NotFound();
            }

            _context.Notificaciones.Remove(notificacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
