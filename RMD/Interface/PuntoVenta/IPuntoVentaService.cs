using RMD.Models.Consulta;
using RMD.Models.PuntoVenta;
using RMD.Models.Responses;

namespace RMD.Interface.PuntoVenta
{
    public interface IPuntoVentaService
    {
        Task<ResponseFromService<PuntoVentaRecetaResponse>>
            ObtenerRecetaAsync(Guid idReceta, Guid idMedico, DateTime fechaUltimaModificacion);
        Task<ResponseFromService<string>> SurtirMedicamentosAsync(Guid idReceta, List<Guid> detallesReceta);

        Task<ResponseFromService<PuntoVentaRecetaResponse>> ConsultarRecetaPorIdAsync(string folio);
    }
}
