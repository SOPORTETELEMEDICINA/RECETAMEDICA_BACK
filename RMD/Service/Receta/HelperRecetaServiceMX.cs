using RMD.Extensions;
using RMD.Interface.Receta;
using RMD.Shared.Models.Receta.Header.Internos;

namespace RMD.Service.Receta
{
    public class HelperRecetaServiceMX : IHelperRecetaServiceMX
    {
       
        public string GenerarRecetaQR(DatosQRHeader datosQR)
        {
            var texto = $"{datosQR.IdReceta}|{datosQR.IdMedico}|{datosQR.IdGEMP}|{datosQR.IdSucursal}|{DateTime.Now:O}";
            var textoEncriptado = EncryptionHelper.Encrypt(texto);
            var qrBase64 = QRGenerator.GenerarQR(textoEncriptado);
            return qrBase64;
        }
    }
}
