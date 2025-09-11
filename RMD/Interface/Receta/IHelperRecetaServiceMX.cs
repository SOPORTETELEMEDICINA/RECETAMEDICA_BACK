using RMD.Shared.Models.Receta.Header.Internos;

namespace RMD.Interface.Receta
{
    public interface IHelperRecetaServiceMX
    {
        string GenerarRecetaQR(DatosQRHeader datosQR);
    }
}
