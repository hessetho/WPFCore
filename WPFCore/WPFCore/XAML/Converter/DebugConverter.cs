using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFCore.XAML.Converter
{
    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine("--- DebugConverter ------------------------------------");
            if (value == null)
                Debug.WriteLine("value is null.");
            else
                Debug.WriteLine("value is {0} ({1})", value, value.GetType().Name);
            
            Debug.WriteLine("target type is {0}", targetType);

            if(parameter!=null)
                Debug.WriteLine("parameter is {0}", parameter);

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
