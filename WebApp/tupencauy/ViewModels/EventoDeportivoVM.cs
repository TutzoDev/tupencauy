using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace tupencauy.ViewModels
{
    public class EventoDeportivoVM
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        //luego modificar para que tome de un combo
        // Tipo de evento
        [Display(Name = "Tipo de Evento")]
        public TipoEvento TipoDeEvento { get; set; }

        // Uno vs uno
        public string EquipoUno { get; set; }
        public string EquipoDos { get; set; }

        // Tipo de deporte
        [Display(Name = "Tipo de Deporte")]
        public Deporte TipoDeDeporte { get; set; }

        public enum Deporte { Futbol, Basquetbol, Boxeo, Handbol, Voleibol }
        public enum TipoEvento { UnoVsUno , FreeForAll }
    }

}
