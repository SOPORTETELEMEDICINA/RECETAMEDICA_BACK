using System.Globalization;

namespace RMD.Movil.Utilities.Converters
{
    public class PasswordIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (value is bool visible && visible) ? "\uf06e" : "\uf070";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return false;
        }
    }
}