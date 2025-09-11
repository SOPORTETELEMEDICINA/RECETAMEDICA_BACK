using QRCoder;

namespace RMD.Extensions
{
    public static class QRGenerator
    {
        public static string GenerarQR(string textoEncriptado)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(textoEncriptado, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData); // Usa PngByteQRCode en vez de QRCode
            var qrImageBytes = qrCode.GetGraphic(20);

            return Convert.ToBase64String(qrImageBytes);
        }
    }
}
