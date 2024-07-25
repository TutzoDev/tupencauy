namespace tupencauywebapi.Models
{
    public class ActualizarResultadosRequest
    {
        public List<ActualizarResultadoRequest> Partidos { get; set; }
    }

    public class ActualizarResultadoRequest
    {
        public string Id { get; set; }
        public string ScoreUno { get; set; }
        public string ScoreDos { get; set; }
    }

}
