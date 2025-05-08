using System.Threading.Tasks;
using RMD.Models.Responses;
using RMD.Models.Usuarios;

namespace RMD.Interface.Usuarios
{
    public interface IUsuarioService
    {
        Task<ResponseFromService<string>> EliminarImagenAsync(Guid idUsuario);
        Task<ResponseFromService<string>> CrearActualizarImagenFirmaAsync(UsuarioImagenRequest request);
        Task<ResponseFromService<IEnumerable<RequestUsuario>>> GetUsuariosByGEMPAsync(Guid idGEMP);
        Task<ResponseFromService<IEnumerable<RequestUsuario>>> GetUsuariosBySucursalAsync(Guid idSucursal);
        Task<ResponseFromService<object>> AddUsuarioAsync(UsuarioCreate usuario, Guid idRol, string rol, Guid idGemp, Guid idSucursal);
        Task<ResponseFromService<string>> UpdateUsuarioAsync(Usuario usuario, Guid idUsuarioSolicitante);
        Task<ResponseFromService<bool>> ValidateUserCredentialsAsync(string usr, string password);
        Task<ResponseFromService<UsuarioDetalle>> GetUsuarioByUsernameAsync(string username);
        Task<ResponseFromService<UsuarioDetalle>> GetUsuarioPacienteByUsernameAsync(string username);
        Task<ResponseFromService<UsuarioDetalle>> GetUsuarioByEmailAsync(string email);
        Task<ResponseFromService<IEnumerable<UsuarioDetalle>>> ObtenerUsuariosPorIdUsuarioYRolAsync(Guid idUsuario, Guid idRol);
        Task<ResponseFromService<string>> EliminarFirmaAsync(Guid idUsuario);
        Task<ResponseFromService<string?>> ObtenerImagenPorIdUsuarioAsync(Guid idUsuario);
        Task<ResponseFromService<string>> InactivarUsuarioAsync(Guid idUsuario, Guid idUsuarioSolicitante);
        Task<ResponseFromService<string>> CambiarPasswordAsync(Guid idUsuario, string nuevaPassword);
        Task<ResponseFromService<string?>> ObtenerFirmaPorIdUsuarioAsync(Guid idUsuario);
    }

}
