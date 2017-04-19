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
        public Type ColumnType { get; private set; }

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

        /// <summary>
        /// A tag that can be applied to the column by the application. Not used internally.
        /// </summary>
        //public object Tag { get; set; }

        //private GUISettings.ViewModel.GUIElementGroupAssignmentViewModel assignedGuiElement;
        //private GUISettings.ViewModel.GUIElementBaseViewModel guiElement;

        ///// <summary>
        ///// Gets or sets the assigned GUI element with its group specific settings.
        ///// </summary>
        //public GUISettings.ViewModel.GUIElementGroupAssignmentViewModel AssignedGuiElement 
        //{
        //    get { return this.assignedGuiElement; }
        //    set
        //    {
        //        this.assignedGuiElement = value;
        //        this.guiElement = value.GetAssignedBaseElement();
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the GUI element (independent of any group specific settings).
        ///// </summary>
        //public GUISettings.ViewModel.GUIElementBaseViewModel GuiElement 
        //{
        //    get { return this.guiElement; }
        //    set { this.guiElement = value; }
        //}

        /// <summary>
        /// Changes the type of the column.
        /// </summary>
        /// <param name="newType">The new type.</param>
        public void ChangeColumnType(Type newType)
        {
            this.ColumnType = newType;
        }
    }
}
