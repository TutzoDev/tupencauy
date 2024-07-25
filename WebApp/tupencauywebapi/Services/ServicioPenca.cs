using Microsoft.EntityFrameworkCore;
using tupencauy.Data;
using tupencauywebapi.DTOs;
using tupencauywebapi.Models;

namespace tupencauywebapi.Services
{
    public class ServicioPenca : IServicioPenca
    {
        readonly AppDbContext _context;
        readonly IServicioEventoDeportivo _servicioEventoDeportivo;
        public ServicioPenca(AppDbContext context, IServicioEventoDeportivo servicioEventoDeportivo)
        {
            _context = context;
            _servicioEventoDeportivo = servicioEventoDeportivo;
        }

        public async Task<List<UnoVsUnoDTO>> GetEventosUnoVsUnoByPencaId(string id)
        {
            var eventosPencaDb = await _context.UnoVsUno.Where(ev => ev.Pencas.Any(p => p.Id == id)).ToListAsync();
            List<UnoVsUnoDTO> eventosReturn = [];

            foreach (var ev in eventosPencaDb)
            {
                var eventoFetch = _servicioEventoDeportivo.UnoVsUnoToDTO(ev);
                eventosReturn.Add(eventoFetch);
            }
            return eventosReturn;
        }
    }
}
