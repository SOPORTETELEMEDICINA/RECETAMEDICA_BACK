using RMD.Shared.Models.Catalogo;

namespace RMD.Interface.Catalogo
{
    public interface ICatAsentamientosService
    {
        // CRUD para CatAsentamientos
        Task<ResponseFromService<IEnumerable<CatAsentamientos>>> GetAllAsentamientosAsync();
        Task<ResponseFromService<CatAsentamientos>> GetAsentamientoByIdAsync(int id);
        Task<ResponseFromService<CatAsentamientos>> CreateAsentamientoAsync(CatAsentamientos asentamiento);
        Task<ResponseFromService<string>> UpdateAsentamientoAsync(int id, CatAsentamientos asentamiento);
        Task<ResponseFromService<string>> DeleteAsentamientoAsync(int id);
    }
}
