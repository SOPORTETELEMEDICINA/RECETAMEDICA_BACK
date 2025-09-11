using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Pacientes.Request;
using RMD.Shared.Models.Pacientes.Response;
using System.Net.Http.Json;
using System.Text.Json;

namespace RMD.Movil.Core.Service.Implementations
{
    public class EventosSaludControllerService(HttpClient httpClient) : IEventosSaludControllerService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<ResponseFromService<IEnumerable<EventosSaludResponse>>> GetAllEventosPacienteAsync()
        {
            try
            {
                var response = await httpClient.GetAsync("api/EventosSalud/eventosSalud");
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<IEnumerable<EventosSaludResponse>>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al obtener eventos de salud",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = []
                };
            }
        }

        public async Task<ResponseFromService<EventosSaludResponse>> GetEventoPacienteByIdAsync(Guid idEventoSalud)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/EventosSalud/eventosSalud/{idEventoSalud}");
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<EventosSaludResponse>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al consultar evento de salud",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = new()
                };
            }
        }

        public async Task<ResponseFromService<bool>> CreateEventoPacienteAsync(EventosSaludRequest evento)
        {
            try
            {
                // Serializar manualmente a camelCase
                var json = JsonSerializer.Serialize(evento, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("api/EventosSalud/eventosSalud", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ResponseFromService<bool>>(responseJson, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al crear evento de salud",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = false
                };
            }
        }

        public async Task<ResponseFromService<bool>> UpdateEventoPacienteAsync(EventosSaludRequest evento)
        {
            try
            {
                var response = await httpClient.PutAsJsonAsync("api/EventosSalud/eventosSalud", evento);
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<bool>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al actualizar evento de salud",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = false
                };
            }
        }

        public async Task<ResponseFromService<bool>> DeleteEventoPacienteAsync(Guid idEventoSalud)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"api/EventosSalud/eventosSalud/{idEventoSalud}");
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<bool>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al eliminar evento de salud",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = false
                };
            }
        }
    }
}
