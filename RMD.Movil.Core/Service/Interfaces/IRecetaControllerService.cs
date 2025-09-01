using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Receta.Header.Request;
using RMD.Shared.Models.Receta.Header.Responses;

namespace RMD.Movil.Core.Service.Interfaces
{
    public interface IRecetaControllerService
    {
        Task<ResponseFromService<List<HeaderTextPlainResponse>>> GetRecetasByPacienteAsync(Guid? idPaciente = null);
        Task<ResponseFromService<string>> GetQRByIdRecetaAsync(Guid idReceta);
        Task<string> GetRecetaByIdRecetaAsync(RecetaRequest request);
    }
}
