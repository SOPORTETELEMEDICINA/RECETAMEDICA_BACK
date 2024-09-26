using RMD.Models.Pacientes;
using System.Threading.Tasks;

namespace RMD.Interface.Pacientes
{
    public interface IPacienteService
    {
        Task<UsuarioPaciente> GetPacienteByIdUsuarioAsync(Guid idUsuario);
        Task<UsuarioPaciente> GetPacienteByIdPacienteAsync(Guid idPaciente);
        Task<IEnumerable<UsuarioPaciente>> GetPacienteByNameAsync(string nombreBusqueda);
        Task<IEnumerable<Paciente>> GetPacientesByEntidadNacimientoAsync(int idEntidadNacimiento);
        Task<bool> CreatePacienteAsync(PacienteCreateConListas pacienteRequest, Guid idUsuarioSolicitante);
        Task<bool> UpdatePacienteAsync(PacienteConListas paciente, Guid idUsuarioSolicitante);
        Task<IEnumerable<EntidadNacimiento>> GetEntidadesFederativasAsync();
    }

}
