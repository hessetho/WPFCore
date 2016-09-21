using System;
using System.Windows.Data;

namespace WPFCore.XAML.Converter
{
    [ValueConversion(typeof(Exception), typeof(string))]
    public class FullExceptionMessageTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var exception = value as Exception;
            if (exception == null) return string.Empty;

            return Controls.ExceptionGallery.GetExceptionText(exception);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
