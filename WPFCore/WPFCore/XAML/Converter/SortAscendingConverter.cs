using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace WPFCore.XAML.Converter
{
    /// <summary>
    /// This <see cref="IValueConverter"/> takes an arbitrary <see cref="IList"/> and returns
    /// a <see cref="ListCollectionView"/> sorted by a property, specified as <c>ConverterParameter</c>.
    /// The sort direction is <c>ListSortDirection.Ascending</c>.
    /// </summary>
    public class SortAscendingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as IList;
            if (collection == null || parameter == null) return null;

            ListCollectionView view = new ListCollectionView(collection);
            SortDescription sort = new SortDescription(parameter.ToString(), ListSortDirection.Ascending);
            view.SortDescriptions.Add(sort);

            return view;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
