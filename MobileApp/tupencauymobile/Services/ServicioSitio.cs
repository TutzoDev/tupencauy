using System.Net.Http.Headers;
using System.Net.Http.Json;
using tupencauymobile.Helpers;
using tupencauymobile.Models;

namespace tupencauymobile.Services
{
    public class ServicioSitio
    {
        HttpClient _httpClient;

        public ServicioSitio()
        {
            _httpClient = new HttpClient();
        }

        List<Sitio> sitiosDisplay = new();

        public async Task<List<Sitio>> GetSitios()
        {
            if (sitiosDisplay.Count != 0)
            {
                return sitiosDisplay;
            }

            try
            {
                var response = await _httpClient.GetAsync($"{NetworkConstants.localUrl}/Sitios");

                if (response.IsSuccessStatusCode)
                {
                    sitiosDisplay = await response.Content.ReadFromJsonAsync<List<Sitio>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return sitiosDisplay;
        }
    }
}
