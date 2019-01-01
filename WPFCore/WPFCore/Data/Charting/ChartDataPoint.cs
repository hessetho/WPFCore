using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.Data.Charting
{
    public class ChartDataPoint : ICustomTypeDescriptor, INotifyPropertyChanged
    {
        // (Statische) Liste der Properties eines Datenpunktes für ICustomTypeDescriptor
        private static PropertyDescriptorCollection properties;

        // (Statische) Liste der dynamischen Properties eines Datenpuntes
        private static string[] dataPointNames;

        // Liste der Werte eines Datenpunktes
        private readonly Dictionary<string, MMAValue> values = new Dictionary<string, MMAValue>();

        /// <summary>
        /// Wird ausgelöst, wenn sich eine Property geändert hat
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isInitialized = false;

        protected ChartDataPoint()
        {
            this.isInitialized = false;
        }

        /// <summary>
        /// Initialisiert einen neuen Datenpunkt
        /// </summary>
        /// <param name="dataPointPropertyNames"></param>
        internal void Initialize(string[] dataPointPropertyNames)
        {
            // Die dynamischen Properties registrieren
            this.InitPropertyDescriptors(dataPointPropertyNames);

            // Alle Werte vorbelegen
            foreach (var valueName in dataPointPropertyNames)
                this.values.Add(valueName, new MMAValue());

            this.isInitialized = true;
        }
        /// <summary>
        /// Initialisiert einen neuen Datenpunkt
        /// </summary>
        internal void Initialize()
        {
            // Alle Werte vorbelegen
            foreach (var valueName in dataPointNames)
            {
                CheckDataPointName(valueName);
                this.values.Add(valueName, new MMAValue());
            }

            this.isInitialized = true;
        }

        [Conditional("DEBUG")]
        private void CheckInit()
        {
            if (!this.isInitialized)
                throw new InvalidOperationException("ChartDataPoint is not initialized!");
        }

        [Conditional("DEBUG")]
        private void CheckDataPointName(string dataPointName)
        {
            if(!properties.OfType<DPPropertyDescriptor>().Select(p => p.Name).Contains(dataPointName))
                throw new InvalidOperationException(string.Format("ChartDataPoint is not initialized correctly! Unknown data point name: {0}", dataPointName));
        }

        /// <summary>
        /// Liefert (oder setzt) den Wert einer Property des Datenpunktes.
        /// </summary>
        /// <remarks>
        /// Ist eine Eigenschaft (<paramref name="propertyName"/>) bei abfragen nicht vorhanden ist, wird stets <c>null</c> zurück gegeben.
        /// Ist sie beim setzen des Wertes nicht vorhanden, wird sie angelegt.
        /// </remarks>
        /// <param name="propertyName">Name der Eigenschaft.</param>
        /// <returns></returns>
        public double? this[string propertyName]
        {
            get
            {
                this.CheckInit();

                if(this.values.ContainsKey(propertyName))
                    // Immer den zuletzt zugewiesenen Wert liefern
                    return this.values[propertyName].Last;

                // wenn die abgefragte Eigenschaft nicht vorhanden ist, geben wir stets NULL zurück
                return (double?)null;
            }
            set
            {
                this.CheckInit();
                if (!this.values.ContainsKey(propertyName))
                    this.values.Add(propertyName, new MMAValue(value));
                else
                    this.values[propertyName].SetValue(value);

                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Liefert alle Werte des Datenpunktes
        /// </summary>
        [Browsable(false)]
        public List<MMAValue> Values
        {
            get
            {
                this.CheckInit();
                return this.values.Values.ToList();
            }
        }

        #region ICustomTypeDescriptor
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return null;
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return this.GetType().ToString();
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return null;
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return null;
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return null;
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return new EventDescriptorCollection(new EventDescriptor[] { });
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return new EventDescriptorCollection(new EventDescriptor[] { });
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return properties;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return properties;
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion ICustomTypeDescriptor

        /// <summary>
        /// Initialisiert die <see cref="PropertyDescriptorCollection"/>
        /// </summary>
        /// <param name="propertyNames"></param>
        private void InitPropertyDescriptors(string[] propertyNames)
        {
            var pdcList = new List<PropertyDescriptor>();

            // Alle "originalen" Properties dieser Klasse ebenfalls hinzufügen (falls vorhanden, z.B. "Date")
            foreach (PropertyDescriptor nativePropDesc in TypeDescriptor.GetProperties(this, true))
                if (nativePropDesc.Name != "Values")
                    pdcList.Add(nativePropDesc);

            // Nun die Properties des Datenpunktes als Properties anmelden
            foreach (var propertyName in propertyNames)
                pdcList.Add(new DPPropertyDescriptor(propertyName));

            properties = new PropertyDescriptorCollection(pdcList.ToArray(), true);
            dataPointNames = propertyNames;
        }

        
    }
}
