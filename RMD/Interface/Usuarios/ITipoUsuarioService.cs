using RMD.Models.Usuarios;

namespace RMD.Interface.Usuarios
{
    public interface ITipoUsuarioService
    {
        Task<IEnumerable<TipoUsuario>> GetAllTipoUsuario();
        Task<TipoUsuario> GetTipoUsuarioById(Guid id);
        Task<string> CreateTipoUsuario(TipoUsuario tipoUsuario);
        Task<string> UpdateTipoUsuario(TipoUsuario tipoUsuario);
    }
}
