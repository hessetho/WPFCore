using System;
using System.ComponentModel;

namespace WPFCore.Data.FlexData
{
    public enum PivotColumnType
    {
        RowHeader,
        PivotColumn,
        NonPivotColumn,
        ComputedColumn
    }

    /// <summary>
    ///     Spezialisierter <see cref="PropertyDescriptor" />
    /// </summary>
    public class ColumnPropertyDescriptor : PropertyDescriptor, IComparable
    {
        private readonly string description;
        private readonly string displayName;
        private readonly string propertyName;
        private readonly bool isBrowsable;
        private readonly Type propertyType;
        private readonly string sourcePropertyName;
        private PivotColumnType pivotColumnType;

        #region Konstruktoren

        public ColumnPropertyDescriptor(string propertyName, string displayName, Type propertyType, PivotColumnType pivotColumnType, string sourcePropertyName)
            : this(propertyName, displayName, propertyType, pivotColumnType, sourcePropertyName, true, string.Empty, new Attribute[] {})
        {
        }

        public ColumnPropertyDescriptor(string propertyName, string displayName, Type propertyType, PivotColumnType pivotColumnType, string sourcePropertyName,
                                        bool isBrowsable)
            : this(propertyName, displayName, propertyType, pivotColumnType, sourcePropertyName, isBrowsable, string.Empty, new Attribute[] { })
        {
        }

        public ColumnPropertyDescriptor(string propertyName, string displayName, Type propertyType, PivotColumnType pivotColumnType, string sourcePropertyName,
                                        bool isBrowsable, string description, Attribute[] attrs)
            : base(propertyName, attrs)
        {
            this.propertyName = propertyName;
            this.displayName = displayName;
            this.propertyType = propertyType;
            this.isBrowsable = isBrowsable;
            this.description = description;
            this.pivotColumnType = pivotColumnType;
            this.sourcePropertyName = sourcePropertyName;

            foreach (Attribute attrib in attrs)
            {
                if (attrib is DisplayNameAttribute)
                    this.displayName = ((DisplayNameAttribute) attrib).DisplayName;
                if (attrib is DescriptionAttribute)
                    this.description = ((DescriptionAttribute) attrib).Description;
                if (attrib is BrowsableAttribute)
                    this.isBrowsable = ((BrowsableAttribute) attrib).Browsable;
            }
        }

        #endregion

        public int Ordinal { get; set; }

        public override string DisplayName
        {
            get { return this.displayName; }
        }

        public override string Description
        {
            get { return this.description; }
        }

        public override bool IsBrowsable
        {
            get { return this.isBrowsable; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type ComponentType
        {
            get { return typeof (FlexRow); }
        }

        public override Type PropertyType
        {
            get { return this.propertyType; }
        }

        public string PropertyName
        {
            get { return this.propertyName; }
        }

        public bool IsRowHeader
        {
            get { return this.pivotColumnType == PivotColumnType.RowHeader; }
        }

        public bool IsNonPivotColumn
        {
            get { return this.pivotColumnType == PivotColumnType.NonPivotColumn; }
        }

        public string SourcePropertyName
        {
            get { return this.sourcePropertyName; }
        }

        internal PivotColumnType PivotColumnType
        {
            get { return this.pivotColumnType; }
        }
        
        public override object GetValue(object component)
        {
            if (this.isBrowsable)
            {
                var flexRow = component as FlexRow;
                if (flexRow == null)
                    throw new ArgumentException("component is of wrong type. Must be of type DynamicDataRow or FlexRow", "component");

                return flexRow[this.PropertyName];
            }

            return null;
        }

        public override void SetValue(object component, object value)
        {
            var flexRow = component as FlexRow;
            if (flexRow == null)
                throw new ArgumentException("component is of wrong type. Must be of type DynamicDataRow or FlexRow", "component");

            flexRow[this.PropertyName] = value;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override string ToString()
        {
            return
                string.Format(
                    "identifier: {0}\tdisplayName: {1}\tpropertyType: {2}\tpivotColumnType: {3}\tisBrowsable: {4}",
                    this.propertyName, this.displayName, this.propertyType, this.pivotColumnType, this.isBrowsable);
        }

        #region Implementation of IComparable

        /// <summary>
        ///     Vergleicht die aktuelle Instanz mit einem anderen Objekt vom selben Typ und gibt eine ganze Zahl zurück, die angibt, ob die aktuelle Instanz in der Sortierreihenfolge vor oder nach dem anderen Objekt oder an derselben Position auftritt.
        /// </summary>
        /// <returns>
        ///     Ein Wert, der die relative Reihenfolge der verglichenen Objekte angibt. Der Rückgabewert hat folgende Bedeutung: Wert  Bedeutung  Kleiner als 0  Diese Instanz geht
        ///     <paramref
        ///         name="obj" />
        ///     in der Sortierreihenfolge voraus.  Zero  Diese Instanz tritt an der gleichen Position in der Sortierreihenfolge wie
        ///     <paramref
        ///         name="obj" />
        ///     auf.  Größer als 0 (null)  Diese Instanz folgt <paramref name="obj" /> in der Sortierreihenfolge.
        /// </returns>
        /// <param name="obj">Ein Objekt, das mit dieser Instanz verglichen werden soll.</param>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="obj" /> hat nicht denselben Typ wie diese Instanz.
        /// </exception>
        public int CompareTo(object obj)
        {
            var other = obj as ColumnPropertyDescriptor;
            if (other == null)
                return 1;

            if (this.PropertyName == other.PropertyName)
                return 0;

            return -1;
        }

        #endregion
    }
}
