using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Receta.Detalle.Response;
using System.Net.Http.Json;
using System.Text.Json;

namespace RMD.Movil.Core.Service.Implementations
{
    public class DetalleRecetasControllerService(HttpClient httpClient) : IDetalleRecetasControllerService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<ResponseFromService<List<DetalleResponse>>> GetDetallesByIdRecetaAsync(Guid idReceta)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/DetalleRecetas/GetDetallesByIdReceta", idReceta);
                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ResponseFromService<List<DetalleResponse>>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new ResponseFromService<List<DetalleResponse>>
                {
                    Code = -999,
                    Message = "Error al obtener detalles de receta",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = []
                };
            }
        }
    }
}
