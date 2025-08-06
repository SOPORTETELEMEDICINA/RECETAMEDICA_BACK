using RMD.Shared.Models.Catalogo;

namespace RMD.Interface.Catalogo
{
    public interface ICatCpService
    {
        // CRUD para Código Postal (CP)
        Task<ResponseFromService<IEnumerable<CatCP>>> GetAllCPAsync();
        Task<ResponseFromService<IEnumerable<CatCP>>> GetAllCPByMunicipioAsync(int idMunicipio);
        Task<ResponseFromService<IEnumerable<CatCP>>> GetAllCPByEntidadAsync(int idEntidad);
        Task<ResponseFromService<CatCP>> GetCPByIdAsync(int id);
        Task<ResponseFromService<CatCP>> CreateCPAsync(CatCP cp);
        Task<ResponseFromService<string>> UpdateCPAsync(int id, CatCP cp);
        Task<ResponseFromService<string>> DeleteCPAsync(int id);
    }
}
