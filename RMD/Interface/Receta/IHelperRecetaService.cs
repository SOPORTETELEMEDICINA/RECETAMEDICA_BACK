using RMD.Shared.Models.Receta.Detalle.Interno;
using RMD.Shared.Models.Receta.Header.Internos;
using RMD.Shared.Models.Receta.Header.Responses;

namespace RMD.Interface.Receta
{
    public interface IHelperRecetaService
    {
        Task<ResponseFromService<(List<DetalleDto> Detalles, List<PackageDetalleDto> Paquetes)>> GetDatosParaTimbradoAsync(Guid idReceta, Guid idUsuario);
        Task<List<DetalleInHeader>> CalcularPaquetesExactosAsync( List<DetalleDto> detalles, List<PackageDetalleDto> paquetes);
        List<DetalleDto> SepararPorBloquesESAsync(List<DetalleDto> mayoresA30Dias);
        List<DetalleDto> UnirDetallesParaCalculoES(List<DetalleDto> bloques, List<DetalleDto> menoresA30Dias);
        Task<Guid> GetMedicoIdOrThrowAsync(Guid idUsuarioClaim);
        Task<ResponseFromService<bool>> ActualizarPacienteDesdeReceta(Guid idPaciente, string alergias, string molecules, string patologias, DateTime fechaUltimaModificacion);
        HeaderCounltResponse TransformRecetaSQLToRecetaGet(HeaderBySQL receta);
        Header_UpdatePacienteParsedRequest ParsePacienteToParsed(Header_UpdatePacienteRequest receta);
        string ConvertirListaAString(List<int> lista);
        string ObtenerValorDesdeToken(string token, string claimType);
    }
}
