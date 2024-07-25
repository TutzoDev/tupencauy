using tupencauy.Models;
using tupencauywebapi.DTOs;

namespace tupencauywebapi.Services
{
    public interface IServicioEventoDeportivo
    {
        Task<IEnumerable<EventoDeportivoDTO>> GetEventosDeportivosAsync();
        EventoDeportivoDTO EventoToDto(EventoDeportivo evento);
        UnoVsUnoDTO UnoVsUnoToDTO(EventoDeportivo evento);
    }
}