using RMD.Models.Pacientes;
using System.Threading.Tasks;

namespace RMD.Interface.Pacientes
{
    public interface IPacienteService
    {
        Task<IEnumerable<PacienteConsultaRequest>> GetPacienteByNameAsync(string nombreBusqueda);
        Task<PacienteConsultaRequest> GetPacienteByIdUsuarioAsync(Guid idUsuario);
        Task<PacienteConsultaRequest> GetPacienteByIdPacienteAsync(Guid idPaciente);
        Task<bool> EliminarPacienteAsync(Guid idPaciente, Guid idUsuarioSolicitante);
        Task<bool> CreatePacienteAsync(PacienteCreateConListas pacienteRequest, Guid idUsuarioSolicitante);
        Task<bool> UpdatePacienteAsync(PacienteConListas paciente, Guid idUsuarioSolicitante);
        Task<IEnumerable<PacienteConsultaRequest>> GetPacientesByMedicoAsync(Guid idMedico);
        Task<IEnumerable<PacienteConsultaRequest>> GetPacientesByGEMPAsync(Guid idGEMP);
        Task<IEnumerable<PacienteConsultaRequest>> GetPacientesBySucursalAsync(Guid idSucursal);

        Task<IEnumerable<EntidadNacimiento>> GetEntidadesFederativasAsync();
        Task<IEnumerable<Paciente>> GetPacientesByEntidadNacimientoAsync(int idEntidadNacimiento);
    }
  
       

}
