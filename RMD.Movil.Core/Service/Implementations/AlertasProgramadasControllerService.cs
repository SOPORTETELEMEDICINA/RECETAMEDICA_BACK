using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Receta.AlertaToma.Request;
using RMD.Shared.Models.Receta.AlertaToma.Response;
using System.Net.Http.Json;
using System.Text.Json;

namespace RMD.Movil.Core.Service.Implementations
{
    public class AlertasProgramadasControllerService(HttpClient httpClient) : IAlertasProgramadasControllerService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<ResponseFromService<List<AlertaProgramadaResponse>>> GetAlertasProgramadasByIdPacienteAsync(GetAlertasProgramadasRequest filterRequest)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/AlertasProgramadas/GetAlertasProgramadas", filterRequest);
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<List<AlertaProgramadaResponse>>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al obtener alertas programadas del paciente",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = []
                };
            }
        }
    }
}