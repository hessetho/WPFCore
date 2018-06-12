using System;

namespace WPFCore.Data.FlexData
{
    /// <summary>
    ///     Beschreibt die Definition einer Spalte der Pivottabelle
    /// </summary>
    public class FlexColumnDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlexColumnDefinition"/> class.
        /// </summary>
        /// <param name="columnTitle">The column title.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <param name="columnPropertyName">The column's internal property name.</param>
        public FlexColumnDefinition(string columnTitle, Type columnType, string columnPropertyName) : this(columnTitle, columnType, columnPropertyName, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlexColumnDefinition"/> class.
        /// </summary>
        /// <param name="columnTitle">The column title.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <param name="columnPropertyName">The column's internal property name.</param>
        /// <param name="sourcePropertyName">For pivot tables only: name of the property that provides this collection's columns.</param>
        public FlexColumnDefinition(string columnTitle, Type columnType, string columnPropertyName, string sourcePropertyName)
        {
            this.ColumnTitle = columnTitle;
            this.ColumnType = columnType;
            this.ColumnPropertyName = columnPropertyName;
            this.SourcePropertyName = sourcePropertyName;
        }

        /// <summary>
        /// Gets the columns title
        /// </summary>
        public string ColumnTitle { get; private set; }

        /// <summary>
        /// Gets the data type of the column
        /// </summary>
        public Type ColumnType { get; set; }

        /// <summary>
        /// Gets the internal name of the column. This is used to identify the column.
        /// </summary>
        /// <remarks>
        /// Pivot table: created from the values defined by <see cref="SourcePropertyName"/>
        /// </remarks>
        public string ColumnPropertyName { get; private set; }

        /// <summary>
        /// Gets the property name of the data source, which (usually) corresponds to a physical column name in the data source.
        /// </summary>
        public string SourcePropertyName { get; private set; }


        public int Index { get; internal set; }

        public FlexColumnDefinitionCollection ParentCollection { get; internal set; }

        /// <summary>
        /// Changes the type of the column.
        /// </summary>
        /// <param name="newType">The new type.</param>
        public void ChangeColumnType(Type newType)
        {
            this.ColumnType = newType;
        }

        public ColumnPropertyDescriptor ColumnPropertyDescriptor { get; internal set; }
    }
}
