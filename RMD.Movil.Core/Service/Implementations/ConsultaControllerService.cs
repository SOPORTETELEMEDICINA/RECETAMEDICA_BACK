using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.GlobalResponse;
using System.Text.Json;

namespace RMD.Movil.Core.Service.Implementations
{
    public class ConsultaControllerService(HttpClient httpClient) : IConsultaControllerService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<ResponseFromService<object>> GetIdsFromLinkAsync(int id, string idType, string relacionType)
        {
            try
            {
                // Construcción de querystring (endpoint exige POST con parámetros en query)
                var qs = $"?id={id}&idType={Uri.EscapeDataString(idType)}&relacionType={Uri.EscapeDataString(relacionType)}";

                // POST sin body (SendAsync con HttpRequestMessage)
                using var req = new HttpRequestMessage(HttpMethod.Post, $"api/Consulta/GetRelaciones{qs}");
                var res = await httpClient.SendAsync(req);

                var json = await res.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<object>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al obtener relaciones (GetRelaciones)",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = null
                };
            }
        }
    }
}