using RMD.Shared.Models.Catalogo;

namespace RMD.Interface.Catalogo
{
    public interface ICatEntidadesFederativasService
    {
        // CRUD para Entidades Federativas
        Task<ResponseFromService<IEnumerable<CatEntidadesFederativas>>> GetAllEntidadesAsync();
        Task<ResponseFromService<CatEntidadesFederativas>> GetEntidadByIdAsync(int id);
        Task<ResponseFromService<IEnumerable<CatEntidadesFederativas>>> GetEntidadesByNameAsync(string name);
        Task<ResponseFromService<CatEntidadesFederativas>> CreateEntidadAsync(CatEntidadesFederativas entidad);
        Task<ResponseFromService<string>> UpdateEntidadAsync(int id, CatEntidadesFederativas entidad);
        Task<ResponseFromService<string>> DeleteEntidadAsync(int id);
    }
}
