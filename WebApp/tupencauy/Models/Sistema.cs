using System.ComponentModel.DataAnnotations;

namespace tupencauy.Models
{
    public class Sistema
    {
        public string Id { get; set; }
        [Range(0, 100, ErrorMessage = "La comisión es un %, debe estar entre 0 y 100.")]
        public double Comision { get; set; }
        public double Billetera { get; set; }
        public int TiempoNotificaciones { get; set; }
    }
}
