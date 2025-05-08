//using RMD.Models.Consulta;
//using RMD.Models.PuntoVenta;
//using System.Xml.Linq;

//namespace RMD.Interface.Consulta
//{
//    public interface IConsultaService
//    {
//        Task<IEnumerable<RequestSearchAllergy>> GetAllergiesByNameAsync(string name);
//        Task<IEnumerable<RequestSearchMolecules>> GetMoleculeByNameAsync(string name);
//        //Task<IEnumerable<RequestSearchVMP>> GetVMPByNameAsync(string name);
//        //Task<IEnumerable<RequestSearchProducts>> GetProductsByNameAsync(string name);
//        //Task<IEnumerable<RequestSearchPackage>> GetPackagesByNameAsync(string name);
//        Task<IEnumerable<RequestSearchCIM10>> GetCIM10sByNameAsync(string name);
//        Task<string> GetIdsFromLink(int id, string IdType, string RelacionType);

//        //Task<List<Cim10Entry>> GetCim10ByName(string name);
//        Task<PrescriptionResponseHTML> ProcessPrescriptionRequest(PrescriptionModel request);
//        Task<PrescriptionResponseXML> ProcessPrescriptionXMLRequest(PrescriptionModel request);
//        Task<IEnumerable<Medicamentos>> GetMedicamentoByNameAsync(string name);
//        Task<Guid> RegistrarRecetaAsync(RecetaRequestModel request, string token);
//        Task<Guid?> ObtenerIdMedicoPorUsuarioAsync(Guid idUsuario);
//        Task<RecetaCreate?> ObtenerRecetaPorIdAsync(Guid idReceta);
//        Task EliminarRecetaAsync(Guid idReceta);
//        Task<RecetaGetRequest> ConsultarRecetaAsync(Guid idReceta);
//        //Task ActualizarRecetaAsync(RecetaGetRequest request);
//        Task<IEnumerable<RecetaGet>> ObtenerRecetasPorMedicoAsync(Guid idMedico, Guid idSucursal);
//        Task<IEnumerable<SucursalModel>> ObtenerSucursalesPorUsuarioAsync(Guid idUsuario);

//        Task<IEnumerable<DetalleRecetaResponse>> GetReaccionMedicamentoPrevioAsync(Guid idPaciente);

//        // Nuevos métodos para verificar eventos
//        Task<bool> GetTieneEventoSaludAsync(Guid idPaciente);
//        Task<bool> GetTieneEventoMedicamentosoAsync(Guid idPaciente);
//    }
//}
using RMD.Models.Consulta;
using RMD.Models.PuntoVenta;
using RMD.Models.Responses;
using System.Xml.Linq;

namespace RMD.Interface.Consulta
{
    public interface IConsultaService
    {
        Task<ResponseFromService<IEnumerable<RequestSearchAllergy>>> GetAllergiesByNameAsync(string name);
        Task<ResponseFromService<IEnumerable<RequestSearchMolecules>>> GetMoleculeByNameAsync(string name);
        Task<ResponseFromService<IEnumerable<RequestSearchCIM10>>> GetCIM10sByNameAsync(string name);
        Task<ResponseFromService<string>> GetIdsFromLink(int id, string idType, string relacionType);
        Task<ResponseFromService<PrescriptionResponseHTML>> ProcessPrescriptionRequest(PrescriptionModel request);
        Task<ResponseFromService<PrescriptionResponseXML>> ProcessPrescriptionXMLRequest(PrescriptionModel request);
        Task<ResponseFromService<IEnumerable<Medicamentos>>> GetMedicamentoByNameAsync(string name);
        Task<ResponseFromService<Guid>> RegistrarRecetaAsync(RecetaRequestModel request, string token);
        Task<ResponseFromService<Guid?>> ObtenerIdMedicoPorUsuarioAsync(Guid idUsuario);
        Task<ResponseFromService<RecetaCreate?>> ObtenerRecetaPorIdAsync(Guid idReceta);
        Task<ResponseFromService<bool>> EliminarRecetaAsync(Guid idReceta);
        Task<ResponseFromService<RecetaGetRequest>> ConsultarRecetaAsync(Guid idReceta);
        Task<ResponseFromService<IEnumerable<RecetaGet>>> ObtenerRecetasPorMedicoAsync(Guid idMedico, Guid idSucursal);
        Task<ResponseFromService<IEnumerable<SucursalModel>>> ObtenerSucursalesPorUsuarioAsync(Guid idUsuario);
        Task<ResponseFromService<IEnumerable<DetalleRecetaResponse>>> GetReaccionMedicamentoPrevioAsync(Guid idPaciente);
        Task<ResponseFromService<bool>> GetTieneEventoSaludAsync(Guid idPaciente);
        Task<ResponseFromService<bool>> GetTieneEventoMedicamentosoAsync(Guid idPaciente);
    }
}
