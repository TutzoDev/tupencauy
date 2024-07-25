using tupencauy.Models;

namespace tupencauy.ViewModels
{
    public class DashboardAdminVM
    {
        public int TotalUsuarios { get; set; }
        public double RecaudacionSitio { get; set; }
        public string Nombre { get; set; }
        public string TipoRegistro { get; set; }
        public int UsuariosDisponibles { get; set; }
        public List<EstadisticasPorPenca> EstadisticasPorPenca { get; set; }

    }

    public class EstadisticasPorPenca
    {
        public string PencaId { get; set; }
        public string PencaNombre { get; set; }
        public int Inscriptos { get; set; }
        public int MinimosUsuarios { get; set; }
        public double Premio { get; set; }
        public double Costo { get; set; }
        public double Recaudacion { get; set; }
        public int TotalPredicciones { get; set; }
        public double PorcentajeAciertos { get; set; }
    }

}