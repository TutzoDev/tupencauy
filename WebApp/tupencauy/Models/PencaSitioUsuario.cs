namespace tupencauy.Models
{
    public class PencaSitioUsuario
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string IdPencaSitio { get; set; }
        public string IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public int  Puntaje { get; set; }
        public int Aciertos { get; set; }
        public ICollection<Prediccion> Predicciones { get; set; }
    }
}