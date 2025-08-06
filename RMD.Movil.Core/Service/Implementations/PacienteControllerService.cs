using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Pacientes.Response;
using System.Text.Json;

namespace RMD.Movil.Core.Service.Implementations
{
    public class PacienteControllerService(HttpClient httpClient) : IPacienteControllerService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<ResponseFromService<PacienteConsultaResponse>> GetPacienteByIdUsuarioAsync(Guid idUsuario)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Pacientes/ByIdUsuario/{idUsuario}");
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<PacienteConsultaResponse>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al obtener paciente",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = new()
                };
            }
        }
    }
}
