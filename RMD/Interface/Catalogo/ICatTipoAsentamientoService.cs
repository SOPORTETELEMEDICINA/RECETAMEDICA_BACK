using RMD.Shared.Models.Catalogo;

namespace RMD.Interface.Catalogo
{
    public interface ICatTipoAsentamientoService
    {
        // CRUD para Tipo de Asentamiento
        Task<ResponseFromService<IEnumerable<CatTipoAsentamiento>>> GetAllTipoAsentamientosAsync();
        Task<ResponseFromService<CatTipoAsentamiento>> GetTipoAsentamientoByIdAsync(int id);
        Task<ResponseFromService<IEnumerable<CatTipoAsentamiento>>> GetTipoAsentamientosByNameAsync(string name);
        Task<ResponseFromService<CatTipoAsentamiento>> CreateTipoAsentamientoAsync(CatTipoAsentamiento asentamiento);
        Task<ResponseFromService<string>> UpdateTipoAsentamientoAsync(int id, CatTipoAsentamiento asentamiento);
        Task<ResponseFromService<string>> DeleteTipoAsentamientoAsync(int id);
    }
}
