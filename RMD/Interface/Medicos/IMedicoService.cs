using RMD.Shared.Models.Medicos;

namespace RMD.Interface.Medicos
{
    public interface IMedicoService
    {
        Task<ResponseFromService<MedicoConsultaRequest>> GetMedicoByIdUsuarioAsync(Guid idUsuario);
        Task<ResponseFromService<bool>> CreateMedicoAsync(MedicoCreate medico, Guid idRol);
        Task<ResponseFromService<string>> UpdateMedicoAsync(Medico medico, Guid idUsuarioSolicitante);
        Task<ResponseFromService<IEnumerable<PacientePorSucursalListModel>>> GetPacientesBySucursalListAsync(Guid idUsuario);
        Task<ResponseFromService<bool>> DeleteMedicoAsync(Guid idMedico, Guid idUsuarioSolicitante);
        Task<ResponseFromService<IEnumerable<MedicoConsultaRequest>>> GetMedicoByNameAsync(string nombreBusqueda);
        Task<ResponseFromService<IEnumerable<MedicoConsultaRequest>>> GetMedicosBySucursalAsync(Guid idSucursal);
        Task<ResponseFromService<IEnumerable<MedicoConsultaRequest>>> GetMedicosByGEMPAsync(Guid idGEMP);
        Task<ResponseFromService<MedicoConsultaRequest>> GetMedicoByIdMedicoAsync(Guid idMedico);
    }
}
