// RMD.Movil/Utilities/Converters/ShowPdfConverter.cs
using System.Globalization;

namespace RMD.Movil.Utilities.Converters
{
    // Visible para todos los estatus EXCEPTO 3 (Cancelada)
    // Se mantiene IMultiValueConverter para compatibilidad con tu XAML,
    // pero la decisión depende del Estatus (values[1]).
    public class ShowPdfConverter : IMultiValueConverter
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
            // Esperado: [0] = GenerarPdf (int), [1] = Estatus (int)
            if (values == null || values.Length < 2) return false;

            var estatus = ToInt(values[1]);

            // Regla: Cancelada (3) => NO mostrar PDF. Todo lo demás => SÍ.
            return estatus != 3;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}