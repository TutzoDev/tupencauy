using Microsoft.EntityFrameworkCore;
using tupencauy.Data;
using tupencauy.Models;
using tupencauywebapi.DTOs;

namespace tupencauywebapi.Services
{
    public class ServicioEventoDeportivo : IServicioEventoDeportivo
    {
        private readonly AppDbContext _context;

        public ServicioEventoDeportivo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventoDeportivoDTO>> GetEventosDeportivosAsync()
        {
            var eventos = await _context.EventosDeportivos.ToListAsync();

            return eventos.Select(e => EventoToDto(e));
        }

        public EventoDeportivoDTO EventoToDto(EventoDeportivo evento)
        { 
            return new EventoDeportivoDTO
            {
                Id = evento.Id,
                Nombre = evento.Nombre,
                FechaInicio = evento.FechaInicio,
                FechaFin = evento.FechaFin,
            };
        }

        public UnoVsUnoDTO UnoVsUnoToDTO(EventoDeportivo evento)
        {
            if (evento is UnoVsUno unoVsUno)
            {
                return new UnoVsUnoDTO
                {
                    Id = unoVsUno.Id,
                    Nombre = unoVsUno.Nombre,
                    FechaInicio = unoVsUno.FechaInicio,
                    FechaFin = unoVsUno.FechaFin,
                    EquipoUno = unoVsUno.EquipoUno,
                    EquipoDos = unoVsUno.EquipoDos,
                    ScoreUno = unoVsUno.ScoreUno,
                    ScoreDos = unoVsUno.ScoreDos,
                    Deporte = unoVsUno.Deporte
                };
            }
            throw new ArgumentException("El evento proporcionado no es del tipo UnoVsUno");
        }
      
    }
}
