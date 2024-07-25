namespace tupencauywebapi.DTOs
{
    public class SitioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Url { get; set; }
        public string TipoRegistro { get; set; }
        public bool? Status { get; set; }
        public string TenantId { get; set; }
        public string ColorPrincipal { get; set; }
        public string ColorSecundario { get; set; }
        public string ColorTipografia { get; set; }
    }
}