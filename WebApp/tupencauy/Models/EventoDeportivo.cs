using System.ComponentModel.DataAnnotations;

namespace tupencauy.Models
{
    public class EventoDeportivo
    {
        public string Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public ICollection<Penca> Pencas { get; set; }
    }
}