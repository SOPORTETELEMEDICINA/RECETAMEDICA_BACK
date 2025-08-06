using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Pacientes.Response;

namespace RMD.Movil.Core.Service.Interfaces
{
    public interface IPacienteControllerService
    {
        Task<ResponseFromService<PacienteConsultaResponse>> GetPacienteByIdUsuarioAsync(Guid idUsuario);
    }
}
