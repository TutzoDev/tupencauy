namespace tupencauywebapi.DTOs
{
    public class AppUserDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string TenantId { get; set; }
        public int Puntaje { get; set; }
        public int Aciertos { get; set; }
        public double Saldo { get; set; }
    }
}
