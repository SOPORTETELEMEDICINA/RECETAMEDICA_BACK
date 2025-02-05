using RMD.Models.Catalogo;
using RMD.Models.Consulta;

namespace RMD.Interface.Catalogo
{
    public interface ICatalogoService
    {
        // CRUD para Entidades Federativas
        Task<IEnumerable<CatEntidadesFederativas>> GetAllEntidadesAsync();
        Task<CatEntidadesFederativas> GetEntidadByIdAsync(int id);
        Task<IEnumerable<CatEntidadesFederativas>> GetEntidadesByNameAsync(string name);
        Task<CatEntidadesFederativas> CreateEntidadAsync(CatEntidadesFederativas entidad);
        Task UpdateEntidadAsync(int id, CatEntidadesFederativas entidad);
        Task DeleteEntidadAsync(int id);
        // CRUD para Municipios
        //Task<IEnumerable<CatMunicipios>> GetAllMunicipiosAsync();
        Task<IEnumerable<CatMunicipios>> GetAllMunicipiosByIdEntidadAsync(int idEntidad); // Nuevo método
        Task<CatMunicipios> GetMunicipioByIdAsync(int id);
        Task<IEnumerable<CatMunicipios>> GetMunicipiosByNameAsync(string name);
        Task<CatMunicipios> CreateMunicipioAsync(CatMunicipios municipio);
        Task UpdateMunicipioAsync(int id, CatMunicipios municipio);
        Task DeleteMunicipioAsync(int id);
        // CRUD para CatTipoAsentamiento
        Task<IEnumerable<CatTipoAsentamiento>> GetAllTipoAsentamientosAsync();
        Task<CatTipoAsentamiento> GetTipoAsentamientoByIdAsync(int id);
        Task<IEnumerable<CatTipoAsentamiento>> GetTipoAsentamientosByNameAsync(string name);
        Task<CatTipoAsentamiento> CreateTipoAsentamientoAsync(CatTipoAsentamiento asentamiento);
        Task UpdateTipoAsentamientoAsync(int id, CatTipoAsentamiento asentamiento);
        Task DeleteTipoAsentamientoAsync(int id);

        // CRUD para CatCP
        Task<IEnumerable<CatCP>> GetAllCPAsync();
        Task<IEnumerable<CatCP>> GetAllCPByMunicipioAsync(int idMunicipio);
        Task<IEnumerable<CatCP>> GetAllCPByEntidadAsync(int idEntidad);
        Task<CatCP> GetCPByIdAsync(int id);
        Task<CatCP> CreateCPAsync(CatCP cp);
        Task UpdateCPAsync(int id, CatCP cp);
        Task DeleteCPAsync(int id);

        // CRUD para CatCiudades
        Task<IEnumerable<CatCiudades>> GetAllCiudadesAsync();
        Task<CatCiudades> GetCiudadByIdAsync(int id);
        Task<IEnumerable<CatCiudades>> GetCiudadesByNameAsync(string name);
        Task<CatCiudades> CreateCiudadAsync(CatCiudades ciudad);
        Task UpdateCiudadAsync(int id, CatCiudades ciudad);
        Task DeleteCiudadAsync(int id);


        Task<IEnumerable<AsentamientoResultModel>> BuscarAsentamientosAsync(AsentamientoSearchModel searchModel);
    }
}
