using System.Net.Http.Headers;
using System.Net.Http.Json;
using tupencauymobile.Helpers;
using tupencauymobile.Models;

namespace tupencauymobile.Services
{
    public class ServicioPenca
    {
        HttpClient _httpClient;

        public ServicioPenca(string token) {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        List<Penca> pencasDisplay = new ();

        public async Task<List<Penca>> GetPencas()
        {
            if(pencasDisplay.Count != 0)
            {
                return pencasDisplay;
            }

            try
            {
                var response = await _httpClient.GetAsync($"{NetworkConstants.localUrl}/Sitios/{Preferences.Get("tenantId", string.Empty)}/Pencas?idUser={Preferences.Get("userId", string.Empty)}");

                if (response.IsSuccessStatusCode)
                {
                    pencasDisplay = await response.Content.ReadFromJsonAsync<List<Penca>>();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        
            return pencasDisplay;
        }
    }
}
