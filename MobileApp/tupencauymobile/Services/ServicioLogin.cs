using System.Net.Http.Json;
using tupencauymobile.Helpers;
using tupencauymobile.Models;

namespace tupencauymobile.Services
{
    public class ServicioLogin
    {
        private readonly HttpClient _httpClient;

        public ServicioLogin()
        {
            _httpClient = new HttpClient();
        }

        public async Task<AuthReturn> LoginAsync(LoginReq request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{NetworkConstants.localUrl}/Auth/login", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthReturn>();
            }

            return new AuthReturn { Success = false, Message = "Error en la solicitud de login." };
        }
    }
}
