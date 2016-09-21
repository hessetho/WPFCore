using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFCore.XAML.Converter
{
    /// <summary>
    /// Returns the type name of an object
    /// </summary>
    class ObjectTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? (object)"" : value.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
