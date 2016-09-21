// ==========================================================================
// <summary>
// WpfHelper: Sammlung von Hilfsfunktionen und Klassen zur Verwendung mit WPF
// </summary>
//
// $Rev: 91 $
// $Id: PropertyComparerCollection.cs 91 2010-10-12 12:43:47Z  $
// 
// <copyright file="PropertyComparerCollection.cs" company="ICEP GmbH">
//      2009-2012 ICEP GmbH, T. Hesse
// </copyright>
// ==========================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WPFCore.Data
{
    /// <summary>
    /// Supports the comparison of two objects by a set of properties which is given as a 
    /// ListSortDescriptionCollection. This is used in conjunction with sortable lists.
    /// </summary>
    /// <remarks>
    /// Dies ist eine Unterstützungsklasse für die <see cref="SortableList&lt;T&gt;"/>-Klasse.
    /// </remarks>
    /// <typeparam name="T">Typ der Elemente, die zu sortieren sind.</typeparam>
    internal class PropertyComparerCollection<T> : IComparer<T>
    {
        /// <summary>
        /// Stellt die Eigenschaften dar, über welche sortiert werden soll
        /// </summary>
        private readonly ListSortDescriptionCollection sorts;

        /// <summary>
        /// Liste der <see cref="PropertyComparer&lt;T&gt;"/> für die zu sortierenden Eigenschaften.
        /// </summary>
        private readonly PropertyComparer<T>[] comparers;

        /// <summary>
        /// Constructor. Take a list of ListSortDescriptions
        /// </summary>
        /// <param name="sorts">List of ListSortDescriptions</param>
        public PropertyComparerCollection(ListSortDescriptionCollection sorts)
        {
            if (sorts == null)
            {
                throw new ArgumentNullException("sorts");
            }

            this.sorts = sorts;

            // create a list of comparer delegates for the ListSortDescriptions
            List<PropertyComparer<T>> list = new List<PropertyComparer<T>>();
            foreach (ListSortDescription item in sorts)
            {
                list.Add(
                    new PropertyComparer<T>(
                        item.PropertyDescriptor,
                        item.SortDirection == ListSortDirection.Descending));
            }

            this.comparers = list.ToArray();
        }

        /// <summary>
        /// Returns a list of ListSortDescriptions
        /// </summary>
        public ListSortDescriptionCollection Sorts
        {
            get { return this.sorts; }
        }

        /// <summary>
        /// Liefert die erste Eigenschaft, über die sortiert werden soll.
        /// </summary>
        public PropertyDescriptor PrimaryProperty
        {
            get
            {
                return this.comparers.Length == 0 ? null : this.comparers[0].Property;
            }
        }

        /// <summary>
        /// Liefert die Sortierrichtung für die erste Eigenschaft, über die sortiert werden soll.
        /// Default ist Ascending
        /// </summary>
        public ListSortDirection PrimaryDirection
        {
            get
            {
                return this.comparers.Length == 0 ? ListSortDirection.Ascending
                    : this.comparers[0].Descending ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
        }

        /// <summary>
        /// Vergleicht zwei Objekte anhand aller Sortier-Eigenschaften.
        /// </summary>
        /// <param name="lhs">Objekt auf der linken Seite</param>
        /// <param name="rhs">Objekt auf der rechten Seite</param>
        /// <returns><c>-1</c> wenn die linke Seite kleiner als die rechte Seite ist, <c>0</c> wenn beide Seiten gleich sind, andernfalls <c>1</c>.</returns>
        int IComparer<T>.Compare(T lhs, T rhs)
        {
            int result = 0;

            // iterate through all comparer and break out on the first inequality
            for (int i = 0; i < this.comparers.Length; i++)
            {
                result = this.comparers[i].Compare(lhs, rhs);
                if (result != 0)
                {
                    break;
                }
            }

            return result;
        }
    }
}
