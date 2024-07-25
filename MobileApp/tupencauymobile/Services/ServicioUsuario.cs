using System.Net.Http.Headers;
using System.Net.Http.Json;
using tupencauymobile.Helpers;
using tupencauymobile.Models;

namespace tupencauymobile.Services
{
    public class ServicioUsuario
    {
        HttpClient _httpClient;

        public ServicioUsuario(string token)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        Usuario userDisplay = new ();

        public async Task<Usuario> GetUsuario(string usuarioId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{NetworkConstants.localUrl}/Users/{usuarioId}/GetInfoUser");

                if (response.IsSuccessStatusCode)
                {
                    userDisplay = await response.Content.ReadFromJsonAsync<Usuario>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return userDisplay;
        }

        public async Task<bool> ActivarDesactivarNotificaciones(string userId, bool habilitadas)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{NetworkConstants.localUrl}/NotificacionesPush/{userId}/ConfigurarNotificaciones?habilitadas={habilitadas}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Se cambio el estado de las notificaciones correctamente");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
