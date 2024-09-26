using RMD.Models.Catalogos;
using RMD.Models.Consulta;

namespace RMD.Interface.Catalogos
{
    public interface ICatalogoService
    {
        Task<IEnumerable<Catalogo>> GetCatalogoByIdCP(int idCP);
        Task<IEnumerable<Catalogo>> GetCatalogoByIdMunicipio(int idMunicipio);
        Task<IEnumerable<Catalogo>> GetCatalogoByIdAsentamiento(int idAsentamiento);
        //Task<IEnumerable<Catalogo>> GetCatalogoByIdEntidad(int idEntidad);
        Task<IEnumerable<Catalogo>> GetCatalogoByIdCiudad(int idCiudad);
        Task<CatalogoDetail> GetDetailsByIdCP(int idCP);
        Task<CatalogoDetail> GetDetailsByIdAsentamiento(int idAsentamiento);
        Task<CatalogoDetail> GetDetailsByIdMunicipio(int idMunicipio);
        //Task<CatalogoDetail> GetDetailsByIdEntidad(int idEntidad);
        Task<CatalogoDetail> GetDetailsByIdCiudad(int idCiudad);
        Task<string> CreateOrUpdateCatalogo(Catalogo catalogo);
        Task<IEnumerable<AsentamientoResultModel>> BuscarAsentamientosAsync(AsentamientoSearchModel searchModel);
    }
}
