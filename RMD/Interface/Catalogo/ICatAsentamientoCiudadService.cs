using RMD.Shared.Models.Catalogo;

namespace RMD.Interface.Catalogo
{
    public interface ICatAsentamientoCiudadService
    {

        // CRUD para CatAsentamientoCiudad
        Task<ResponseFromService<IEnumerable<CatAsentamientoCiudad>>> GetAllAsentamientoCiudadAsync();
        Task<ResponseFromService<CatAsentamientoCiudad>> GetAsentamientoCiudadByIdAsync(int id);
        Task<ResponseFromService<CatAsentamientoCiudad>> CreateAsentamientoCiudadAsync(CatAsentamientoCiudad entidad);
        Task<ResponseFromService<string>> UpdateAsentamientoCiudadAsync(int id, CatAsentamientoCiudad entidad);
        Task<ResponseFromService<string>> DeleteAsentamientoCiudadAsync(int id);
    }
}
