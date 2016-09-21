using System;
using System.Windows.Data;

namespace WPFCore.XAML.Converter
{
    public class NegateValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;

            double val;
            if (Double.TryParse(value.ToString(), out val))
                return -val;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
