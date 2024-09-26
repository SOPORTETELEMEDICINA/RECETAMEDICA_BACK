using RMD.Models.Recetas;

namespace RMD.Interface.Recetas
{
    public interface IRecetaService
    {
        Task<Receta> GetRecetaByIdAsync(Guid id);
        Task<IEnumerable<Receta>> GetRecetasByMedicoAsync(Guid idMedico);
        Task<IEnumerable<Receta>> GetRecetasByPacienteAsync(Guid idPaciente);
        Task<bool> CreateRecetaYDetalleRecetaAsync(Receta receta, List<DetalleReceta> detalles);
    }
}
