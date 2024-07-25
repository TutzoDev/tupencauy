using tupencauy.Models;

namespace tupencauy.ViewModels
{
    public class DetallePencaVM
    {
        public string Nombre { get; set; }
        public List<DatosUsuario> Usuarios { get; set; }

    }

    public class DatosUsuario
    {
        public string Nombre { get; set; }
        public int Puntaje { get; set; }
        public int Aciertos { get; set; }

    }
}
