// RMD.Movil/Utilities/Converters/IntToBoolConverter.cs

using System.Globalization;

namespace RMD.Movil.Utilities.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return false;

            switch (value)
            {
                case bool b:
                    return b;
                case sbyte or byte or short or ushort or int or uint or long or ulong:
                    return System.Convert.ToInt64(value) > 0;
                case string s:
                    if (string.Equals(s, "true", StringComparison.OrdinalIgnoreCase)) return true;
                    if (string.Equals(s, "false", StringComparison.OrdinalIgnoreCase)) return false;
                    if (long.TryParse(s, out var n)) return n > 0;
                    return false;
                default:
                    return false;
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}