using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFCore.XAML
{
    /// <summary>
    /// http://www.thomaslevesque.com/2009/03/27/wpf-automatically-sort-a-gridview-when-a-column-header-is-clicked/
    /// </summary>
    /// <remarks>
    /// Usage:
    ///            &lt;ListView ItemsSource="{Binding Path=MYBINDING_TO_LIST}"
    ///                         local:GridViewSort.AutoSort="True"&gt;    
    ///                &lt;ListView.View&gt;
    ///                    &lt;GridView&gt;
    ///                        &lt;GridView.Columns&gt;
    ///                                 &lt;GridViewColumn Header="MyHeader"
    ///                                            DisplayMemberBinding="{Binding Path=MYBINDING}"
    ///                                            local:GridViewSort.PropertyName="MYBINDING" /&gt;
    ///
    /// </remarks>
    public class GridViewSort
    {
        #region PowerUp properties

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(GridViewSort),
                new UIPropertyMetadata(null, (o, e) =>
                {
                    var listView = o as ItemsControl;
                    if (listView != null)
                    {
                        if (!GetAutoSort(listView))
                        // Don't change click handler if AutoSort enabled                             
                        {
                            if (e.OldValue != null && e.NewValue == null)
                            {
                                listView.RemoveHandler(ButtonBase.ClickEvent,
                                    new RoutedEventHandler(ColumnHeader_Click));
                            }
                            if (e.OldValue == null && e.NewValue != null)
                            {
                                listView.AddHandler(ButtonBase.ClickEvent,
                                    new RoutedEventHandler(ColumnHeader_Click));
                            }
                        }
                    }
                }));

        public static readonly DependencyProperty AutoSortProperty =
            DependencyProperty.RegisterAttached(
                "AutoSort",
                typeof(bool),
                typeof(GridViewSort),
                new UIPropertyMetadata(false, (o, e) =>
                {
                    var listView = o as ListView;
                    if (listView != null)
                    {
                        if (GetCommand(listView) == null)
                        // Don't change click handler if a command is set                            
                        {
                            var oldValue = (bool)e.OldValue;
                            var newValue = (bool)e.NewValue;
                            if (oldValue && !newValue)
                            {
                                listView.RemoveHandler(ButtonBase.ClickEvent,
                                    new RoutedEventHandler(ColumnHeader_Click));
                            }
                            if (!oldValue && newValue)
                            {
                                listView.AddHandler(ButtonBase.ClickEvent,
                                    new RoutedEventHandler(ColumnHeader_Click));
                            }
                        }
                    }
                }));

        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(GridViewSort),
                new UIPropertyMetadata(null));


        // Using a DependencyProperty as the backing store for Command.  
        // This enables animation, styling, binding, etc...         
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoSort.  
        // This enables animation, styling, binding, etc...         
        public static bool GetAutoSort(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoSortProperty);
        }

        public static void SetAutoSort(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoSortProperty, value);
        }

        // Using a DependencyProperty as the backing store for PropertyName.  
        // This enables animation, styling, binding, etc...         
        public static string GetPropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(PropertyNameProperty);
        }

        public static void SetPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(PropertyNameProperty, value);
        }


        #endregion

        #region Column header click event handler

        private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked != null)
            {
                string propertyName = GetPropertyName(headerClicked.Column);
                if (!string.IsNullOrEmpty(propertyName))
                {
                    var listView = GetAncestor<ListView>(headerClicked);
                    if (listView != null)
                    {
                        ICommand command = GetCommand(listView);
                        if (command != null)
                        {
                            if (command.CanExecute(propertyName))
                            {
                                command.Execute(propertyName);
                            }
                        }
                        else if (GetAutoSort(listView))
                        {
                            ApplySort(listView.Items, propertyName);
                        }
                    }
                }
            }
        }

        #endregion

        #region Helper methods

        public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);
            while (!(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (parent != null) return (T)parent;
            return null;
        }

        public static void ApplySort(ICollectionView view, string propertyName)
        {
            var direction = ListSortDirection.Ascending;
            if (view.SortDescriptions.Count > 0)
            {
                SortDescription currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                {
                    if (currentSort.Direction == ListSortDirection.Ascending) direction = ListSortDirection.Descending;
                    else direction = ListSortDirection.Ascending;
                }
                view.SortDescriptions.Clear();
            }
            if (!string.IsNullOrEmpty(propertyName))
            {
                view.SortDescriptions.Add(new SortDescription(propertyName, direction));
            }
        }

        #endregion
    }
}
