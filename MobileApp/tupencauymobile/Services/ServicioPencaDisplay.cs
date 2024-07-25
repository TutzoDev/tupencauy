using System.Net.Http.Headers;
using System.Net.Http.Json;
using tupencauymobile.Helpers;
using tupencauymobile.Models;

namespace tupencauymobile.Services
{
    public partial class ServicioPencaDisplay
    {
        HttpClient _httpClient;
        string tenant;

        public ServicioPencaDisplay(string token, string tenantId)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            this.tenant = tenantId;
        }

        List<UnoVsUno> eventosUnoVsUnoPenca = new();
        List<PosicionPenca> posicionesPenca = new();

        public async Task<List<UnoVsUno>> GetPencaDisplay(string id)
        {
            if (eventosUnoVsUnoPenca.Count != 0)
            {
                return eventosUnoVsUnoPenca;
            }

            try
            {
                var response = await _httpClient.GetAsync($"{NetworkConstants.localUrl}/Pencas/{id}/GetEventosUnoVsUno");

                if (response.IsSuccessStatusCode)
                {
                    eventosUnoVsUnoPenca = await response.Content.ReadFromJsonAsync<List<UnoVsUno>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return eventosUnoVsUnoPenca;
        }

        public async Task<List<PosicionPenca>> GetPosicionesPencaSitio(string pencaId)
        {
            if (posicionesPenca.Count != 0)
            {
                return posicionesPenca;
            }

            try
            {
                var response = await _httpClient.GetAsync($"{NetworkConstants.localUrl}/Pencas/Sitio/{this.tenant}/Penca/{pencaId}/GetUsuarios");
                if (response.IsSuccessStatusCode)
                {
                    posicionesPenca = await response.Content.ReadFromJsonAsync<List<PosicionPenca>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return posicionesPenca;
        }
    }
}