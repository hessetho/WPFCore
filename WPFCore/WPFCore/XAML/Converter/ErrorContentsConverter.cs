using System;
using System.Linq;
using System.Windows.Data;

namespace WPFCore.XAML.Converter
{
    public class ErrorContentsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var errors = value as System.Collections.ObjectModel.ReadOnlyObservableCollection<System.Windows.Controls.ValidationError>;
            if (errors == null || errors.Count == 0)
                return null;

            return string.Join("\r\n", errors.Select(e => e.ErrorContent).ToList());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
