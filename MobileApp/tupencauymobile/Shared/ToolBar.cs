using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using tupencauymobile.Services;
using tupencauymobile.ViewModels;
using tupencauymobile.Views;

namespace tupencauymobile.Shared
{
    public class ToolBar : ContentPage
    {
        private readonly HttpClient _httpClient;

        public ToolBar()
        {
            _httpClient = new HttpClient(); // Puedes inyectar esto usando un IoC container si lo prefieres
            InicializarToolbar();
        }

        private void InicializarToolbar()
        {
            var profileToolbarItem = new ToolbarItem
            {
                IconImageSource = "profileimage.png",
                Order = ToolbarItemOrder.Primary,
                Priority = 1
            };
            profileToolbarItem.Clicked += OnProfileClicked;

            var notificationToolbarItem = new ToolbarItem
            {
                IconImageSource = "notificacion.png",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            notificationToolbarItem.Clicked += OnNotificationClicked;

            ToolbarItems.Add(profileToolbarItem);
            ToolbarItems.Add(notificationToolbarItem);
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await GetUsuario();
        }

        private async void OnNotificationClicked(object sender, EventArgs e)
        {
            await GetNotificacionesUsuario();
        }

        private async Task GetUsuario()
        {
            try
            {
                ServicioUsuario servicioUser = new ServicioUsuario(Preferences.Get("jwt", string.Empty));
                UsuarioVM userVM = new UsuarioVM(servicioUser);
                await Shell.Current.Navigation.PushAsync(new PerfilUsuario(userVM));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No hemos podido cargar tu perfil: {ex.Message}", "Cerrar");
            }
        }

        private async Task GetNotificacionesUsuario()
        {
            try
            {

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ha habido un error al cargar tus notificaciones: {ex.Message}", "Cerrar");
            }
        }
    }
}
