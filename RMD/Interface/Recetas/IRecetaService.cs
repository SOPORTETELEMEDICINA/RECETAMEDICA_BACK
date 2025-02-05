using RMD.Models;
using RMD.Models.Recetas;

namespace RMD.Interface.Recetas
{
    public interface IRecetaService
    {
        //Task<string> CreateRecetaAsync(RecetaModel recetaModel, List<RecetaDetalleModel> recetaDetalles);
        Task<RecetaRecibidaModel> GetRecetaByIdRecetaByMedicoAsync(Guid idReceta, Guid idUsuario);
       // Task<bool> UpdateRecetaAsync(RecetaModel recetaModel, List<RecetaDetalleModel> recetaDetalles, Guid idUsuario);


        Task<Receta> GetRecetaByIdAsync(Guid id);
        Task<IEnumerable<Receta>> GetRecetasByMedicoAsync(Guid idMedico);
        Task<IEnumerable<Receta>> GetRecetasByPacienteAsync(Guid idPaciente);
        // Task<bool> CreateRecetaYDetalleRecetaAsync(Receta receta, List<DetalleReceta> detalles);
        Task<IEnumerable<RecetaList>> GetFilteredRecetasAsync(string roleId, Guid idGEMP, Guid? idSucursal, DateTime? startDate, DateTime? endDate, string dateFilter);

        Task<List<RecetaList>> GetRecetasByIdPacienteAsync(Guid idPaciente, DateTime? startDate, DateTime? endDate, string dateFilter);
        Task<string> GetRecetaByIdRecetaAsync(Guid idReceta, Guid idPaciente);
    }
}
