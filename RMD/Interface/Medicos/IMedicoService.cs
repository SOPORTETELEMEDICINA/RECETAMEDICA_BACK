using RMD.Models.Medicos;

namespace RMD.Interface.Medicos
{
    public interface IMedicoService
    {
        Task<MedicoConsultaRequest> GetMedicoByIdUsuarioAsync(Guid idUsuario);
        Task<bool> CreateMedicoAsync(MedicoCreate medico, Guid idRol);
        Task<bool> UpdateMedicoAsync(Medico medico, Guid idRol);
        Task<IEnumerable<PacientePorSucursalListModel>> GetPacientesBySucursalListAsync(Guid idUsuario);
        Task<bool> DeleteMedicoAsync(Guid idMedico, Guid idUsuarioSolicitante);
        Task<IEnumerable<MedicoConsultaRequest>> GetMedicoByNameAsync(string nombreBusqueda);
        Task<IEnumerable<MedicoConsultaRequest>> GetMedicosBySucursalAsync(Guid idSucursal);
        Task<IEnumerable<MedicoConsultaRequest>> GetMedicosByGEMPAsync(Guid idGEMP);
        Task<MedicoConsultaRequest> GetMedicoByIdMedicoAsync(Guid idMedico);

    }
}
