using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Receta.Header.Request;
using RMD.Shared.Models.Receta.Header.Responses;
using System.Net.Http.Json;
using System.Text.Json;

namespace RMD.Movil.Core.Service.Implementations
{
    public class RecetaControllerService(HttpClient httpClient) : IRecetaControllerService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<ResponseFromService<List<HeaderTextPlainResponse>>> GetRecetasByIdPacienteAsync(HeaderFilterByPacienteRequest filterRequest)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/Recetas/GetRecetasByIdPaciente", filterRequest);
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<List<HeaderTextPlainResponse>>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al obtener recetas del paciente",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = []
                };
            }
        }

        public async Task<ResponseFromService<string>> GetQRByIdRecetaAsync(Guid idReceta)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/Recetas/GetQRByIdReceta", idReceta);
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<string>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al obtener QR de receta",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = string.Empty
                };
            }
        }

        public async Task<string> GetRecetaByIdRecetaAsync(RecetaRequest request)
        {
            try
            {
                using var response = await httpClient.PostAsJsonAsync("api/Recetas/GetRecetaByIdReceta", request);
                var html = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var status = (int)response.StatusCode;
                    var reason = response.ReasonPhrase ?? "Sin razón";
                    throw new HttpRequestException($"Error HTTP {status} al obtener la receta: {reason}");
                }

                if (string.IsNullOrWhiteSpace(html))
                    throw new InvalidOperationException("El servidor devolvió HTML vacío.");

                return html;
            }
            catch (TaskCanceledException ex) when (!ex.CancellationToken.IsCancellationRequested)
            {
                throw new HttpRequestException("Tiempo de espera agotado al obtener la receta.", ex);
            }
            catch (HttpRequestException)
            {
                throw; // Deja pasar errores HTTP con su mensaje original
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Fallo al obtener la receta (error inesperado).", ex);
            }
        }

    }
}
