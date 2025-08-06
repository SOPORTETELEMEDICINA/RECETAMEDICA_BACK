using RMD.Shared.Models.Catalogo;

namespace RMD.Interface.Catalogo
{
    public interface ICatMunicipiosService
    {

        // CRUD para Municipios
        Task<ResponseFromService<IEnumerable<CatMunicipios>>> GetAllMunicipiosByIdEntidadAsync(int idEntidad);
        Task<ResponseFromService<CatMunicipios>> GetMunicipioByIdAsync(int id);
        Task<ResponseFromService<IEnumerable<CatMunicipios>>> GetMunicipiosByNameAsync(string name);
        Task<ResponseFromService<CatMunicipios>> CreateMunicipioAsync(CatMunicipios municipio);
        Task<ResponseFromService<string>> UpdateMunicipioAsync(int id, CatMunicipios municipio);
        Task<ResponseFromService<string>> DeleteMunicipioAsync(int id);
    }
}
