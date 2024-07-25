using tupencauy.Models;

namespace tupencauy.ViewModels
{
    public class SistemaVM
    {
        public string Id { get; set; }
        public double Comision { get; set; }
        public double Billetera { get; set; }
        public int Notificaciones { get; set; } // Intervalo de tiempo en minutos para enviar notificaciones

        public List<infoSitio> sitios { get; set; } = new List<infoSitio>();

    }

    public class infoSitio 
        {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int cantidadUsuarios { get; set; }
    }
}
