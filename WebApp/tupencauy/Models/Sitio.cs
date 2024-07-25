using System.ComponentModel.DataAnnotations;

namespace tupencauy.Models
{
    public class Sitio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Url { get; set; }
        public string TipoRegistro { get; set; }
        public string ColorPrincipal { get; set; }
        public string ColorSecundario { get; set; }
        public string ColorTipografia { get; set; }
        public bool? Status { get; set; }
        public int cantidadUsuarios { get; set; }
        public string TenantId { get; set; }

    }
}
