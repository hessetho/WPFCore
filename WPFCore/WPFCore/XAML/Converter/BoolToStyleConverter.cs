using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFCore.XAML.Converter
{
    /// <summary>
    ///     Value converter to convert a boolean into a <see cref="Style" />
    /// </summary>
    public class BoolToStyleConverter : IValueConverter
    {
        /// <summary>
        /// Gets (or sets) the style to use if the value matches the criterion 
        /// </summary>
        /// <remarks>
        /// The criterion is defined by the ConverterParameter, if omitted <c>True</c> is used as default.
        /// </remarks>
        public Style Style { get; set; }

        /// <summary>
        /// Gets (or sets) the style to use, if the value does not match the criterion
        /// </summary>
        /// <remarks>
        /// The criterion is defined by the ConverterParameter, if omitted <c>True</c> is used as default.
        /// This style must not be set.
        /// </remarks>
        public Style DefaultStyle { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var b = (bool) value;
            var criterion = true;

            if (parameter != null)
                criterion = System.Convert.ToBoolean(parameter);

            return b == criterion ? this.Style : this.DefaultStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}