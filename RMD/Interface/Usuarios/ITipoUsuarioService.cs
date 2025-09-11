using RMD.Shared.Models.Usuarios;

namespace RMD.Interface.Usuarios
{
    public interface ITipoUsuarioService
    {
        Task<ResponseFromService<string>> CreateTipoUsuarioAsync(TipoUsuario tipoUsuario);
        Task<ResponseFromService<string>> UpdateTipoUsuarioAsync(TipoUsuario tipoUsuario);
        Task<ResponseFromService<IEnumerable<TipoUsuario>>> GetAllTipoUsuarioAsync();
        Task<ResponseFromService<TipoUsuario>> GetTipoUsuarioByIdAsync(Guid id);
    }
}
