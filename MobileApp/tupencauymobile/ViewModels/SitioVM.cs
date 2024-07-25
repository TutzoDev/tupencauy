using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tupencauymobile.Models;
using tupencauymobile.Services;
using tupencauymobile.Views;

namespace tupencauymobile.ViewModels
{
    public partial class SitioVM : BaseViewModel
    {
        ServicioSitio servicioSitio;

        public ObservableCollection<Sitio> Sitios { get; } = new();

        private Sitio _sitioSeleccionado;
        public Sitio SitioSeleccionado
        {
            get => _sitioSeleccionado;
            set
            {
                _sitioSeleccionado = value;
                OnPropertyChanged();
            }
        }

        public SitioVM(ServicioSitio servicioSitio)
        {
            this.servicioSitio = servicioSitio;
        }

        [RelayCommand]
        async Task GetSitiosAsync()
        {
            if (Ocupado)
                return;
            try
            {
                Ocupado = true;
                var sitios = await servicioSitio.GetSitios();

                if (Sitios.Count() != 0)
                {
                    Sitios.Clear();
                }

                foreach (var sitio in sitios)
                {
                    Sitios.Add(sitio);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Ha habido un error!", $"No hemos podido traer las pencas : {ex.Message}", "OK");
            }
            finally
            {
                Ocupado = false;
            }
        }
    }
}
