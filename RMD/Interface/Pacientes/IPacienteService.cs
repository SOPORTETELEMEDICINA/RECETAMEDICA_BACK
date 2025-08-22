using RMD.Shared.Models.Pacientes.Request;
using RMD.Shared.Models.Pacientes.Response;

namespace RMD.Interface.Pacientes
{
    public interface IPacienteService
    {
        //Task<ResponseFromService<Guid?>> GetIdPacienteByUsuarioAsync(Guid idUsuario);
        Task<ResponseFromService<IEnumerable<PacienteConsultaResponse>>> GetPacienteByNameAsync(string nombreBusqueda, Guid idGEMP);
        Task<ResponseFromService<PacienteConsultaResponse>> GetPacienteByIdUsuarioAsync(Guid idUsuario);
        Task<ResponseFromService<PacienteConsultaResponse>> GetPacienteByIdPacienteAsync(Guid idPaciente);
        Task<ResponseFromService<bool>> EliminarPacienteAsync(Guid idPaciente, Guid idUsuarioSolicitante);
        Task<ResponseFromService<bool>> CreatePacienteAsync(PacienteCreateConListasRequest pacienteRequest, Guid idUsuarioSolicitante);
        Task<ResponseFromService<bool>> UpdatePacienteAsync(PacienteConListasRequest paciente, Guid idUsuarioSolicitante);
        Task<ResponseFromService<IEnumerable<PacienteConsultaResponse>>> GetPacientesByMedicoAsync(Guid idMedico);
        Task<ResponseFromService<IEnumerable<PacienteConsultaResponse>>> GetPacientesByGEMPAsync(Guid idGEMP);
        Task<ResponseFromService<IEnumerable<PacienteConsultaResponse>>> GetPacientesBySucursalAsync(Guid idSucursal);

        Task<ResponseFromService<IEnumerable<EntidadNacimientoResponse>>> GetEntidadesFederativasAsync();
        //Task<ResponseFromService<IEnumerable<Paciente>>> GetPacientesByEntidadNacimientoAsync(int idEntidadNacimiento);
        Task<ResponseFromService<string>> GenerarQRParaPacienteAsync(Guid idPaciente);
        Task<ResponseFromService<Guid>> GetIdPacienteByUsuarioAsync(Guid idUsuario);
    }
}
