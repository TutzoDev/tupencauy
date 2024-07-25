using System.ComponentModel.DataAnnotations;
using tupencauy.Models;

namespace tupencauywebapi.DTOs
{
    public class PencaDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public double CostoPenca { get; set; }
        public bool Inscrito { get; set; }
    }
}
