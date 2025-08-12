using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Usuarios;
using System.Net.Http.Json;
using System.Text.Json;

namespace RMD.Movil.Core.Service.Implementations
{
    public class UsuariosControllerService(HttpClient http) : IUsuariosControllerService
    {
        private static readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

        public async Task<ResponseFromService<string>> ActualizarAsync(Usuario request)
        {
            var res = await http.PutAsJsonAsync("api/Usuarios/actualizar", request);
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ResponseFromService<string>>(json, options)!;
        }

        public async Task<ResponseFromService<string>> CambiarPasswordAsync(CambiarPasswordRequest request)
        {
            var res = await http.PostAsJsonAsync("api/Usuarios/cambiar-password", request);
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ResponseFromService<string>>(json, options)!;
        }

        public async Task<ResponseFromService<string>> ImagenFirmaAsync(UsuarioImagenRequest request)
        {
            var res = await http.PostAsJsonAsync("api/Usuarios/imagen-firma", request);
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ResponseFromService<string>>(json, options)!;
        }


        public async Task<ResponseFromService<string?>> ImagenAsync()
        {
            var res = await http.GetAsync("api/Usuarios/imagen");
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ResponseFromService<string?>>(json, options)!;
        }

        public async Task<ResponseFromService<string>> EliminarImagenAsync()
        {
            var res = await http.DeleteAsync("api/Usuarios/imagen");
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ResponseFromService<string>>(json, options)!;
        }
    }
}
