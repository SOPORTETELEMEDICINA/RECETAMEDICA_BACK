using RMD.Shared.Models.Receta.Detalle.Interno;
using RMD.Shared.Models.Receta.Header.Internos;

namespace RMD.Interface.Receta
{
    public interface IHelperRecetaServiceES
    {
        Task<List<DetalleDto>> PrepararPaquetesExactosESAsync(
            List<DetalleDto> detalles, List<PackageDetalleDto> paquetes);

        string GenerarRecetaQR(DatosQRHeader datosQR);
    }
}
