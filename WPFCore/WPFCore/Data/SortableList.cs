// ==========================================================================
// <summary>
// WpfHelper: Sammlung von Hilfsfunktionen und Klassen zur Verwendung mit WPF
// </summary>
//
// $Rev: 94 $
// $Id: SortableList.cs 94 2010-12-13 14:19:17Z  $
// 
// <copyright file="SortableList.cs" company="ICEP GmbH">
//      2009-2012 ICEP GmbH, T. Hesse
// </copyright>
// ==========================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WPFCore.Data
{
    /// <summary>
    ///     Erweiterte BindingList&lt;T&gt;-Klasse, welche das Sortieren unterstützt
    /// </summary>
    /// <remarks>
    ///     Quelle: http://groups.google.co.uk/group/microsoft.public.dotnet.languages.csharp/msg/2b7528c689f9ef84
    /// </remarks>
    /// <example>
    /// </example>
    /// <typeparam name="T">Typ der Listenelemente</typeparam>
    public class SortableList<T> : BindingList<T>, IBindingListView
    {
        /// <summary>
        ///     Collection of sort definitions.
        /// </summary>
        private PropertyComparerCollection<T> sorts;

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="SortableList&lt;T&gt;" />-Klasse.
        /// </summary>
        /// <param name="list">Eine Liste mit den zu sortierenden Elementen.</param>
        public SortableList(List<T> list)
            : base(list)
        {
        }

        /// <summary>
        ///     Ruft einen Wert ab, der angibt, ob die Liste sortiert ist.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     true, wenn die Liste sortiert ist, andernfalls false. Der Standardwert ist false.
        /// </returns>
        protected override bool IsSortedCore
        {
            get { return this.sorts != null; }
        }

        /// <summary>
        ///     Ruft einen Wert ab, der angibt, ob die Liste Sortiervorgänge unterstützt.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     true, wenn die Liste die Sortierung unterstützt, andernfalls false. Der Standardwert ist false.
        /// </returns>
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        /// <summary>
        ///     Ruft die Sortierrichtung der Liste ab.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     Einer der <see cref="T:System.ComponentModel.ListSortDirection" />-Werte. Der Standardwert ist
        ///     <see cref="F:System.ComponentModel.ListSortDirection.Ascending" />.
        /// </returns>
        protected override ListSortDirection SortDirectionCore
        {
            get { return this.sorts == null ? ListSortDirection.Ascending : this.sorts.PrimaryDirection; }
        }

        /// <summary>
        ///     Ruft den Eigenschaftendeskriptor auf, mit dem die Liste sortiert wird, wenn die Liste in einer abgeleiteten Klasse
        ///     sortiert wird, andernfalls null.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     Der zum Sortieren der Liste verwendete <see cref="T:System.ComponentModel.PropertyDescriptor" />.
        /// </returns>
        protected override PropertyDescriptor SortPropertyCore
        {
            get { return this.sorts == null ? null : this.sorts.PrimaryProperty; }
        }

        /// <summary>
        ///     Ruft die Auflistung der momentan auf die Datenquelle angewendeten Sortierbeschreibungen ab.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     Die momentan auf die Datenquelle angewendete <see cref="T:System.ComponentModel.ListSortDescriptionCollection" />.
        /// </returns>
        ListSortDescriptionCollection IBindingListView.SortDescriptions
        {
            get { return this.sorts.Sorts; }
        }

        /// <summary>
        ///     Ruft einen Wert ab, der angibt, ob die Datenquelle erweiterte Sortierung unterstützt.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     true, wenn die Datenquelle erweiterte Sortierung unterstützt, andernfalls false.
        /// </returns>
        bool IBindingListView.SupportsAdvancedSorting
        {
            get { return true; }
        }

        /// <summary>
        ///     Ruft einen Wert ab, der angibt, ob die Datenquelle Filterung unterstützt.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     true, wenn die Datenquelle Filterung unterstützt, andernfalls false.
        /// </returns>
        bool IBindingListView.SupportsFiltering
        {
            get { return false; }
        }

        /// <summary>
        ///     Ruft den Filter ab, mit dem Elemente aus der Auflistung von Elementen, die von der Datenquelle zurückgegeben wird,
        ///     ausgeschlossen werden, oder legt diesen fest.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     Die Zeichenfolge, anhand derer die Elemente in der Auflistung von Elementen, die von der Datenquelle zurückgegeben
        ///     wird, ausgefiltert werden.
        /// </returns>
        string IBindingListView.Filter
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        ///     Entfernt den gegenwärtig auf die Datenquelle angewendeten Filter.
        /// </summary>
        void IBindingListView.RemoveFilter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Apply sorting to the collection
        /// </summary>
        /// <param name="sortCollection">The sort collection.</param>
        public void ApplySort(ListSortDescriptionCollection sortCollection)
        {
            // Stop raising of events (preserve current state)
            var oldRaise = this.RaiseListChangedEvents;
            this.RaiseListChangedEvents = false;

            try
            {
                // sort the items into a temporary list by using the new sort descriptions 
                var tmp = new PropertyComparerCollection<T>(sortCollection);
                var items = new List<T>(this);
                items.Sort(tmp);

                // apply the sorting to the actual list of items
                var index = 0;
                foreach (var item in items)
                {
                    this.SetItem(index++, item);
                }

                // and keep the sort descriptions
                this.sorts = tmp;
            }
            finally
            {
                // allow events and force the update of bindings
                this.RaiseListChangedEvents = oldRaise;
                this.ResetBindings();
            }
        }

        /// <summary>
        ///     Sort the list, based on a previously set of sort descriptions
        /// </summary>
        public void Sort()
        {
            if (this.sorts != null)
            {
                var items = new List<T>(this);
                items.Sort(this.sorts);

                // apply the sorting to the actual list of items
                var index = 0;
                foreach (var item in items)
                {
                    this.SetItem(index++, item);
                }
            }
        }

        /// <summary>
        ///     Sort the list on a property and a given direction
        /// </summary>
        /// <param name="propertyName">Name of the property to sort the collection on</param>
        /// <param name="direction">direction to sort</param>
        public void Sort(string propertyName, ListSortDirection direction)
        {
            var props = TypeDescriptor.GetProperties(typeof (T));
            var prop = props[propertyName];
            this.ApplySortCore(prop, direction);
        }

        /// <summary>
        ///     Sort the list on a property and a given direction
        /// </summary>
        /// <param name="propertyName1">Name of the 1st property to sort the collection on</param>
        /// <param name="direction1">1st direction to sort</param>
        /// <param name="propertyName2">Name of the 2nd property to sort the collection on</param>
        /// <param name="direction2">2nd direction to sort</param>
        public void Sort(string propertyName1, ListSortDirection direction1, string propertyName2,
            ListSortDirection direction2)
        {
            var props = TypeDescriptor.GetProperties(typeof (T));

            ListSortDescription[] sortDescriptions =
            {
                new ListSortDescription(props[propertyName1], direction1),
                new ListSortDescription(props[propertyName2], direction2)
            };
            this.ApplySort(new ListSortDescriptionCollection(sortDescriptions));
        }

        /// <summary>
        ///     Sort the list on a property and a given direction
        /// </summary>
        /// <param name="propertyName1">Name of the 1st property to sort the collection on</param>
        /// <param name="direction1">1st direction to sort</param>
        /// <param name="propertyName2">Name of the 2nd property to sort the collection on</param>
        /// <param name="direction2">2nd direction to sort</param>
        /// <param name="propertyName3">Name of the 3rd property to sort the collection on</param>
        /// <param name="direction3">3rd direction to sort</param>
        public void Sort(
            string propertyName1,
            ListSortDirection direction1,
            string propertyName2,
            ListSortDirection direction2,
            string propertyName3,
            ListSortDirection direction3)
        {
            var props = TypeDescriptor.GetProperties(typeof (T));

            ListSortDescription[] sortDescriptions =
            {
                new ListSortDescription(props[propertyName1], direction1),
                new ListSortDescription(props[propertyName2], direction2),
                new ListSortDescription(props[propertyName3], direction3)
            };
            this.ApplySort(new ListSortDescriptionCollection(sortDescriptions));
        }

        /// <summary>
        ///     Searches the list for a particular key value for a given property
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="key">Search criterion</param>
        /// <returns>
        ///     Index of the item that matches the criterion, otherwise -1
        /// </returns>
        public int Find(string propertyName, object key)
        {
            var props = TypeDescriptor.GetProperties(typeof (T));
            var prop = props[propertyName];

            return this.FindCore(prop, key);
        }

        /// <summary>
        ///     Entfernt jede mit
        ///     <see
        ///         cref="M:System.ComponentModel.BindingList`1.ApplySortCore(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)" />
        ///     angewendete Sortierung, wenn die Sortierung in einer abgeleiteten Klasse implementiert wird; andernfalls wird
        ///     <see cref="T:System.NotSupportedException" /> ausgelöst.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        ///     Die Methode wird in einer abgeleiteten Klasse nicht überschrieben.
        /// </exception>
        protected override void RemoveSortCore()
        {
            this.sorts = null;
        }

        /// <summary>
        ///     Sortiert die gegebenenfalls in einer abgeleiteten Klasse überschriebenen Elemente; andernfalls wird eine
        ///     <see cref="T:System.NotSupportedException" /> ausgelöst.
        /// </summary>
        /// <param name="prop">
        ///     Ein <see cref="T:System.ComponentModel.PropertyDescriptor" />, der die Eigenschaft angibt, nach der
        ///     sortiert werden soll.
        /// </param>
        /// <param name="direction">Einer der <see cref="T:System.ComponentModel.ListSortDirection" />-Werte.</param>
        /// <exception cref="T:System.NotSupportedException">
        ///     Die Methode wird in einer abgeleiteten Klasse nicht überschrieben.
        /// </exception>
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            ListSortDescription[] arr = {new ListSortDescription(prop, direction)};
            this.ApplySort(new ListSortDescriptionCollection(arr));
        }

        /// <summary>
        ///     Sucht nach dem Index des Elements, das über den angegebenen Eigenschaftendeskriptor mit dem angegebenen Wert
        ///     verfügt, wenn der Suchvorgang in einer abgeleiteten Klasse implementiert wird, andernfalls
        ///     <see cref="T:System.NotSupportedException" />.
        /// </summary>
        /// <param name="prop">Der zu suchende <see cref="T:System.ComponentModel.PropertyDescriptor" />.</param>
        /// <param name="key">Der Wert von <paramref name="prop" />, der übereinstimmen soll.</param>
        /// <returns>
        ///     Der nullbasierte Index des Elements, das dem Eigenschaftendeskriptor entspricht und den angegebenen Wert enthält.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        ///     <see cref="M:System.ComponentModel.BindingList`1.FindCore(System.ComponentModel.PropertyDescriptor,System.Object)" />
        ///     wird in einer abgeleiteten Klasse nicht überschrieben.
        /// </exception>
        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            for (var i = 0; i < this.Count; i++)
            {
                var val = prop.GetValue(this[i]);
                if (val != null && val.Equals(key))
                    return i;

                if (val == null && key == null)
                    return i;
            }

            return -1;
        }
    }
}