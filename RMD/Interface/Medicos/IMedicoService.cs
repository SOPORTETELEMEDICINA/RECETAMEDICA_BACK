using RMD.Models.Medicos;

namespace RMD.Interface.Medicos
{
    public interface IMedicoService
    {
        Task<UsuarioMedico> GetMedicoByIdUsuarioAsync(Guid idUsuario);
        Task<UsuarioMedico> GetMedicoByIdMedicoAsync(Guid idMedico);
        Task<UsuarioMedico> GetMedicoByNameAsync(string nombreBusqueda);
        Task<bool> CreateMedicoAsync(MedicoCreate medico, Guid idRol);
        Task<bool> UpdateMedicoAsync(Medico medico, Guid idRol);
        Task<IEnumerable<PacientePorSucursalListModel>> GetPacientesBySucursalListAsync(Guid idUsuario);

    }
}
