using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Receta.Detalle.Request;
using RMD.Shared.Models.Receta.Detalle.Response;
using RMD.Shared.Models.Receta.Paciente.Request;
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

                var dto = JsonSerializer.Deserialize<ResponseFromService<List<DetalleResponse>>>(json, JsonOptions)
                          ?? new ResponseFromService<List<DetalleResponse>>
                          {
                              Code = (int)response.StatusCode,
                              Toast = "error",
                              Message = "Respuesta vacía del servidor",
                              Data = new List<DetalleResponse>()
                          };

                // FILTRO AQUÍ: solo con CantidadSurtida > 0
                if (dto.Data != null)
                    dto.Data = dto.Data
                        .Where(d => d.CantidadSurtida.HasValue && d.CantidadSurtida.Value > 0)
                        .ToList();

                return dto;
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


        public async Task<ResponseFromService<IEnumerable<MedicamentoActivoResponse>>> GetSoloMedicamentosActivosAsync()
        {
            try
            {
                var response = await httpClient.GetAsync("api/DetalleRecetas/GetSoloMedicamentosActivos");
                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ResponseFromService<IEnumerable<MedicamentoActivoResponse>>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new ResponseFromService<IEnumerable<MedicamentoActivoResponse>>
                {
                    Code = -999,
                    Message = "Error al obtener medicamentos activos",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = []
                };
            }
        }

        public async Task<ResponseFromService<string>> CreateUpdateReaccionAsync(DetalleRequest request)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/DetalleRecetas/reaccion-crear-actualizar", request);
                var json = await response.Content.ReadAsStringAsync();

                // El backend devuelve Ok/BadRequest con el mismo envelope
                var result = JsonSerializer.Deserialize<ResponseFromService<string>>(json, JsonOptions);
                if (result is not null) return result;

                // Fallback si no pudo deserializar
                return new ResponseFromService<string>
                {
                    Code = (int)response.StatusCode,
                    Toast = "error",
                    Message = "No se pudo interpretar la respuesta del servidor.",
                    Descripcion = [$"HTTP {(int)response.StatusCode}"]
                };
            }
            catch (Exception ex)
            {
                return new ResponseFromService<string>
                {
                    Code = -999,
                    Message = "Error al crear/actualizar reacción",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = string.Empty
                };
            }
        }

        public async Task<ResponseFromService<List<DetalleResponse>>> GetDetalleByPacienteAsync(Guid? idPaciente = null)
        {
            try
            {
                // El controller resuelve IdPaciente desde el token si mandas null o {}
                var body = new RecetasByPacienteRequest { IdPaciente = idPaciente };

                var response = await httpClient.PostAsJsonAsync("api/DetalleRecetas/GetDetalleByPaciente", body);
                var json = await response.Content.ReadAsStringAsync();

                var dto = JsonSerializer.Deserialize<ResponseFromService<List<DetalleResponse>>>(json, JsonOptions)
                          ?? new ResponseFromService<List<DetalleResponse>>
                          {
                              Code = (int)response.StatusCode,
                              Toast = "error",
                              Message = "Respuesta vacía del servidor",
                              Data = new List<DetalleResponse>()
                          };

                // No filtramos en cliente: el SP ya aplica reglas (periodo, estatus, CantidadSurtida cuando aplica)
                return dto;
            }
            catch (Exception ex)
            {
                return new ResponseFromService<List<DetalleResponse>>
                {
                    Code = -999,
                    Message = "Error al obtener detalles por paciente",
                    Toast = "error",
                    Descripcion = new List<string> { ex.Message },
                    Data = new List<DetalleResponse>()
                };
            }
        }
    }
}
