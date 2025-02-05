using RMD.Models.Consulta;
using RMD.Models.PuntoVenta;
using System.Xml.Linq;

namespace RMD.Interface.Consulta
{
    public interface IConsultaService
    {
        Task<IEnumerable<RequestSearchAllergy>> GetAllergiesByNameAsync(string name);
        Task<IEnumerable<RequestSearchMolecules>> GetMoleculeByNameAsync(string name);
        //Task<IEnumerable<RequestSearchVMP>> GetVMPByNameAsync(string name);
        //Task<IEnumerable<RequestSearchProducts>> GetProductsByNameAsync(string name);
        //Task<IEnumerable<RequestSearchPackage>> GetPackagesByNameAsync(string name);
        Task<IEnumerable<RequestSearchCIM10>> GetCIM10sByNameAsync(string name);
        Task<string> GetIdsFromLink(int id, string IdType, string RelacionType);

        //Task<List<Cim10Entry>> GetCim10ByName(string name);
        Task<PrescriptionResponseHTML> ProcessPrescriptionRequest(PrescriptionModel request);
        Task<PrescriptionResponseXML> ProcessPrescriptionXMLRequest(PrescriptionModel request);
        Task<IEnumerable<Medicamentos>> GetMedicamentoByNameAsync(string name);
        Task<Guid> RegistrarRecetaAsync(RecetaRequestModel request, string token);
        Task<Guid?> ObtenerIdMedicoPorUsuarioAsync(Guid idUsuario);
        Task<RecetaCreate?> ObtenerRecetaPorIdAsync(Guid idReceta);
        Task EliminarRecetaAsync(Guid idReceta);
        Task<RecetaGetRequest?> ConsultarRecetaAsync(Guid idReceta, Guid? idMedico);
        Task ActualizarRecetaAsync(RecetaGetRequest request);
        Task<IEnumerable<RecetaPacienteModel>> ObtenerRecetasPorMedicoAsync(Guid idMedico, Guid idSucursal);
        Task<IEnumerable<SucursalModel>> ObtenerSucursalesPorUsuarioAsync(Guid idUsuario);

        Task<IEnumerable<DetalleRecetaResponse>> GetReaccionMedicamentoPrevioAsync(Guid idPaciente);
    }
}
