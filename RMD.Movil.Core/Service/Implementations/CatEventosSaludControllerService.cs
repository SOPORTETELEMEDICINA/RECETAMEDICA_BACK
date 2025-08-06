using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Catalogo;
using RMD.Shared.Models.GlobalResponse;
using System.Net.Http.Json;

namespace RMD.Movil.Core.Service.Implementations
{
    public class CatEventosSaludControllerService : ICatEventosSaludControllerService
    {
        private readonly HttpClient _httpClient;

        public CatEventosSaludControllerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseFromService<IEnumerable<CatEventosDeSalud>>> GetAllEventosSaludAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/CatEventosDeSalud/CateventosSalud");

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseFromService<IEnumerable<CatEventosDeSalud>>
                    {
                        Code = (int)response.StatusCode,
                        Message = "Error al consultar eventos de salud.",
                        Toast = "error",
                        Data = null!
                    };
                }

                return await response.Content.ReadFromJsonAsync<ResponseFromService<IEnumerable<CatEventosDeSalud>>>();
            }
            catch (Exception ex)
            {
                return new ResponseFromService<IEnumerable<CatEventosDeSalud>>
                {
                    Code = 500,
                    Message = "Error de conexión: " + ex.Message,
                    Toast = "error",
                    Data = null!
                };
            }
        }
    }
}