using RMD.Shared.Models.Receta.Header.Request;
using RMD.Shared.Models.Receta.Header.Responses;

namespace RMD.Interface.Receta
{
    public interface IRecetaService
    {
        Task<ResponseFromService<string>> GetRecetaByIdRecetaAsync(Guid idReceta, Guid idPaciente);

        Task<ResponseFromService<HeaderUpdateResponse>> GetRecetaUpdatgeByIdRecetaAsync(Guid idReceta, Guid idPaciente, Guid idMedico);
       
        Task<ResponseFromService<IEnumerable<HeaderTextPlainResponse>>> GetFilteredRecetasAsync(string roleId, Guid idGEMP, Guid? idSucursal, string? folio, DateTime? startDate, DateTime? endDate, string dateFilter);
        Task<ResponseFromService<IEnumerable<HeaderTextPlainResponse>>> GetFilteredRecetasByIdMedicoAsync(Guid idUsuario, Guid idGEMP, Guid? idSucursal, string? folio, DateTime? startDate, DateTime? endDate, string dateFilter);
        Task<ResponseFromService<List<HeaderTextPlainResponse>>> GetRecetasByIdPacienteAsync(Guid idUsuario, Guid idPaciente, DateTime? startDate, DateTime? endDate, string dateFilter);
        Task<ResponseFromService<Guid?>> GetIdPacienteByUsuarioAsync(Guid idUsuario);
        Task<ResponseFromService<string>> GetQRAsync(Guid idReceta, Guid idPaciente);
        Task<ResponseFromService<Guid>> RegistrarRecetaAsync(HeaderRequest request, string token);
        Task<ResponseFromService<bool>> ActualizarRecetaAsync(HeaderUpdateRequest req, Guid idUsuario);
        Task<ResponseFromService<bool>> EliminarRecetaAsync(Guid idReceta);
        Task<ResponseFromService<List<Guid>>> TimbrarAsync(QRRequest request, Guid idUsuarioClaim);
        Task<ResponseFromService<IEnumerable<HeaderCounltResponse>>> ObtenerRecetasPorMedicoAsync(Guid idMedico, Guid idSucursal);
        Task<ResponseFromService<HeaderAndDetalleResponse>> ConsultarRecetaAsync(Guid idReceta, Guid idUsuario);
    }
}
