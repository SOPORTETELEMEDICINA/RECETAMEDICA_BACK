using System.Text.Json;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Receta.Catalogos;

namespace RMD.Movil.Core.Service.Implementations
{
    public class RecetaCatalogosControllerService : IRecetaCatalogosControllerService
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public RecetaCatalogosControllerService(HttpClient httpClient)
        {
            _http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<ResponseFromService<List<FrecuencyTypeListModel>>> GetFrecuencyTypesAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/RecetaCatalogos/frecuency-types");
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<List<FrecuencyTypeListModel>>>(json, JsonOptions)
                       ?? new ResponseFromService<List<FrecuencyTypeListModel>> { Data = [] };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al obtener Frecuency Types",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = []
                };
            }
        }

        public async Task<ResponseFromService<FrecuencyTypeListModel>> GetFrecuencyTypeByIdAsync(int id)
        {
            try
            {
                var response = await _http.GetAsync($"api/RecetaCatalogos/frecuency-types/{id}");
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<FrecuencyTypeListModel>>(json, JsonOptions)
                       ?? new ResponseFromService<FrecuencyTypeListModel> { Data = new() };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al obtener Frecuency Type por Id",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = new()
                };
            }
        }
    }
}
