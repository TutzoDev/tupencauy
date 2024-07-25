namespace tupencauy.Models
{
    public class Prediccion
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string IdPencaSitioUsuario { get; set; }
        public string IdUnoVsUno { get; set; }
        public int ScoreTeam1 { get; set; }
        public int ScoreTeam2 { get; set; }

        //      le pondria un status para usarlo de bandera:
        //      mientras este true se puede predecir:
        //      se actualizan todos a la hora que inicia el partido a false

        //public bool? Status { get; set; }

        //      el puntaje se carga luego que el partido culmino:
        //      si acerto ambos score se le otorga 8 puntos
        //      si acerto la diferencia se le otorga 5 puntos
        //      si acerto el ganador solo se le otorga 3 puntos

        //public int puntaje { get; set; }
    }
}