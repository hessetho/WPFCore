using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace WPFCore.Data.FlexData
{
    public class FlexTable<T> : List<T> where T: FlexRow
    {
        /// <summary>
        ///     Öffentliche Liste der Spaltendefinitionen
        /// </summary>
        private readonly List<FlexColumnDefinition> columnDefinitions = new List<FlexColumnDefinition>();

        /// <summary>
        ///     List of the specialized <see cref="PropertyDescriptor" />s
        /// </summary>
        private readonly List<ColumnPropertyDescriptor> columnDescriptors;

        /// <summary>
        ///     List of all column headers
        /// </summary>
        private readonly List<string> columnHeaders = new List<string>();

        /// <summary>
        ///     List of the values of the pivotized columns
        /// </summary>
        private readonly List<string> columnIdentifierValues = new List<string>();

        /// <summary>
        ///     Constructor
        /// </summary>
        public FlexTable()
        {
            this.columnDescriptors = new List<ColumnPropertyDescriptor>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexTable"/> class.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        public FlexTable(List<FlexColumnDefinition> columnDefinitions)
            : this()
        {
            foreach (var colDef in columnDefinitions)
                this.AddColumn(colDef);
        }

        /// <summary>
        ///     Internal constructor.
        /// </summary>
        /// <param name="columnDescriptors"></param>
        internal FlexTable(List<ColumnPropertyDescriptor> columnDescriptors)
        {
            this.columnDescriptors = columnDescriptors;

            foreach (var propertyDescriptor in this.columnDescriptors)
            {
                this.columnHeaders.Add(propertyDescriptor.DisplayName);
                this.columnIdentifierValues.Add(propertyDescriptor.PropertyName);

                this.ColumnDefinitions.Add(new FlexColumnDefinition(propertyDescriptor.DisplayName,
                    propertyDescriptor.PropertyType, propertyDescriptor.PropertyName,
                    propertyDescriptor.SourcePropertyName));
            }
        }

        /// <summary>
        /// Removes all elements.
        /// </summary>
        public new void Clear()
        {
            // remove all backward pointers from the collection's row
            foreach (var row in this)
                row.OwningCollection = null;

            // clear
            base.Clear();
        }

        /// <summary>
        ///     Returns the list of all column headers
        /// </summary>
        public List<string> ColumnHeaders
        {
            get { return this.columnHeaders; }
        }

        /// <summary>
        ///     Returns the list of the identifying values for the pivotized columns
        /// </summary>
        public List<string> ColumnIdentifierValues
        {
            get { return this.columnIdentifierValues; }
        }

        /// <summary>
        ///     Returns the list of all column definitions
        /// </summary>
        public List<FlexColumnDefinition> ColumnDefinitions
        {
            get { return this.columnDefinitions; }
        }

        /// <summary>
        ///     Add's a column definition
        /// </summary>
        /// <param name="displayName">Column header</param>
        /// <param name="propertyType">Type of the property/column</param>
        /// <param name="propertyName">Name/identifier of the column/property</param>
        /// <param name="sourcePropertyName">Name of the property/column from the data source</param>
        public FlexColumnDefinition AddColumn(string displayName, Type propertyType, string propertyName,
            string sourcePropertyName)
        {
            var col = new FlexColumnDefinition(displayName, propertyType, propertyName, sourcePropertyName);
            this.AddColumn(col);

            //this.columnHeaders.Add(displayName);
            //this.columnIdentifierValues.Add(propertyName);

            return col;
        }

        /// <summary>
        /// Add's a column definition
        /// </summary>
        /// <param name="displayName">Column header (will be used as propertyName and sourcePropertyName)</param>
        /// <param name="propertyType">Type of the property/column</param>
        /// <returns></returns>
        public FlexColumnDefinition AddColumn(string displayName, Type propertyType)
        {
            return this.AddColumn(displayName, propertyType, displayName, displayName);
        }

        /// <summary>
        ///     Adds a column definition
        /// </summary>
        /// <param name="column">the column definition</param>
        public void AddColumn(FlexColumnDefinition column)
        {
            this.ColumnDefinitions.Add(column);
            this.columnHeaders.Add(column.ColumnTitle);
            this.columnIdentifierValues.Add(column.ColumnPropertyName);

            // create a property descriptor, which helps Reflection
            var cpd = new ColumnPropertyDescriptor(column.ColumnPropertyName, column.ColumnTitle, column.ColumnType,
                PivotColumnType.NonPivotColumn, column.SourcePropertyName);
            this.columnDescriptors.Add(cpd);
            column.ColumnPropertyDescriptor = cpd;

            // add the new column to all existing rows of this collection
            foreach (var row in this)
                row.AddColumn(cpd);
        }

        /*
        /// <summary>
        ///     Creates a new data row
        /// </summary>
        /// <param name="uniqueRowIdentifier"></param>
        /// <returns></returns>
        public FlexRow NewRow(string uniqueRowIdentifier)
        {
            var row = new FlexRow(this.columnDescriptors, uniqueRowIdentifier) { OwningCollection = this };
            row.InitializeRow();
            return row;
        }
*/

        /// <summary>
        ///     Creates a new data row of type &lt;T&gt; which must be inherited from <see cref="FlexRow" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public T NewRow(string uniqueIdentifier)
        {
            var row = Activator.CreateInstance<T>();
            row.SetUniqueIdentifier(uniqueIdentifier);
            row.SetPropertyDescriptors(this.columnDescriptors);
            row.OwningCollection = this;
            row.InitializeRow();
            return row;
        }

        /// <summary>
        ///     Returns the <see cref="ColumnPropertyDescriptor.IsBrowsable" /> flag of a property/column
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsBrowsable(string propertyName)
        {
            var def = this.columnDescriptors.Find(cd => cd.Name.Equals(propertyName));
            Debug.Assert(def != null, string.Format("Oops! The property '{0}' does not exist!", propertyName));

            return def.IsBrowsable;
        }

        /// <summary>
        ///     Returns a list of rows filtered by a filter function
        /// </summary>
        /// <param name="filterFunc"></param>
        /// <returns></returns>
        public FlexTable<T> FilteredRows(Func<T, bool> filterFunc)
        {
            var result = new FlexTable<T>(this.columnDescriptors);

            result.AddRange(this.Where(row => filterFunc(row)));

            return result;
        }

        /// <summary>
        ///     Finds a column by its ColumnPropertyName
        /// </summary>
        /// <param name="columnPropertyName"></param>
        /// <returns></returns>
        public FlexColumnDefinition GetColumnByName(string columnPropertyName)
        {
            var colDef = this.columnDefinitions.FirstOrDefault(col => col.ColumnPropertyName == columnPropertyName);
            if (colDef == null)
                throw new ArgumentException("Requested column does not exist", "columnPropertyName");

            return colDef;
        }

        /// <summary>
        /// Returns <c>True</c> if the requested column exists in this collection
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public bool ContainsColumn(string columnPropertyName)
        {
            return this.columnDefinitions.Any(col => col.ColumnPropertyName == columnPropertyName);
        }


        /// <summary>
        /// Returns a row identified by its unique identifier or <c>null</c> if it does not exist
        /// </summary>
        /// <remarks>
        /// Serach is performed using <see cref="System.Linq.FirstOrDefault%lt;T&gt;"/> method.
        /// </remarks>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public FlexRow FindRowByUniqueIdentifier(string uniqueIdentifier)
        {
            return this.FirstOrDefault(row => row.UniqueIdentifier == uniqueIdentifier);
        }

        /// <summary>
        ///     Attaches a row set to the current row set (only new columns, does not overwrite or append)
        /// </summary>
        /// <remarks>
        ///     This will append those columns from the other row set, which are
        ///     new to the current row set. Additionally it will NOT overwrite
        ///     the values of any already existent columns of the current row set.
        ///     It will also not add any rows of the other row set, which have no
        ///     correspondent rows in the current row set.
        ///     Only the first occurence of the matching value in the other row set
        ///     will be merged (this is non-deterministic!)
        /// </remarks>
        /// <param name="other">the other row set</param>
        /// <param name="myMatchColumn">column name to match on my side</param>
        /// <param name="otherMatchColumn">column name to match on the other side</param>
        public void Attach(FlexTable<T> other, string myMatchColumn, string otherMatchColumn)
        {
            // get the list of columns, which are "new" to the current row set.
            var colList =
                other.ColumnDefinitions.Where(
                    col => !this.ColumnDefinitions.Any(myCol => myCol.ColumnPropertyName.Equals(col.ColumnPropertyName)))
                    .ToList();
            if (colList.Count == 0) return; // nothing to do, so we do not need to scan

            // add the columns of the other list
            foreach (var column in colList)
                this.AddColumn(column.ColumnTitle, column.ColumnType, column.ColumnPropertyName,
                    column.SourcePropertyName);

            // now loop over the rows of the current row set
            // try to match a corresponding row in the other row set
            foreach (var row in this)
            {
                // get my matching key
                var myKey = row[myMatchColumn];
                // skip this one if the key is null or empty
                if (myKey == null || (myKey is string && string.IsNullOrEmpty((string)myKey)))
                    continue;

                // find the first occurence of the key on the other side
                var otherRow = other.FirstOrDefault(r => r[otherMatchColumn].Equals(myKey));

                // copy the data
                if (otherRow != null)
                    foreach (var column in colList)
                        row[column.ColumnPropertyName] = otherRow[column.ColumnPropertyName];
            }
        }
    }
}