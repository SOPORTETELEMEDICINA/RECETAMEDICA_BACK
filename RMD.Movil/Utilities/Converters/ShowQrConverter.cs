// RMD.Movil/Utilities/Converters/ShowQrConverter.cs
using System.Globalization;
using Microsoft.Maui.Controls;

namespace RMD.Movil.Utilities.Converters
{
    // Visible solo si:
    // - GenerarQR == 1
    // - Estatus ∈ {1 (Activa), 5 (Surtida Parcial)}
    public class ShowQrConverter : IMultiValueConverter
    {
        private static int ToInt(object v)
        {
            if (v == null) return 0;
            return v switch
            {
                int i => i,
                long l => (int)l,
                string s when int.TryParse(s, out var n) => n,
                _ => 0
            };
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Esperado: [0] = GenerarQR (int), [1] = Estatus (int)
            if (values == null || values.Length < 2) return false;

            var generarQr = ToInt(values[0]);
            var estatus = ToInt(values[1]);

            if (generarQr != 1)
                return false;

            // Solo Activa (1) y Surtida Parcial (5)
            return estatus == 1 || estatus == 5;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}