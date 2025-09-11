using RMD.Models.PuntoVenta;
using RMD.Shared.Models.PuntoVenta;

namespace RMD.Interface.PuntoVenta
{
    public interface IPuntoVentaService
    {
        Task<ResponseFromService<PuntoVentaRecetaResponse>> ObtenerRecetaAsync(string qrEncriptado);
        Task<ResponseFromService<string>> SurtirMedicamentosAsync(SurtirRecetaRequest RecetSurtida);
        Task<ResponseFromService<PuntoVentaRecetaResponse>> ConsultarRecetaPorIdAsync(string folio);
        //Task<ResponseFromService<RepositoryPaciente>> GetRepositoryPacienteByQR(string qrEncriptado);
    }
}

