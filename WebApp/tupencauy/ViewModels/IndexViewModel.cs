using tupencauy.Models;

namespace tupencauy.ViewModels
{
    public class IndexViewModel
    {
        public List<Sitio> Sitios { get; set; }
        public List<NotificacionViewModel> Notificaciones { get; set; }
    }

    public class NotificacionViewModel
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string SitioNombre { get; set; }

        public int SitioId { get; set; }
        public bool Leida { get; set; }
    }
}