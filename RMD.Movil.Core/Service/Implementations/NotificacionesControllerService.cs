using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.OneSignal.Request;
using System.Net.Http.Json;

namespace RMD.Movil.Core.Service.Implementations
{
    public sealed class NotificacionesControllerService : INotificacionesControllerService
    {
        private readonly HttpClient _http;
        public NotificacionesControllerService(HttpClient http) { _http = http; }

        public async Task<ResponseFromService<object>> ProgramarTomaAsync(ProgramarTomaRequest req)
        {
            var resp = await _http.PostAsJsonAsync("api/Notificaciones/ProgramarToma", req);
            resp.EnsureSuccessStatusCode();
            return (await resp.Content.ReadFromJsonAsync<ResponseFromService<object>>())!;
        }
    }
}
