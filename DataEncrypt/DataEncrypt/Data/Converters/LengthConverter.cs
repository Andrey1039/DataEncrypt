using System;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace DataEncrypt.Data.Converters
{
    internal class LengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;

            if (text != null)
                return Encoding.UTF8.GetByteCount(text) >= 32;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
