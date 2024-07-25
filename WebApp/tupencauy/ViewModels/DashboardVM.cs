namespace tupencauy.ViewModels
{
    public class DashboardVM
    {
        public int TotalUsuarios { get; set; }
        public int TotalSitios { get; set; }
        public decimal SaldoSistema { get; set; }
        public List<EstadisticasSitios> EstadisticasSitios { get; set; }
        public List<EstadisticasPencas> EstadisticasPencas { get; set; }
    }

    public class EstadisticasSitios
    {
        public string NombreSitio { get; set; }
        public int CantidadUsuarios { get; set; }
        public double Recaudacion { get; set; }
    }

    public class EstadisticasPencas
    {
        public string NombrePenca { get; set; }
        public double Costo { get; set; }
        public double Premio { get; set; }
        public int CantidadUsuarios { get; set; }
        public double Recaudacion { get; set; }
    }
}