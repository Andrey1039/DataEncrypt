using System;
using System.Windows.Data;
using System.Globalization;

namespace DataEncrypt.Data.Converters
{
    internal class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;

            if (text != string.Empty) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
