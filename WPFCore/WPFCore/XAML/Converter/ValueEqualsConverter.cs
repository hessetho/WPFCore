using System;
using System.Windows.Data;

namespace WPFCore.XAML.Converter
{
    /// <summary>
    /// Compares a bound value against a <see cref="ComparisonValue"/>. Returns <c>True</c> if both are equal, otherwise <c>False</c>.
    /// If a <c>null</c> is passed or something that is not a number, <c>False</c> is returned.
    /// </summary>
    [ValueConversion(typeof(object), typeof(bool))]
    public class ValueEqualsConverter : IValueConverter
    {
        public double ComparisonValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;

            double val;

            if (Double.TryParse(value.ToString(), out val))
                return val.Equals(this.ComparisonValue);

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
