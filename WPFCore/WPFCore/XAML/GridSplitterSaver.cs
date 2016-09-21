using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFCore.XAML
{
    public static class GridSplitterSaver
    {
        ///<summary>
        /// Bezeichnet den Namen der Einstellung für den zugehörigen <see cref="GridSplitter"/>
        ///</summary>
        public static DependencyProperty SaveNameProperty =
           DependencyProperty.RegisterAttached("SaveName", typeof(string), typeof(GridSplitterSaver),
           new PropertyMetadata(OnSaveNamePropertyChanged));

        ///<summary>
        /// Liefert den Namen der Einstellung für den zugehörigen <see cref="GridSplitter"/>
        ///</summary>
        ///<param name="depObj">Der zugewiesene <c>GridSplitter</c></param>
        ///<returns>Der Name der Einstellung</returns>
        public static string GetSaveName(DependencyObject depObj)
        {
            return ((string)(depObj.GetValue(SaveNameProperty))); 
        }

        ///<summary>
        /// Setzt den Namen der Einstellung für den zugehörigen <see cref="GridSplitter"/>
        ///</summary>
        ///<param name="depObj">Der zugewiesene <c>GridSplitter</c></param>
        ///<param name="saveName">Der Name der Einstellung</param>
        public static void SetSaveName(DependencyObject depObj, string saveName)
        {
            depObj.SetValue(SaveNameProperty, saveName);
        }

        /// <summary>
        /// Ereignis-Handler, der ausgeführt wird, wenn sich die Eigenschaft <see cref="SaveNameProperty"/> ändert.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnSaveNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != null && e.OldValue == null)
            {
                var splitter = d as GridSplitter;
                if (splitter == null) return;

                var grid = VisualTreeHelper.GetParent(splitter) as Grid;
                if (grid == null) return;

                new SplitHandler(e.NewValue as string, splitter, grid);
            }
        }
    }
}
