using NuGet.Versioning;
using tupencauy.Models;

namespace tupencauy.ViewModels
{
    public class PencaSitioVM
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public double Premio { get; set; }
        public double Costo { get; set; }
        public string SelectedPencaId { get; set; }
        public string SitioTenantId { get; set; }
        public int Inscriptos { get; set; }
        public double Recaudacion { get; set; }
        public List<Penca> Pencas { get; set; } = new List<Penca>();
    }
}
