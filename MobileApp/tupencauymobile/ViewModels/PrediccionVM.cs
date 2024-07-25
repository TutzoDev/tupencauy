using Android.Content.Res;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using System.Diagnostics;
using tupencauymobile.Models;
using tupencauymobile.Services;
using tupencauymobile.Views;

namespace tupencauymobile.ViewModels
{
    public partial class PrediccionVM : BaseViewModel
    {
        private ServicioPrediccion servicioPrediccion { get; set; }

        string tenantId;
        string pencaId;
        string userId;
        string eventoId;
        int golesTeamUno;
        int golesTeamDos;

        [ObservableProperty]
        private string idEvento;

        [ObservableProperty]
        private string equipoUno;

        [ObservableProperty]
        private string equipoDos;

        [ObservableProperty]
        private int? scoreUno;

        [ObservableProperty]
        private int? scoreDos;

        [ObservableProperty]
        private bool realizada;

        [ObservableProperty]
        private bool noRealizada;

        //[ObservableProperty]
        //private string pencasitioId;

        //[ObservableProperty]
        //private string pencasitiousuarioId;

        public PrediccionVM(ServicioPrediccion servicioPrediccion) {
            Titulo = "Prediccion";
            this.servicioPrediccion = servicioPrediccion;
            ScoreUno = null;
            ScoreDos = null;
    }

        public async Task Inicializar(string tenantId, string pencaId, string eventoId, string userId)
        {
            this.tenantId = tenantId;
            this.pencaId = pencaId;
            this.eventoId = eventoId;
            this.userId = userId;
            await GetPrediccionAsync();
        }

        public async Task EnviarPrediccion(string idEvento, int golesEquipoUno, int golesEquipoDos, bool yaPredijoAntes)
        {
            this.eventoId = idEvento;
            this.golesTeamUno = golesEquipoUno;
            this.golesTeamDos = golesEquipoDos;
            if (!yaPredijoAntes)
            {
                await PostPrediccionAsync();
            }
            else
            {
                await EditarPrediccionAsync();
            }
        }

        [RelayCommand]
        async Task GetPrediccionAsync()
        {
            if (Ocupado)
                return;
            try
            {
                Ocupado = true;

                var prediccionEvento = await servicioPrediccion.GetPrediccionDisplay(tenantId, pencaId, eventoId, userId);

                IdEvento = prediccionEvento.IdEvento;
                EquipoUno = prediccionEvento.EquipoUno;
                EquipoDos = prediccionEvento.EquipoDos;
                ScoreUno = prediccionEvento.ScoreTeamUno;
                ScoreDos = prediccionEvento.ScoreTeamDos;
                Realizada = prediccionEvento.Realizada;
                NoRealizada = !prediccionEvento.Realizada;
                Preferences.Set("psuId", prediccionEvento.IdPencaSitioUsuario);
                //PencasitioId = evento.IdPencaSitio;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await Shell.Current.DisplayAlert("Ha habido un error!", $"No hemos podido mostrar el evento : {ex.Message}", "OK");
            }
            finally
            {
                Ocupado = false;
            }
        }

        [RelayCommand]
        async Task PostPrediccionAsync()
        {
           var resultado = await servicioPrediccion.PostPrediccionDisplay(Preferences.Get("psuId", string.Empty), eventoId, this.golesTeamUno, this.golesTeamDos);
            if (resultado.Success)
            {
                await Shell.Current.DisplayAlert("Prediccion", "Tu prediccion se ha realizado correctamente!", "OK");
                await Shell.Current.Navigation.PopModalAsync();
            }
            
        }

        [RelayCommand]
        async Task EditarPrediccionAsync()
        {
            var resultado = await servicioPrediccion.EditarPrediccionDisplay(Preferences.Get("psuId", string.Empty), eventoId, this.golesTeamUno, this.golesTeamDos);
            if (resultado.Success)
            {
                await Shell.Current.DisplayAlert("Prediccion", "Tu prediccion se ha editado correctamente!", "OK");
                await Shell.Current.Navigation.PopModalAsync();
            }

        }
    }
}
