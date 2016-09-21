using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using WPFCore.Helper;

namespace WPFCore.XAML.Controls
{
    /// <summary>
    /// 
    /// Based on:
    /// http://bengribaudo.com/blog/2012/03/14/1942/saving-restoring-wpf-datagrid-columns-size-sorting-and-order
    /// </summary>
    public class EnhancedDataGrid : DataGrid
    {
        private bool inWidthChange = false;
        private bool updatingColumnInfo = false;

        /// <summary>
        /// The column information property
        /// </summary>
        public static readonly DependencyProperty ColumnInfoProperty =
            DependencyProperty.Register("ColumnInfos", typeof(ColumnInfoCollection), typeof(EnhancedDataGrid),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColumnInfoChangedCallback));

        /// <summary>
        /// Occurs when the width of a column changed.
        /// </summary>
        public event EventHandler<ColumnInfoCollection> ColumnInfosChanged;

        /// <summary>
        /// Initialization. Set's up the event handler for column width changes
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            EventHandler sortDirectionChangedHandler = (sender, x) => this.UpdateColumnInfo();
            EventHandler widthPropertyChangedHandler = (sender, x) => this.inWidthChange = true;

            // get the property descriptors for"SortDirection" and "Width"
            var sortDirectionPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.SortDirectionProperty, typeof(DataGridColumn));
            var widthPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.WidthProperty, typeof(DataGridColumn));

            base.Loaded += (sender, x) =>
                {
                    // register the event handler
                    foreach (var column in this.Columns)
                    {
                        sortDirectionPropertyDescriptor.AddValueChanged(column, sortDirectionChangedHandler);
                        widthPropertyDescriptor.AddValueChanged(column, widthPropertyChangedHandler);
                    }
                };

            base.Unloaded += (sender, x) =>
                {
                    // unregister the event handler
                    foreach (var column in this.Columns)
                    {
                        sortDirectionPropertyDescriptor.RemoveValueChanged(column, sortDirectionChangedHandler);
                        widthPropertyDescriptor.RemoveValueChanged(column, widthPropertyChangedHandler);
                    }
                };

            base.Columns.CollectionChanged += (sender, x) =>
                {
                    this.UpdateColumnInfo();
                };
            
            if (this.ColumnInfos == null)
                this.UpdateColumnInfo();

            base.OnInitialized(e);
        }

        /// <summary>
        /// Liefert die aktuelle Liste der <see cref="ColumnInfo"/>-Elemente bzw. legt diese fest
        /// </summary>
        public ColumnInfoCollection ColumnInfos
        {
            get { return (ColumnInfoCollection)this.GetValue(ColumnInfoProperty); }
            set { this.SetValue(ColumnInfoProperty, value); }
        }

        /// <summary>
        /// Event-Handler: Tritt ein, wenn die <see cref="ColumnInfo"/>-Eigenschaft verändert wurde
        /// </summary>
        /// <remarks>
        /// Eine Änderung der <c>ColumnInfo</c>-Eigenschaft führt hier dazu, dass die Spalten des <c>DataGrid</c>'s 
        /// entsprechend eingerichtet werden.
        /// </remarks>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        private static void ColumnInfoChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var grid = (EnhancedDataGrid)dependencyObject;
            if (!grid.updatingColumnInfo) 
                grid.ColumnInfoChanged();
        }

        /// <summary>
        /// Aktualisiert alle <see cref="ColumnInfo"/>-Elemente anhand der 
        /// vorhandenen Spalten des <c>DataGrid</c>'s
        /// </summary>
        public void UpdateColumnInfo()
        {
            this.updatingColumnInfo = true;

            // recreate the internal column informations
            this.ColumnInfos = new ColumnInfoCollection(base.Columns);

            if (this.ColumnInfosChanged != null)
                this.ColumnInfosChanged(this, this.ColumnInfos);

            this.updatingColumnInfo = false;
        }

        /// <summary>
        /// Übernimmt die Änderung der Spaltenreihenfolge
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnReordered(DataGridColumnEventArgs e)
        {
            this.UpdateColumnInfo();
            base.OnColumnReordered(e);
        }

        /// <summary>
        /// Event-Handler. Wenn die rechte Maustaste über einer Spaltenüberschrift losgelassen wird,
        /// kann dies ggf. das Ende einer Spaltenbreiten-Änderung sein. Dies wird im Zusammenspiel 
        /// mit dem Event, das die Änderung der Width-Eigenschaft erkennt (siehe <see cref="OnInitialized"/>),
        /// ermittelt.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.inWidthChange)
            {
                this.inWidthChange = false;
                this.UpdateColumnInfo();
            }

            base.OnPreviewMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Übernimmt alle Einstellungen aus der Liste der <see cref="ColumnInfo"/> in 
        /// die Spalten des <c>DataGrid</c>'s.
        /// </summary>
        public void ColumnInfoChanged()
        {
            this.Items.SortDescriptions.Clear();
            foreach (var column in this.ColumnInfos)
            {
                // find the grid's column matching the column info
                // start by search by ColumnId
                var gridColumn = this.Columns.FirstOrDefault(c => c is IEnhancedDataGridColumn && ((IEnhancedDataGridColumn)c).ColumnId.ToString() == column.ColumnId);
                // try using the ColumnId as column header
                if(gridColumn == null)
                    gridColumn = this.Columns.FirstOrDefault(c => column.ColumnId.Equals(c.Header));

                if (gridColumn != null)
                    column.Apply(gridColumn, this.Columns.Count, this.Items.SortDescriptions);
            }
        }

        #region class ColumnInfo
        public class ColumnInfo
        {
            public string ColumnId { get; set; }
            public object Header { get; set; }
            public string PropertyPath { get; set; }
            public ListSortDirection? SortDirection { get; set; }
            public int DisplayIndex { get; set; }
            public double WidthValue { get; set; }
            public DataGridLengthUnitType WidthType { get; set; }
            public Visibility Visibility { get; set; }

            private object baseColumn;

            /// <summary>
            /// Leerer Konstruktor. Erforderlich um serialisieren zu können
            /// </summary>
            public ColumnInfo()
            {
            }

            public ColumnInfo(DataGridColumn column)
            {
                this.baseColumn = column;

                if (column is IEnhancedDataGridColumn)
                    this.ColumnId = ((IEnhancedDataGridColumn)column).ColumnId.ToString();
                else
                    this.ColumnId = (string)column.Header;

                this.Header = column.Header;
                this.WidthValue = column.Width.DisplayValue;
                this.WidthType = column.Width.UnitType;
                this.SortDirection = column.SortDirection;
                this.DisplayIndex = column.DisplayIndex;
                this.Visibility = column.Visibility;
            }

            public ColumnInfo(string configurationString)
            {
                using (var preservedCulture = CultureHelper.UseSpecificCulture("en-US"))
                {
                    var regex = new Regex("(?<id>.*), (?<visibility>.*), (?<index>.*), (?<sort>.*), (?<width>.*)");
                    var colInfo = regex.Match(configurationString);

                    this.ColumnId = colInfo.Groups["id"].Value;
                    this.DisplayIndex = Convert.ToInt32(colInfo.Groups["index"].Value);
                    var sort = colInfo.Groups["sort"].Value;
                    var width = colInfo.Groups["width"].Value;

                    this.Visibility = (Visibility)Enum.Parse(typeof(Visibility), colInfo.Groups["visibility"].Value);
                    if (sort != "NoSort" && sort != "nosort")
                        this.SortDirection = (ListSortDirection)Enum.Parse(typeof(ListSortDirection), colInfo.Groups["visibility"].Value);
                    else
                        this.SortDirection = null;

                    if (width == "Auto")
                    {
                        this.WidthType = DataGridLengthUnitType.Auto;
                    }
                    else
                    {
                        this.WidthType = DataGridLengthUnitType.Pixel;
                        this.WidthValue = Convert.ToDouble(width);
                    }
                }
            }

            internal string GetConfigurationString()
            {
                using(var preservedCulture = CultureHelper.UseSpecificCulture("en-US"))
                    return string.Format("{0}, {1}, {2}, {3}, {4}", this.ColumnId, this.Visibility, this.DisplayIndex, (this.SortDirection.HasValue ? this.SortDirection.Value.ToString() : "NoSort"), new DataGridLength(this.WidthValue, this.WidthType));
            }

            [XmlIgnore]
            public DataGridColumn DataGridColumn
            {
                get { return this.baseColumn as DataGridColumn; }
                set { this.baseColumn = value; }
            }

            public void Apply(DataGridColumn column, int gridColumnCount, SortDescriptionCollection sortDescriptions)
            {
                column.Width = new DataGridLength(this.WidthValue, this.WidthType);
                column.SortDirection = this.SortDirection;
                column.Visibility = this.Visibility;

                if (this.SortDirection.HasValue)
                    sortDescriptions.Add(new SortDescription(this.PropertyPath, this.SortDirection.Value));

                if (column.DisplayIndex != this.DisplayIndex)
                {
                    var maxIndex = (gridColumnCount == 0) ? 0 : gridColumnCount - 1;
                    if (this.DisplayIndex < 0)
                        column.DisplayIndex = maxIndex;
                    else
                        column.DisplayIndex = (this.DisplayIndex <= maxIndex) ? this.DisplayIndex : maxIndex;
                }
            }
        }
        #endregion class ColumnInfo

        #region class ColumnInfoCollection
        public class ColumnInfoCollection : ObservableCollection<ColumnInfo>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ColumnInfoCollection"/> class.
            /// </summary>
            public ColumnInfoCollection() 
            { 
                // empty constructor is required for xml de-serialization
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ColumnInfoCollection"/> class.
            /// </summary>
            /// <param name="columns">The columns.</param>
            public ColumnInfoCollection(ObservableCollection<DataGridColumn> columns)
                : base(columns.Select((x) => new ColumnInfo(x)))
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ColumnInfoCollection"/> class.
            /// </summary>
            /// <param name="columnInfoSetting">The column information setting.</param>
            public ColumnInfoCollection(string columnInfoSetting)
            {
                var regex = new Regex(@"\((?<col>[^\)]*)\)");
                foreach (Match colMatch in regex.Matches(columnInfoSetting))
                {
                    var ci = new ColumnInfo(colMatch.Groups["col"].Value);
                    base.Add(ci);
                }
            }

            /// <summary>
            /// Gets the <see cref="ColumnInfo"/> with the specified column.
            /// </summary>
            /// <value>
            /// The <see cref="ColumnInfo"/>.
            /// </value>
            /// <param name="column">The column.</param>
            /// <returns></returns>
            public ColumnInfo this[DataGridColumn column]
            {
                get
                {
                    return this.FirstOrDefault(ci => ci.DataGridColumn == column);
                }
            }

            /// <summary>
            /// Finds the column.
            /// </summary>
            /// <param name="column">The column.</param>
            /// <returns></returns>
            public ColumnInfo FindColumn(DataGridColumn column)
            {
                // zunächst die zugewiesenen Spalten durchsuchen
                var ci = this[column];

                // nicht gefunden? Dann nach dem Header suchen
                if (ci == null)
                    ci = this.FirstOrDefault(c => c.Header.Equals((string)column.Header));

                return ci;
            }

            /// <summary>
            /// Dumps this instance.
            /// </summary>
            public void Dump()
            {
                foreach (var item in this)
                {
                    Debug.WriteLine("{0}: {1}", item.DisplayIndex, (string)item.Header);
                }
            }

            public string GetConfigurationString()
            {
                using (var preservedCulture = CultureHelper.UseSpecificCulture("en-US"))
                    return string.Join(";", this.Select(ci => string.Format("({0})", ci.GetConfigurationString())));
            }
        }
        #endregion class ColumnInfoCollection
    }
}
