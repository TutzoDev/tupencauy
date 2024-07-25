
using Firebase.Abt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using tupencauymobile.Helpers;
using tupencauymobile.Models;

namespace tupencauymobile.Services
{
    public class ServicioPrediccion
    {
        HttpClient _httpClient;

        public ServicioPrediccion(string token)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        Prediccion prediccion = new();

        public async Task<Prediccion> GetPrediccionDisplay(string tenantId, string pencaId, string eventoId, string userId)
        {
            try
            {
                PrediccionReq dataRequired = new PrediccionReq
                {
                    tenantId = tenantId,
                    pencaId = pencaId,
                    IdEvento = eventoId,
                    userId = userId
                };

                var response = await _httpClient.PostAsJsonAsync($"{NetworkConstants.localUrl}/EventosDeportivos/GetPrediccionUser", dataRequired);
                if (response.IsSuccessStatusCode)
                {
                    prediccion = await response.Content.ReadFromJsonAsync<Prediccion>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return prediccion;
        }

        public async Task<AuthReturn> PostPrediccionDisplay(string pencaSitioUserId, string eventoId, int golesEquipoUno, int golesEquipoDos)
        {
            try
            {
                PrediccionReq dataRequired = new PrediccionReq
                {
                    IdPencaSitioUsuario = pencaSitioUserId,
                    IdEvento = eventoId,
                    ScoreTeamUno = golesEquipoUno,
                    ScoreTeamDos = golesEquipoDos
                };

                var response = await _httpClient.PostAsJsonAsync($"{NetworkConstants.localUrl}/EventosDeportivos/Predecir", dataRequired);
                if (response.IsSuccessStatusCode)
                {
                    return new AuthReturn { Success= true, Message="Tu prediccion se ha realizado correctamente"};
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new AuthReturn { Success = false, Message = "Ha habido un problema al realizar la prediccion" };
        }

        public async Task<AuthReturn> EditarPrediccionDisplay(string pencaSitioUserId, string eventoId, int golesEquipoUno, int golesEquipoDos)
        {
            try
            {
                PrediccionReq dataRequired = new PrediccionReq
                {
                    IdPencaSitioUsuario = pencaSitioUserId,
                    IdEvento = eventoId,
                    ScoreTeamUno = golesEquipoUno,
                    ScoreTeamDos = golesEquipoDos
                };

                var response = await _httpClient.PostAsJsonAsync($"{NetworkConstants.localUrl}/EventosDeportivos/EditarPrediccion", dataRequired);
                if (response.IsSuccessStatusCode)
                {
                    return new AuthReturn { Success = true, Message = "Tu prediccion se ha realizado correctamente" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new AuthReturn { Success = false, Message = "Ha habido un problema al realizar la prediccion" };
        }
    }
}
