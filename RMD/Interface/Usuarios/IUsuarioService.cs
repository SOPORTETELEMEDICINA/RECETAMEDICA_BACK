using RMD.Models.Usuarios;

namespace RMD.Interface.Usuarios
{
    public interface IUsuarioService
    {
        Task<Usuario?> GetUsuarioByIdAsync(Guid idUsuario);
        Task<IEnumerable<Usuario>> GetUsuariosByAsentamientoAsync(int idAsentamiento);
        Task<IEnumerable<Usuario>> GetUsuariosByGEMPAsync(Guid idGEMP);
        Task<IEnumerable<Usuario>> GetUsuariosBySucursalAsync(Guid idSucursal);
        Task<IEnumerable<Usuario>> GetUsuariosByTipoUsuarioAsync(Guid idTipoUsuario);
        Task<IEnumerable<UsuarioSucursal>> GetUsuarioSucursalByIdAsync(Guid idSucursal);
        Task<(string Message, Guid IdUsuario)> AddUsuarioAsync(UsuarioCreate usuario, Guid idRol, string rol, Guid idGemp, Guid idSucursal);
        Task<(string mensaje, bool exito)> UpdateUsuarioAsync(Usuario usuario, Guid idUsuarioSolicitante);
        Task<bool> ValidateUserCredentialsAsync(string usr, string password);
        Task<UsuarioDetalle?> GetUsuarioByUsernameAsync(string username);
        Task<UsuarioDetalle?> GetUsuarioByEmailAsync(string email);
        Task<IEnumerable<UsuarioDetalle>> ObtenerUsuariosPorIdUsuarioYRolAsync(Guid idUsuario, Guid idRol);
        Task<string> CrearActualizarImagenFirmaAsync(UsuarioImagenRequest request);
        Task<string?> ObtenerFirmaPorIdUsuarioAsync(Guid idUsuario);
        Task<string?> ObtenerImagenPorIdUsuarioAsync(Guid idUsuario);
        Task<IEnumerable<SucursalResponse>> GetSucursalesByUsuarioAsync(Guid idUsuario);

        Task<(string mensaje, bool exito)> InactivarUsuarioAsync(Guid idUsuario, Guid idUsuarioSolicitante);
        Task<(string mensaje, bool exito)> CambiarPasswordAsync(Guid idUsuario, string nuevaPassword);
    }

}
