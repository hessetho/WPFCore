using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace WPFCore.XAML
{
    /// <summary>
    /// BindableMultiSelection is an attachable property for ItemControls that
    /// allow the user to select multiple items from its list.
    /// 
    /// However, this works one-way only, i.e. the user's changes to the selection 
    /// are reflected to the bound items collection but not vice versa.
    /// </summary>
    public class BindableMultiSelection
    {
        public static DependencyProperty SelectedItemsProperty =
                        DependencyProperty.RegisterAttached("SelectedItems", typeof(ObservableCollection<object>), typeof(BindableMultiSelection),
                        new PropertyMetadata(OnSelectedItemsPropertyChanged));

        public static ObservableCollection<object> GetSelectedItems(DependencyObject d)
        {
            return (ObservableCollection<object>) d.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(DependencyObject d, ObservableCollection<object> selectedItems)
        {
            d.SetValue(SelectedItemsProperty, selectedItems);
        }

        private static void OnSelectedItemsPropertyChanged(DependencyObject d,  DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                if (d is DataGrid)
                {
                    var listView = d as DataGrid;
                    listView.SelectionChanged -= OnSelectionChanged;
                }
                else if (d is Xceed.Wpf.DataGrid.DataGridControl)
                {
                    var grid = d as Xceed.Wpf.DataGrid.DataGridControl;
                    grid.SelectionChanged -= OnXceedSelectionChanged;
                }
            }
            if (e.NewValue != null)
            {
                if (d is DataGrid)
                {
                    var listView = d as DataGrid;
                    listView.SelectionChanged += OnSelectionChanged;
                }
                else if (d is Xceed.Wpf.DataGrid.DataGridControl)
                {
                    var grid = d as Xceed.Wpf.DataGrid.DataGridControl;
                    grid.SelectionChanged += OnXceedSelectionChanged;
                }
            }
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid)
            {
                var grid = (DataGrid)sender;
                var list = (ObservableCollection<object>) grid.GetValue(SelectedItemsProperty);

                foreach (var newItem in e.AddedItems)
                    if(!list.Contains(newItem))
                        list.Add(newItem);

                foreach (var removedItem in e.RemovedItems)
                    list.Remove(removedItem);
            }
        }

        static void OnXceedSelectionChanged(object sender, Xceed.Wpf.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            if (sender is Xceed.Wpf.DataGrid.DataGridControl)
            {
                var grid = (Xceed.Wpf.DataGrid.DataGridControl)sender;
                var list = (ObservableCollection<object>)grid.GetValue(SelectedItemsProperty);

                foreach (var selectionInfo in e.SelectionInfos)
                {
                    foreach (var newItem in selectionInfo.AddedItems)
                        if (!list.Contains(newItem))
                            list.Add(newItem);
                    foreach (var removedItem in selectionInfo.RemovedItems)
                        list.Remove(removedItem);
                }
            }
        }
    }
}
