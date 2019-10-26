using System;
using System.Windows.Data;

namespace WPFCore.XAML.Converter
{
    public class BoolToVisibilityHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return System.Windows.Visibility.Hidden;

            var b = (bool)value;
            var r = parameter == null ? true : System.Convert.ToBoolean(parameter);

            return b == r ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
