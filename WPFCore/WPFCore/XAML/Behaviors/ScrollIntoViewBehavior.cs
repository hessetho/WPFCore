using System;
using System.Windows.Controls;

namespace WPFCore.XAML.Behaviors
{
    /// <summary>
    /// http://www.codeproject.com/Tips/125583/ScrollIntoView-for-a-DataGrid-when-using-MVVM
    /// </summary>
    public class ScrollIntoViewBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += new SelectionChangedEventHandler(this.AssociatedObject_SelectionChanged);
        }

        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid)
            {
                DataGrid grid = (sender as DataGrid);
                if (grid.SelectedItem != null)
                {
                    Action action =delegate()
                    {
                        grid.UpdateLayout();
                        grid.ScrollIntoView(grid.SelectedItem, null);
                        grid.Focus();
                    };
                    grid.Dispatcher.BeginInvoke(action);
                }
            }
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -=
                new SelectionChangedEventHandler(this.AssociatedObject_SelectionChanged);
        }
    }
}
