
namespace tupencauymobile.Models
{
    public class Prediccion
    {
        //public string IdPencaSitio { get; set; }
        public string IdPencaSitioUsuario { get; set; }
        public string IdEvento { get; set; }
        public string EquipoUno { get; set; }
        public string EquipoDos { get; set; }
        public int? ScoreTeamUno { get; set; }
        public int? ScoreTeamDos { get; set; }
        public bool Realizada { get; set; }
    }
}
