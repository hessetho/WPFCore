using System;
using System.Windows.Data;

namespace WPFCore.XAML.Converter
{
    public class BoolToTextConverter : IValueConverter
    {
        public string TrueText { get; set; }
        public string FalseText { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var b = false;
            if (value is bool)
                return ((bool)value) ? this.TrueText : this.FalseText;
            else if (value is string)
                if (bool.TryParse((string)value, out b))
                {
                    return b ? this.TrueText : this.FalseText;
                }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
