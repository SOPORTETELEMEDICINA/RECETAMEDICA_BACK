using RMD.Models;
using RMD.Models.Recetas;
using RMD.Models.Responses;

namespace RMD.Interface.Recetas
{
    public interface IRecetaService
    {
        Task<ResponseFromService<string>> GetRecetaByIdRecetaAsync(Guid idReceta, Guid idPaciente);
        Task<ResponseFromService<IEnumerable<RecetaList>>> GetFilteredRecetasAsync(string roleId, Guid idGEMP, Guid? idSucursal, string? folio, DateTime? startDate, DateTime? endDate, string dateFilter);
        Task<ResponseFromService<IEnumerable<RecetaList>>> GetFilteredRecetasByIdMedicoAsync(Guid idUsuario, Guid idGEMP, Guid? idSucursal, string? folio, DateTime? startDate, DateTime? endDate, string dateFilter);
        Task<ResponseFromService<List<RecetaList>>> GetRecetasByIdPacienteAsync(Guid idUsuario, Guid idPaciente, DateTime? startDate, DateTime? endDate, string dateFilter);
        Task<ResponseFromService<Guid?>> GetIdPacienteByUsuarioAsync(Guid idUsuario);
        Task<ResponseFromService<string>> GetQRAsync(Guid idReceta, Guid idPaciente);
    }
}
