using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using tupencauymobile.Models;
using tupencauymobile.Services;
using tupencauymobile.Views;

namespace tupencauymobile.ViewModels
{
    public partial class PencaVM : BaseViewModel
    {
        ServicioPenca servicioPenca;

        public ObservableCollection<Penca> Pencas { get; } = new();

        public PencaVM(ServicioPenca servicioPenca)
        {
            Titulo = "Pencas";
            this.servicioPenca = servicioPenca;
        }

        [RelayCommand]
        async Task OnPencaClickedAsync(string id)
        {
            var servicioPencaDisplay = new ServicioPencaDisplay(Preferences.Get("jwt", string.Empty), Preferences.Get("tenantId", string.Empty));
            var pencaDisplayVM = new PencaDisplayVM(servicioPencaDisplay);
            pencaDisplayVM.Inicializar(id);
            await Shell.Current.Navigation.PushAsync(new PencaDisplay(pencaDisplayVM));
        }

        [RelayCommand]
        async Task GetPencasAsync()
        {
            if (Ocupado)
                return;
            try
            {
                Ocupado = true;
                var pencas = await servicioPenca.GetPencas();

                if (Pencas.Count() != 0)
                {
                    Pencas.Clear();
                }

                foreach (var penca in pencas)
                {
                    //if(penca.Inscrito)
                        Pencas.Add(penca);
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
