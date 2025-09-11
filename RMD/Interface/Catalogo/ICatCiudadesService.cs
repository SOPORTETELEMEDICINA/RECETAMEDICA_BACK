using RMD.Shared.Models.Catalogo;

namespace RMD.Interface.Catalogo
{
    public interface ICatCiudadesService
    {
        // CRUD para Ciudades
        Task<ResponseFromService<IEnumerable<CatCiudades>>> GetAllCiudadesAsync();
        Task<ResponseFromService<CatCiudades>> GetCiudadByIdAsync(int id);
        Task<ResponseFromService<IEnumerable<CatCiudades>>> GetCiudadesByNameAsync(string name);
        Task<ResponseFromService<CatCiudades>> CreateCiudadAsync(CatCiudades ciudad);
        Task<ResponseFromService<string>> UpdateCiudadAsync(int id, CatCiudades ciudad);
        Task<ResponseFromService<string>> DeleteCiudadAsync(int id);
    }
}
