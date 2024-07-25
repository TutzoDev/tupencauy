using CommunityToolkit.Mvvm.Input;
using tupencauymobile.Models;
using tupencauymobile.Services;
using System.Diagnostics;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using tupencauymobile.Views;
using Android.Content.Res;

namespace tupencauymobile.ViewModels
{
    public partial class PencaDisplayVM : BaseViewModel
    {
        private readonly ServicioPencaDisplay servicioPencaDisplay;

        private string pencaId;

        public ObservableCollection<UnoVsUno> Eventos { get; } = new();

        public ObservableCollection<PosicionPenca> Posiciones { get; } = new();

        public PencaDisplayVM(ServicioPencaDisplay servicioPencaDisplay)
        {
            this.servicioPencaDisplay = servicioPencaDisplay;
            //CargarDatosHardcodeados();
        }

        private void CargarDatosHardcodeados()
        {
            // Cargar 10 eventos (UnoVsUno)
            for (int i = 1; i <= 10; i++)
            {
                Eventos.Add(new UnoVsUno
                {
                    Id = $"Evento{i}",
                    EquipoUno = $"Equipo A{i}",
                    EquipoDos = $"Equipo B{i}",
                    ScoreUno = $"{i}",
                    ScoreDos = $"{i - 1}",
                    Deporte = "Fútbol",
                    Nombre = $"Partido {i}",
                    FechaInicio = DateTime.Now.AddDays(i),
                    FechaFin = DateTime.Now.AddDays(i + 1)
                });
            }

            // Cargar 10 posiciones (PosicionPenca)
            for (int i = 1; i <= 9; i++)
            {
                Posiciones.Add(new PosicionPenca
                {
                    posicion = i,
                    NombreUsuario = $"Usuario{i}",
                    Aciertos = 30 - (i * 2),
                    Puntaje = 50 - (i*3)
                });
            }
        }

        public async void Inicializar(string id)
        {
            pencaId = id;
            await GetPencaDisplayAsync();
            await GetPosicionesAsync();
        }

        [RelayCommand]
        async Task OnEventoClickedAsync(string eventoId)
        {
            try
            {
                var servicioPrediccion = new ServicioPrediccion(Preferences.Get("jwt", string.Empty));
                var prediccionVM = new PrediccionVM(servicioPrediccion);
                await prediccionVM.Inicializar(Preferences.Get("tenantId", string.Empty), pencaId, eventoId, Preferences.Get("userId", string.Empty));
                await Shell.Current.Navigation.PushModalAsync(new PrediccionDisplay(prediccionVM), true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en OnEventoClickedAsync: {ex.Message}");
            }           
        }

        [RelayCommand]
        async Task GetPencaDisplayAsync()
        {
            if (Ocupado)
                return;
            try
            {
                Ocupado = true;
                var eventosPenca = await servicioPencaDisplay.GetPencaDisplay(pencaId);

                if (Eventos.Count != 0)
                {
                    Eventos.Clear();
                }

                foreach (var ev in eventosPenca)
                {

                    Eventos.Add(ev);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await Shell.Current.DisplayAlert("Ha habido un error!", $"No hemos podido mostrar la penca : {ex.Message}", "OK");
            }
            finally
            {
                Ocupado = false;
            }
        }

        [RelayCommand]
        async Task GetPosicionesAsync()
        {
            if (Ocupado)
                return;
            try
            {
                Ocupado = true;
                var posicionesPenca = await servicioPencaDisplay.GetPosicionesPencaSitio(pencaId);

                if (Posiciones.Count != 0)
                {
                    Posiciones.Clear();
                }

                foreach (var posicion in posicionesPenca)
                {

                    Posiciones.Add(posicion);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await Shell.Current.DisplayAlert("Ha habido un error!", $"No hemos podido mostrar la penca : {ex.Message}", "OK");
            }
            finally
            {
                Ocupado = false;
            }
        }
    }
}
