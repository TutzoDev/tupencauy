namespace tupencauy.Models
{
    public class Recarga
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string IdUsuario { get; set; }
        public double Carga { get; set; }
        public string ComprobantePath { get; set; }
        public bool? Aprobado { get; set; }
        public DateTime Tmstmp { get; set; }
    }
}
