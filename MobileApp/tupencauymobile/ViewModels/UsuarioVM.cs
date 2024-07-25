using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using tupencauymobile.Services;

namespace tupencauymobile.ViewModels
{
    public partial class UsuarioVM : BaseViewModel
    {
        private ServicioUsuario servicioUser { get; set; }

        [ObservableProperty]
        private string nombreUsuario;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private bool _areNotificationsEnabled = true;

        [ObservableProperty]
        private bool _isButton1Enabled = true;

        [ObservableProperty]
        private bool _isButton2Enabled = true;

        [ObservableProperty]
        private bool _isButton3Enabled = true;

        [ObservableProperty]
        private bool _isSaving;


        public UsuarioVM(ServicioUsuario servicioUser)
        {
            Titulo = "Perfil usuario";
            this.servicioUser = servicioUser;
        }

        [RelayCommand]
        async Task GetUserAsync()
        {
            if (Ocupado)
                return;
            try
            {
                Ocupado = true;

                string userId = Preferences.Get("userId", string.Empty);

                var userReturn = await servicioUser.GetUsuario(userId);

                NombreUsuario = userReturn.Nombre;
                Username = userReturn.UserName;
                Email = userReturn.Email;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await Shell.Current.DisplayAlert("Ha habido un error!", $"No hemos podido cargar tu perfil : {ex.Message}", "Cerrar");
            }
            finally
            {
                Ocupado = false;
            }
        }

        partial void OnAreNotificationsEnabledChanged(bool habilitadas)
        {
            UpdateButtonsState();
            CambioEstadoNotificacionesAsync(habilitadas);
        }

        private void UpdateButtonsState()
        {
            IsButton1Enabled = AreNotificationsEnabled;
            IsButton2Enabled = AreNotificationsEnabled;
            IsButton3Enabled = AreNotificationsEnabled;
        }

        private async void CambioEstadoNotificacionesAsync(bool habilitadas)
        {
            if (IsSaving) return;

            try
            {
                IsSaving = true;

                string userId = Preferences.Get("userId", string.Empty);
                bool success = await servicioUser.ActivarDesactivarNotificaciones(userId, habilitadas);

                if (!success)
                {
                    // Si la actualización falla, revertimos el cambio
                    AreNotificationsEnabled = !habilitadas;
                    await Shell.Current.DisplayAlert("Error", "No se pudo actualizar la configuración de notificaciones", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error al actualizar las notificaciones: {ex.Message}", "OK");

                // Revertimos el cambio en caso de error
                AreNotificationsEnabled = !habilitadas;
            }
            finally
            {
                IsSaving = false;
            }
        }
    }
}
