
namespace tupencauymobile.Models
{
    public class PrediccionReq
    {
        public string IdPencaSitioUsuario { get; set; }
        public string tenantId { get; set; }
        public string userId { get; set; }
        public string pencaId { get; set; }
        public string IdEvento { get; set; }
        public int ScoreTeamUno { get; set; }
        public int ScoreTeamDos { get; set;}
    }
}
