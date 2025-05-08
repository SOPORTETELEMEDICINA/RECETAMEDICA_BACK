using RMD.Models.Catalogo;
using RMD.Models.Consulta;
using RMD.Models.Responses;

namespace RMD.Interface.Catalogo
{
    public interface ICatalogoService
    {
        // CRUD para Entidades Federativas
        Task<ResponseFromService<IEnumerable<CatEntidadesFederativas>>> GetAllEntidadesAsync();
        Task<ResponseFromService<CatEntidadesFederativas>> GetEntidadByIdAsync(int id);
        Task<ResponseFromService<IEnumerable<CatEntidadesFederativas>>> GetEntidadesByNameAsync(string name);
        Task<ResponseFromService<CatEntidadesFederativas>> CreateEntidadAsync(CatEntidadesFederativas entidad);
        Task<ResponseFromService<string>> UpdateEntidadAsync(int id, CatEntidadesFederativas entidad);
        Task<ResponseFromService<string>> DeleteEntidadAsync(int id);

        // CRUD para Municipios
        Task<ResponseFromService<IEnumerable<CatMunicipios>>> GetAllMunicipiosByIdEntidadAsync(int idEntidad);
        Task<ResponseFromService<CatMunicipios>> GetMunicipioByIdAsync(int id);
        Task<ResponseFromService<IEnumerable<CatMunicipios>>> GetMunicipiosByNameAsync(string name);
        Task<ResponseFromService<CatMunicipios>> CreateMunicipioAsync(CatMunicipios municipio);
        Task<ResponseFromService<string>> UpdateMunicipioAsync(int id, CatMunicipios municipio);
        Task<ResponseFromService<string>> DeleteMunicipioAsync(int id);

        // CRUD para Tipo de Asentamiento
        Task<ResponseFromService<IEnumerable<CatTipoAsentamiento>>> GetAllTipoAsentamientosAsync();
        Task<ResponseFromService<CatTipoAsentamiento>> GetTipoAsentamientoByIdAsync(int id);
        Task<ResponseFromService<IEnumerable<CatTipoAsentamiento>>> GetTipoAsentamientosByNameAsync(string name);
        Task<ResponseFromService<CatTipoAsentamiento>> CreateTipoAsentamientoAsync(CatTipoAsentamiento asentamiento);
        Task<ResponseFromService<string>> UpdateTipoAsentamientoAsync(int id, CatTipoAsentamiento asentamiento);
        Task<ResponseFromService<string>> DeleteTipoAsentamientoAsync(int id);

        // CRUD para Código Postal (CP)
        Task<ResponseFromService<IEnumerable<CatCP>>> GetAllCPAsync();
        Task<ResponseFromService<IEnumerable<CatCP>>> GetAllCPByMunicipioAsync(int idMunicipio);
        Task<ResponseFromService<IEnumerable<CatCP>>> GetAllCPByEntidadAsync(int idEntidad);
        Task<ResponseFromService<CatCP>> GetCPByIdAsync(int id);
        Task<ResponseFromService<CatCP>> CreateCPAsync(CatCP cp);
        Task<ResponseFromService<string>> UpdateCPAsync(int id, CatCP cp);
        Task<ResponseFromService<string>> DeleteCPAsync(int id);

        // CRUD para Ciudades
        Task<ResponseFromService<IEnumerable<CatCiudades>>> GetAllCiudadesAsync();
        Task<ResponseFromService<CatCiudades>> GetCiudadByIdAsync(int id);
        Task<ResponseFromService<IEnumerable<CatCiudades>>> GetCiudadesByNameAsync(string name);
        Task<ResponseFromService<CatCiudades>> CreateCiudadAsync(CatCiudades ciudad);
        Task<ResponseFromService<string>> UpdateCiudadAsync(int id, CatCiudades ciudad);
        Task<ResponseFromService<string>> DeleteCiudadAsync(int id);

        // Búsqueda de Asentamientos
        Task<ResponseFromService<IEnumerable<AsentamientoResultModel>>> BuscarAsentamientosAsync(AsentamientoSearchModel searchModel);

        // CRUD para Eventos de Salud
        Task<ResponseFromService<IEnumerable<CatEventosDeSalud>>> GetAllEventosSaludAsync();
        Task<ResponseFromService<CatEventosDeSalud>> GetEventoSaludByIdAsync(int id);
        Task<ResponseFromService<CatEventosDeSalud>> CreateEventoSaludAsync(CatEventosDeSalud evento);
        Task<ResponseFromService<string>> UpdateEventoSaludAsync(int id, CatEventosDeSalud evento);
        Task<ResponseFromService<string>> DeleteEventoSaludAsync(int id);
    }
}
