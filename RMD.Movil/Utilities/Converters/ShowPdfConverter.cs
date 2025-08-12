// RMD.Movil/Utilities/Converters/ShowPdfConverter.cs
using System.Globalization;

namespace RMD.Movil.Utilities.Converters
{
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
            if (values == null || values.Length < 2) return false;

            var generarPdf = ToInt(values[0]);
            var estatus = ToInt(values[1]);

            return generarPdf == 1 || estatus == 2; // Surtida
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}