using System;
using System.Windows.Data;
using System.Globalization;

namespace DataEncrypt.Data.Converters
{
    internal class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }
    }
}
