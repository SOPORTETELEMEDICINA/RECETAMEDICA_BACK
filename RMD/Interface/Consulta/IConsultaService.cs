using RMD.Shared.Models.Consulta;

namespace RMD.Interface.Consulta
{
    public interface IConsultaService
    {
        Task<ResponseFromService<object>> GetIdsFromLink(int id, string idType, string relacionType);
        Task<ResponseFromService<PrescriptionResponseHTML>> ProcessPrescriptionRequest(PrescriptionModel request);
        Task<ResponseFromService<PrescriptionResponseXML>> ProcessPrescriptionXMLRequest(PrescriptionModel request);
        Task<ResponseFromService<IEnumerable<Medicamentos>>> GetMedicamentoByNameAsync(string name);
        Task<ResponseFromService<IEnumerable<SucursalModel>>> ObtenerSucursalesPorUsuarioAsync(Guid idUsuario);
        Task<ResponseFromService<bool>> GetTieneEventoSaludAsync(Guid idPaciente);
        Task<ResponseFromService<bool>> GetTieneEventoMedicamentosoAsync(Guid idPaciente);
    }
}
