using System.ComponentModel.DataAnnotations;

namespace tupencauy.Models
{
    public class Penca
    {
        public string Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public DateTime FechaInicio { get; set; }
        [Required]
        public DateTime FechaFin { get; set; }
        public bool IsFinish { get; set; }
        public ICollection<EventoDeportivo> EventosDeportivos { get; set; }
    }
}