using System.ComponentModel.DataAnnotations;

namespace tupencauywebapi.DTOs
{
    public class EventoDeportivoDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
