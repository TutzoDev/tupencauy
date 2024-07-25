using Microsoft.AspNetCore.Mvc.ModelBinding;
using tupencauy.Models;

namespace tupencauy.ViewModels
{
    public class PencaEventoVM
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public string SelectedEventsId { get; set; }
        public List<Evento> Eventos { get; set; } = new List<Evento>();
    }

    public class Evento
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string EquipoUno { get; set; }
        public string EquipoDos { get; set; }
        public DateTime Fecha { get; set; }
    }


}





