using tupencauymobile.Helpers;
using System.Net.Http.Json;
using tupencauymobile.Models;
using System.Net.Http.Headers;

namespace tupencauymobile.Services
{
    public class ServicioFcm
    {
        HttpClient _httpClient;

        public ServicioFcm(string jwt)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        }

        public async Task SendToken(FirebaseToken fcmTokenUser)
        {
            var request = await _httpClient.PostAsJsonAsync($"{NetworkConstants.localUrl}/NotificacionesPush/GuardarFcmToken", fcmTokenUser);

            if(request.IsSuccessStatusCode)
            {
                Console.WriteLine(request.Content);
                Console.WriteLine(request.RequestMessage);
            }
            else
            {
                Console.WriteLine(request.Content);
                Console.WriteLine(request.RequestMessage);
            }
        }
    }
}
