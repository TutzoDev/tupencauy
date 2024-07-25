namespace tupencauy.Models
{
    public class UnoVsUno : EventoDeportivo
    {
        public string EquipoUno { get; set; }

        public string EquipoDos { get; set; }

        public string? HoraPartido { get; set; }

        public string[]? Canales { get; set; }

        public string ScoreUno { get; set; }

        public string ScoreDos { get; set; }

        public string Deporte { get; set; }

        public bool ResultadoNotificado { get; set; }
    }
}
