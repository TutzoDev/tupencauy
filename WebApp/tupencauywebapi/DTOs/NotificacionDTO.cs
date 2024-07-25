namespace tupencauywebapi.DTOs
{
    public class NotificacionDTO
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Leida { get; set; }
    }
}
