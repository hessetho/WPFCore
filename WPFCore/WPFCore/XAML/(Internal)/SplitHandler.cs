using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WPFCore.Helper;

namespace WPFCore.XAML
{
    /// <summary>
    /// Internal class. Used to immediatly store the current "position" of a GridSplitter.
    /// This class is instanciated from <see cref="GridSplitterSaver"/>
    /// </summary>
    internal class SplitHandler : IDisposable
    {
        public GridSplitter Splitter { get; private set; }
        public Grid Grid { get; private set; }
        public readonly string Name;

        private Window parentWindow;
        private DependencyPropertyDescriptor dpd;
        private object registeredComponent;
        private EventHandler registeredHandler;

        /// <summary>
        /// Constructor. Registers all required details for a GridSplitter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="splitter"></param>
        /// <param name="grid"></param>
        public SplitHandler(string name, GridSplitter splitter, Grid grid)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (splitter == null) throw new ArgumentNullException("splitter");
            if (grid == null) throw new ArgumentNullException("grid");

            this.Name = name;
            this.Splitter = splitter;
            this.Grid = grid;

            if (this.Splitter.IsLoaded)
                this.Initialize();
            else
            {
                // Warten, bis der Splitter komplett geladen worden ist
                // Da der Handler nur einmal gebraucht wird, erzeugen wir hier einen
                // anonymen Ereignis-Delegaten, der dann gleich wieder "abgehängt" wird.
                RoutedEventHandler onLoaded = null;
                onLoaded = (ds, de) =>
                    {
                        this.Splitter.Loaded -= onLoaded;
                        this.Initialize();
                    };

                this.Splitter.Loaded += onLoaded;
            }

        }

        private void Initialize()
        {
            this.parentWindow = Window.GetWindow(this.Splitter);

            if (this.Splitter.ResizeDirection == GridResizeDirection.Columns)
            {
                var col = (int)this.Splitter.GetValue(Grid.ColumnProperty);
                var sizeThisColumn = col > 0 ? this.Grid.ColumnDefinitions[col - 1] : this.Grid.ColumnDefinitions[col];

                sizeThisColumn.Width = this.LoadSplitter(sizeThisColumn.Width);

                this.dpd = DependencyPropertyDescriptor.FromProperty(ColumnDefinition.WidthProperty, typeof(ColumnDefinition));
                this.RegisterHandler(sizeThisColumn, this.OnColumnWidthChanged);
            }
            else
            {
                var row = (int)this.Splitter.GetValue(Grid.RowProperty);
                var sizeThisRow = row > 0 ? this.Grid.RowDefinitions[row - 1] : this.Grid.RowDefinitions[row];

                sizeThisRow.Height = this.LoadSplitter(sizeThisRow.Height);

                this.dpd = DependencyPropertyDescriptor.FromProperty(RowDefinition.HeightProperty, typeof(RowDefinition));
                this.RegisterHandler(sizeThisRow, this.OnRowHeightChanged);
            }
        }

        /// <summary>
        /// Registers an event handler which tracks the position of the GridSplitter
        /// </summary>
        /// <param name="component"></param>
        /// <param name="handler"></param>
        private void RegisterHandler(object component, EventHandler handler)
        {
            this.registeredComponent = component;
            this.registeredHandler = handler;
            this.dpd.AddValueChanged(component, handler);
        }

        /// <summary>
        /// Do some clean up.
        /// </summary>
        public void Dispose()
        {
            this.dpd.RemoveValueChanged(this.registeredComponent, this.registeredHandler);
        }

        /// <summary>
        /// Event handler. Tracks column width changes of the GridSplitter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnColumnWidthChanged(object sender, EventArgs e)
        {
            var column = sender as ColumnDefinition;
            Debug.Assert(column != null);

            this.SaveSplitter(column.ActualWidth);
        }

        /// <summary>
        /// Event handler. Tracks row height changes of the GridSplitter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRowHeightChanged(object sender, EventArgs e)
        {
            var row = sender as RowDefinition;
            Debug.Assert(row != null);

            this.SaveSplitter(row.ActualHeight);
        }

        /// <summary>
        /// Save the current GridSplitter position
        /// </summary>
        /// <param name="size"></param>
        private void SaveSplitter(double size)
        {
            this.parentWindow.SaveSettingDouble(this.Name, size);
        }

        /// <summary>
        /// Retrieve the last stored position of the GridSplitter
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private GridLength LoadSplitter(GridLength defaultValue)
        {
            var value = this.parentWindow.GetSettingDouble(this.Name, double.NaN);
            return !double.IsNaN(value) ? new GridLength(value, GridUnitType.Pixel) : defaultValue;
        }
    }
}
