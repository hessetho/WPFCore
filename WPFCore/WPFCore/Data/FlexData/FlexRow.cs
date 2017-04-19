using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using WPFCore.ViewModelSupport;

namespace WPFCore.Data.FlexData
{
    public class FlexRow : ViewModelCore, ICustomTypeDescriptor
    {
        /// <summary>
        ///     Liste der <see cref="PropertyDescriptor" />en der Werte
        /// </summary>
        private PropertyDescriptorCollection propertyDescriptors;

        /// <summary>
        ///     Speichert die Werte der einzelnen Spalten
        /// </summary>
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        protected Dictionary<string, object> Values
        {
            get { return this.values; }
        }


        /// <summary>
        /// Stores the values for any proxy-type column
        /// </summary>
        protected readonly Dictionary<string, object> proxyValues = new Dictionary<string, object>();

        public FlexRow()
        {
        }

        /// <summary>
        ///     Konstruktor. Initialisiert die Datenzeile
        /// </summary>
        /// <param name="propertyDescriptors"></param>
        /// <param name="uniqueIdentifier"></param>
        internal FlexRow(List<ColumnPropertyDescriptor> propertyDescriptors, string uniqueIdentifier)
        {
            // Kopie der ColumnPropteryDescriptoren anlegen, welche von ICustomTypeDescriptor dann verwendet wird
            // ToDo: muss hier eine Kopie erstellt werden? Oder reicht eine einmalig eingerichtete Liste auch? (spart dann Zeit und Speicher)
            this.SetPropertyDescriptors(propertyDescriptors);
            this.UniqueIdentifier = uniqueIdentifier;
        }

        /// <summary>
        /// Returns the collection which owns this row
        /// </summary>
        [Browsable(false)]
        public object OwningCollection { get; set; }

        /// <summary>
        /// Returns the cells of the row
        /// </summary>
        public List<object> Cells
        {
            get { return this.values.Values.ToList(); }
        }

        public List<object> PivotCells
        {
            get
            {
                var result = from pivotColumn in this.PivotColumnProperties
                             join cell in this.values on pivotColumn.PropertyName equals cell.Key
                             select cell.Value;
                return result.ToList();
            }
        }

        /// <summary>
        /// Give inherited instances access to the property descriptors
        /// </summary>
        protected PropertyDescriptorCollection PropertyDescriptors
        {
            get { return this.propertyDescriptors; }
        }

        /// <summary>
        /// Sets the property descriptors for the columns.
        /// </summary>
        /// <param name="propertyDescriptors">The column property descriptors.</param>
        internal void SetPropertyDescriptors(List<ColumnPropertyDescriptor> propertyDescriptors)
        {
            var allPropertyDescriptors = new List<PropertyDescriptor>(propertyDescriptors);

            // collect all "original" properties of the current instance and add them to the "artificial" properties
            foreach (PropertyDescriptor nativePropDesc in TypeDescriptor.GetProperties(this, true))
                allPropertyDescriptors.Add(nativePropDesc);

            // and convert the list into a proper colelction for later use (see GetProperties)
            this.propertyDescriptors = new PropertyDescriptorCollection(allPropertyDescriptors.ToArray());

            // create the "columns" from the supplied property descriptors
            foreach (ColumnPropertyDescriptor pdc in propertyDescriptors)
                this.AddColumn(pdc);
        }

        /// <summary>
        /// Allows derived classes to implement initial row initialization
        /// </summary>
        /// <remarks>
        /// This method is called from <see cref="FlexTable.NewRow"/>
        /// and <see cref="FlexTable.NewRow;lt&T;gt&"/>.
        /// </remarks>
        protected internal virtual void InitializeRow()
        {
        }

        /// <summary>
        /// Allows derived classes to implement initial column initialization
        /// </summary>
        /// <remarks>
        /// This method is called from <see cref="AddColumn"/>
        /// </remarks>
        protected internal virtual void InitializeColumn(string columnName)
        {
        }

        /// <summary>
        /// Initializes the cell.
        /// </summary>
        /// <remarks>
        /// This method is called from <see cref="this"/>
        /// </remarks>
        /// <param name="cellValue">The cell value.</param>
        protected internal virtual void InitializeCell(object cellValue)
        {
        }

        /// <summary>
        /// Disposes the cell.
        /// </summary>
        /// <remarks>
        /// This method is called from <see cref="this"/>
        /// </remarks>
        /// <param name="cellValue">The cell value.</param>
        protected internal virtual void DisposeCell(object cellValue)
        {
        }

        internal void SetUniqueIdentifier(string uniqueIdentifier)
        {
            this.UniqueIdentifier = uniqueIdentifier;
        }

        internal void AddColumn(ColumnPropertyDescriptor pdc)
        {
            Debug.Assert(this.values.ContainsKey(pdc.PropertyName) == false, string.Format("The property {0} is already part of the column list.", pdc.PropertyName));

            if (pdc.PropertyType == typeof(ProxyProperty))
                this.values.Add(pdc.PropertyName, new ProxyProperty(this, pdc.PropertyName, pdc.DisplayName));
            else
                this.values.Add(pdc.PropertyName, null);

            this.InitializeColumn(pdc.PropertyName);

            if (this.propertyDescriptors.Contains(pdc) == false)
                this.propertyDescriptors.Add(pdc);
        }

        /// <summary>
        /// Returns <c>True</c> if the requested column exists in this row
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        public bool ContainsColumn(string colName)
        {
            return this.values.ContainsKey(colName);
        }

        /// <summary>
        ///     Liefert den Wert einer benannten Spalte bzw. legt diesen fest
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object this[string propertyName]
        {
            get
            {
                // return the value from the internal buffer

                if (this.values.Keys.Contains(propertyName))
                    return this.values[propertyName];

                // use the qualified name by including the type
                var qualifiedPropertyName = string.Format("{0}.{1}", this.GetType().Name, propertyName);
                if (this.values.Keys.Contains(qualifiedPropertyName))
                    return this.values[qualifiedPropertyName];

                throw new ArgumentException(string.Format("Oops! The requested property name ([{1}.]{0}) does not exist as key", propertyName, this.GetType().Name), "propertyName");
            }
            set
            {
                if (value == DBNull.Value) value = null;
#if DEBUG
                if (!this.values.Keys.Contains(propertyName))
                    throw new ArgumentException(string.Format("Oops! The requested property name ({0}) does not exist as key", propertyName), "identifier");
                if (this.propertyDescriptors[propertyName].PropertyType == typeof(ProxyProperty))
                    throw new InvalidOperationException(string.Format("Oops! Your trying to write to a proxy-style property ({0})", propertyName));
                if (value != null && this.propertyDescriptors[propertyName].PropertyType != typeof(object))
                    //if (this.propertyDescriptors[propertyName].PropertyType != value.GetType())
                    if (!this.propertyDescriptors[propertyName].PropertyType.IsAssignableFrom(value.GetType()))
                        throw new InvalidOperationException(string.Format("Ouch! The properties' ({0}) type ({1}) and the values' type ({2}) are not the same or can not be inherited from.", propertyName, this.propertyDescriptors[propertyName].PropertyType, value.GetType()));
#endif
                //object val = null;
                //if (value != DBNull.Value && value != null)
                //    val = Convert.ChangeType(value, this.propertyDescriptors[propertyName].PropertyType);

                // if the current value is about to be set to NULL and if there was a previous value in the cell
                // allow the owner (the row) to do some disposal work
                if (value == null || (value is string && string.IsNullOrEmpty((string)value)))
                    if (this.values[propertyName] != null)
                        this.DisposeCell(this.values[propertyName]);

                // now store the new value
                this.values[propertyName] = value;

                // if the new value is not NULL, 
                // allow the owner (the row) to do some initializations
                if (value != null)
                    this.InitializeCell(value);

                this.OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        ///     Liefert den Wert einer indexierten Spalte
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index]
        {
            get { return this.values.ElementAt(index); }
        }

        /// <summary>
        ///     Liefert den eindeutigen Schlüssel für diese Datenzeile
        /// </summary>
        public string UniqueIdentifier { get; private set; }

        #region Implementation of ICustomTypeDescriptor

        /// <summary>
        ///     Gibt eine Auflistung benutzerdefinierter Attribute für diese Instanz einer Komponente zurück.
        /// </summary>
        /// <returns>
        ///     Eine <see cref="T:System.ComponentModel.AttributeCollection" /> mit den Attributen für dieses Objekt.
        /// </returns>
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return null;
        }

        /// <summary>
        ///     Gibt den Klassennamen dieser Instanz einer Komponente zurück.
        /// </summary>
        /// <returns>
        ///     Der Klassenname des Objekts oder null, wenn die Klasse keinen Namen hat.
        /// </returns>
        string ICustomTypeDescriptor.GetClassName()
        {
            return this.GetType().ToString();
        }

        /// <summary>
        ///     Gibt den Namen dieser Instanz einer Komponente zurück.
        /// </summary>
        /// <returns>
        ///     Der Name des Objekts oder null, wenn das Objekt keinen Namen hat.
        /// </returns>
        string ICustomTypeDescriptor.GetComponentName()
        {
            return null;
        }

        /// <summary>
        ///     Gibt einen Typkonverter für diese Instanz einer Komponente zurück.
        /// </summary>
        /// <returns>
        ///     Ein <see cref="T:System.ComponentModel.TypeConverter" />, bei dem es sich um den Konverter für dieses Objekt handelt, oder null, wenn für dieses Objekt kein
        ///     <see
        ///         cref="T:System.ComponentModel.TypeConverter" />
        ///     vorhanden ist.
        /// </returns>
        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return null;
        }

        /// <summary>
        ///     Gibt das Standardereignis für diese Instanz einer Komponente zurück.
        /// </summary>
        /// <returns>
        ///     Ein <see cref="T:System.ComponentModel.EventDescriptor" />, der das Standardereignis dieses Objekts darstellt, oder null, wenn dieses Objekt keine Ereignisse aufweist.
        /// </returns>
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }

        /// <summary>
        ///     Gibt die Standardeigenschaft für diese Instanz einer Komponente zurück.
        /// </summary>
        /// <returns>
        ///     Ein <see cref="T:System.ComponentModel.PropertyDescriptor" />, der die Standardeigenschaft dieses Objekts darstellt, oder null, wenn dieses Objekt keine Eigenschaften aufweist.
        /// </returns>
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        /// <summary>
        ///     Gibt einen Editor vom angegebenen Typ für diese Instanz einer Komponente zurück.
        /// </summary>
        /// <returns>
        ///     Ein <see cref="T:System.Object" /> vom angegebenen Typ, bei dem es sich um den Editor für dieses Objekt handelt, oder null, wenn der Editor nicht gefunden werden kann.
        /// </returns>
        /// <param name="editorBaseType">
        ///     Ein <see cref="T:System.Type" />, der den Editor für dieses Objekt darstellt.
        /// </param>
        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return null;
        }

        /// <summary>
        ///     Gibt die Ereignisse für diese Instanz einer Komponente zurück.
        /// </summary>
        /// <returns>
        ///     Eine <see cref="T:System.ComponentModel.EventDescriptorCollection" />, die die Ereignisse für diese Komponenteninstanz darstellt.
        /// </returns>
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return new EventDescriptorCollection(new EventDescriptor[] { });
        }

        /// <summary>
        ///     Gibt die Ereignisse für diese Instanz einer Komponente unter Verwendung des angegebenen Attributarrays als Filter zurück.
        /// </summary>
        /// <returns>
        ///     Eine <see cref="T:System.ComponentModel.EventDescriptorCollection" />, die die gefilterten Ereignisse für diese Komponenteninstanz darstellt.
        /// </returns>
        /// <param name="attributes">
        ///     Ein Array vom Typ <see cref="T:System.Attribute" />, das als Filter verwendet wird.
        /// </param>
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return new EventDescriptorCollection(new EventDescriptor[] { });
        }

        /// <summary>
        ///     Gibt die Eigenschaften für diese Instanz einer Komponente zurück.
        /// </summary>
        /// <returns>
        ///     Eine <see cref="T:System.ComponentModel.PropertyDescriptorCollection" />, die die Eigenschaften für diese Komponenteninstanz darstellt.
        /// </returns>
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return this.propertyDescriptors;
        }

        /// <summary>
        ///     Gibt die Eigenschaften für diese Instanz einer Komponente unter Verwendung des Attributarrays als Filter zurück.
        /// </summary>
        /// <returns>
        ///     Eine <see cref="T:System.ComponentModel.PropertyDescriptorCollection" />, die die gefilterten Eigenschaften für diese Komponenteninstanz darstellt.
        /// </returns>
        /// <param name="attributes">
        ///     Ein Array vom Typ <see cref="T:System.Attribute" />, das als Filter verwendet wird.
        /// </param>
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return this.propertyDescriptors;
        }

        /// <summary>
        ///     Gibt ein Objekt zurück, in dem die vom angegebenen Eigenschaftenbezeichner beschriebene Eigenschaft enthalten ist.
        /// </summary>
        /// <returns>
        ///     Ein <see cref="T:System.Object" />, das den Besitzer der angegebenen Eigenschaft darstellt.
        /// </returns>
        /// <param name="pd">
        ///     Ein <see cref="T:System.ComponentModel.PropertyDescriptor" />, der die Eigenschaft darstellt, deren Besitzer gesucht werden soll.
        /// </param>
        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        internal List<ColumnPropertyDescriptor> PivotColumnProperties
        {
            get
            {
                return this.GetColumnProperties(PivotColumnType.PivotColumn);
            }
        }

        internal List<ColumnPropertyDescriptor> NonPivotColumnProperties
        {
            get
            {
                return this.GetColumnProperties(PivotColumnType.NonPivotColumn);
            }
        }

        internal List<ColumnPropertyDescriptor> RowHeaderColumnProperties
        {
            get
            {
                return this.GetColumnProperties(PivotColumnType.RowHeader);
            }
        }

        internal List<ColumnPropertyDescriptor> ComputedColumnProperties
        {
            get
            {
                return this.GetColumnProperties(PivotColumnType.ComputedColumn);
            }
        }

        internal List<ColumnPropertyDescriptor> GetColumnProperties(PivotColumnType columnType)
        {
            return this.propertyDescriptors.Cast<ColumnPropertyDescriptor>().Where(pdc => pdc.PivotColumnType == columnType).ToList();
        }

        [Conditional("DEBUG")]
        private void DumpProperties()
        {
            foreach (ColumnPropertyDescriptor pd in this.propertyDescriptors)
            {
                Debug.WriteLine(string.Format("{0}: {1} ({2})", pd.Name, pd.DisplayName, pd.PropertyType));
            }
        }
    }
}
