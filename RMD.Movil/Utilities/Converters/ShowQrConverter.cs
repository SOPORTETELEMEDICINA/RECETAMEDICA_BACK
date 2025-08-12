using System.Globalization;

namespace RMD.Movil.Utilities.Converters
{
    // Muestra el botón de QR solo si GenerarQR == 1 y Estatus NO es {3,4,6}
    public class ShowQrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            int generarQr = 0;
            int estatus = 0;

            if (values[0] is int gq)
                generarQr = gq;
            if (values[1] is int es)
                estatus = es;

            if (generarQr != 1)
                return false;

            // 3 Cancelada, 4 Vencida, 6 Vencida Surtida Parcial => ocultar
            return estatus != 3 && estatus != 4 && estatus != 6;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}