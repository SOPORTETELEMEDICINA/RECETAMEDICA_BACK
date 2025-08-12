using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Usuarios;

namespace RMD.Movil.Core.Service.Interfaces
{
    public interface IUsuariosControllerService
    {
        Task<ResponseFromService<string>> ActualizarAsync(Usuario request);
        Task<ResponseFromService<string>> CambiarPasswordAsync(CambiarPasswordRequest request);
        Task<ResponseFromService<string>> ImagenFirmaAsync(UsuarioImagenRequest request);
        Task<ResponseFromService<string?>> ImagenAsync();
        Task<ResponseFromService<string>> EliminarImagenAsync();
    }
}
