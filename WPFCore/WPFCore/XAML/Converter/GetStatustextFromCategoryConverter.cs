using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace WPFCore.XAML.Converter
{
    public class GetStatustextFromCategoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var list = value as Dictionary<string, string>;
            var category = (string)parameter;

            if (list != null && list.ContainsKey(category))
                return list[category];

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
