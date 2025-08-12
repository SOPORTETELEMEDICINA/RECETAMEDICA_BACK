using System.Globalization;

namespace RMD.Movil.Utilities.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        private static int ToStatusId(object? value)
        {
            if (value is int i) return i;

            // Si viene como string y es número, lo convertimos
            if (int.TryParse(value?.ToString(), out var id))
                return id;

            return 0;
        }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var id = ToStatusId(value);

            return id switch
            {
                1 => Color.FromArgb("#4CAF50"), // Activa → Verde
                2 => Color.FromArgb("#39d1bf"), // Surtida → Verde agua
                3 => Color.FromArgb("#e53935"), // Cancelada → Rojo fuerte
                4 => Color.FromArgb("#FF9800"), // Vencida → Naranja
                5 => Color.FromArgb("#968de2"), // Surtida Parcialmente → Morado claro
                6 => Color.FromArgb("#f06292"), // Vencida Surtida Parcialmente → Rosa fuerte
                _ => Colors.Gray               // Desconocido
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    public class StatusToTextConverter : IValueConverter
    {
        private static int ToStatusId(object? value)
        {
            if (value is int i) return i;
            if (int.TryParse(value?.ToString(), out var id))
                return id;

            return 0;
        }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var id = ToStatusId(value);

            return id switch
            {
                1 => "Activa",
                2 => "Surtida",
                3 => "Cancelada",
                4 => "Vencida",
                5 => "Surtida Parcialmente",
                6 => "Vencida Surtida Parcialmente",
                _ => "Desconocido"
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    public class BoolNegationConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is bool b ? !b : value;

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is bool b ? !b : value;
    }

    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
                return dt.ToString(parameter?.ToString() ?? "yyyy-MM-dd");

            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
