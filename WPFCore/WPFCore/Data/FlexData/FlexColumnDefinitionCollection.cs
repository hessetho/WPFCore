using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace WPFCore.Data.FlexData
{
    /// <summary>
    ///     Sortierte Liste mit <see cref="ColumnDefinition" />-Objekten.
    /// </summary>
    public class FlexColumnDefinitionCollection : IList, ICollection, IEnumerable
    {
        private static int sequence;

        private readonly Dictionary<string, FlexColumnDefinition> columnDictionary = new Dictionary<string, FlexColumnDefinition>();
        private readonly List<FlexColumnDefinition> columns = new List<FlexColumnDefinition>();

        public FlexColumnDefinitionCollection()
        {
            this.CollectionName = string.Format("~coll{0}", sequence++);
        }

        /// <summary>
        /// Liefert die Bezeichnung der Spaltenliste bzw. legt diese fest
        /// </summary>
        [XmlIgnore]
        public string CollectionName { get; set; }

        public int Count => ((ICollection)columnDictionary).Count;

        public object SyncRoot => ((ICollection)columnDictionary).SyncRoot;

        public bool IsSynchronized => ((ICollection)columnDictionary).IsSynchronized;

        public bool IsReadOnly { get { return false; } }

        public bool IsFixedSize { get { return false; } }

        public object this[int index]
        {
            get
            {
                return this.columns[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Gets or sets the ColumnDefinition with the specified column name.
        /// </summary>
        /// <value>The ColumnDefinition.</value>
        public FlexColumnDefinition this[string columnPropertyName]
        {
            get { return this.columnDictionary[columnPropertyName]; }
        }

        /// <summary>
        ///     Determines whether the column list contains the specified column name.
        /// </summary>
        /// <param name="columnPropertyName">Name of the column.</param>
        /// <returns>
        ///     <c>true</c> if the column list contains the specified column name; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string columnPropertyName)
        {
            return this.columnDictionary.ContainsKey(columnPropertyName);
        }

        public List<FlexColumnDefinition> AsList()
        {
            return this.columns;
        }

        /// <summary>
        ///     Adds the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <returns></returns>
        public FlexColumnDefinition Add(string columnTitle, Type columnType, string columnPropertyName)
        {
            var newColDefinition = new FlexColumnDefinition(columnTitle, columnType, columnPropertyName);

            this.columnDictionary.Add(columnPropertyName, newColDefinition);
            this.columns.Add(newColDefinition);

            newColDefinition.Index = this.Count;
            newColDefinition.ParentCollection = this;

            return newColDefinition;
        }

        public void Dump()
        {
            foreach (var columnDefinition in this)
                Debug.WriteLine(columnDefinition.ToString());
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)columnDictionary).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return this.columns.GetEnumerator();
        }

        public int Add(object value)
        {
            var flexCol = (FlexColumnDefinition)value;
            if (this.columnDictionary.ContainsKey(flexCol.ColumnPropertyName))
                throw new ArgumentException();

            this.columnDictionary.Add(flexCol.ColumnPropertyName, flexCol);
            this.columns.Add(flexCol);

            return this.columns.IndexOf(flexCol);
        }

        public bool Contains(object value)
        {
            var flexCol = (FlexColumnDefinition)value;
            return this.Contains(flexCol.ColumnPropertyName);
        }

        public void Clear()
        {
            this.columnDictionary.Clear();
            this.columns.Clear();
        }

        public int IndexOf(object value)
        {
            var flexCol = (FlexColumnDefinition)value;
            return this.columns.IndexOf(flexCol);
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            var flexCol = (FlexColumnDefinition)value;
            this.columnDictionary.Remove(flexCol.ColumnPropertyName);
            this.columns.Remove(flexCol);
        }

        public void RemoveAt(int index)
        {
            var flexCol = this.columns[index];
            this.columnDictionary.Remove(flexCol.ColumnPropertyName);
            this.columns.Remove(flexCol);
        }
    }
}