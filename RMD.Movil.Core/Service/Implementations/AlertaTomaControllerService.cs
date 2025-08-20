using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Consulta;
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Receta.AlertaToma.Request;
using System.Net.Http.Json;
using System.Text.Json;

namespace RMD.Movil.Core.Service.Implementations
{
    public class AlertaTomaControllerService(HttpClient httpClient) : IAlertaTomaControllerService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<ResponseFromService<bool>> ActivarAlertaTomaAsync(ActivarAlertaTomaRequest request)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/Alertas/ActivarAlertaToma", request);
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<bool>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al activar alerta",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = false
                };
            }
        }

        public async Task<ResponseFromService<bool>> DesactivarAlertaTomaAsync(DesactivarAlertaTomaRequest request)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/Alertas/DesactivarAlertaToma", request);
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<bool>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al desactivar alerta",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = false
                };
            }
        }

        public async Task<ResponseFromService<bool>> ActivarAlertaManualAsync(ActivarAlertaManualRequest request)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/AlertaManual/ActivarAlertaTomaManual", request);
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<bool>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al activar alerta manual",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = false
                };
            }
        }

        public async Task<ResponseFromService<bool>> DesactivarAlertaManualAsync(DesactivarAlertaManualRequest request)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/AlertaManual/DesactivarAlertaTomaManual", request);
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseFromService<bool>>(json, JsonOptions)!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al desactivar alerta manual",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = false
                };
            }
        }

        public async Task<ResponseFromService<List<Medicamentos>>> BuscarPackagePorNombreAsync(BuscarPackageRequest request)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/AlertaManual/BuscarPorNombre", request);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ResponseFromService<List<Medicamentos>>>(json, JsonOptions)
                             ?? new ResponseFromService<List<Medicamentos>> { Data = [] };

                return result;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -999,
                    Message = "Error al buscar paquetes",
                    Toast = "error",
                    Descripcion = [ex.Message],
                    Data = []
                };
            }
        }
    }
}
