// ==========================================================================
// <summary>
// WpfHelper: Sammlung von Hilfsfunktionen und Klassen zur Verwendung mit WPF
// </summary>
//
// $Rev: 91 $
// $Id: PropertyComparer.cs 91 2010-10-12 12:43:47Z  $
// 
// <copyright file="PropertyComparer.cs" company="ICEP GmbH">
//      2009-2012 ICEP GmbH, T. Hesse
// </copyright>
// ==========================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace WPFCore.Data
{
    /// <summary>
    /// Diese Klasse unterstützt den Vergleich zweier Objekte.
    /// </summary>
    /// <remarks>
    /// Dies ist eine Unterstützungsklasse für die <see cref="SortableList&lt;T&gt;"/>-Klasse.
    /// </remarks>
    /// <typeparam name="T">Typ der zu vergleichenden Objekte.</typeparam>
    internal class PropertyComparer<T> : IComparer<T>
    {
        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="PropertyComparer&lt;T&gt;"/>-Klasse.
        /// </summary>
        /// <param name="property">The property used for the comparison</param>
        /// <param name="descending">The sort direction (simply inverts the comparison result if descending)</param>
        public PropertyComparer(PropertyDescriptor property, bool descending)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            this.Descending = descending;
            this.Property = property;
        }

        /// <summary>
        /// Gets the sort direction (true if descending)
        /// </summary>
        public bool Descending { get; private set; }

        /// <summary>
        /// returns the property used for the comparison
        /// </summary>
        public PropertyDescriptor Property { get; private set; }

        /// <summary>
        /// Compares two objects based on the value of the comparison property
        /// (-1: x &lt; y, 0: x = y, +1: x &gt; y, results inverted for descending sorting)
        /// </summary>
        /// <param name="x">first object</param>
        /// <param name="y">second object</param>
        /// <returns>-1: x &lt; y, 0: x = y, +1: x &gt; y, results inverted for descending sorting</returns>
        public int Compare(T x, T y)
        {
            // ToDo: catch some null cases 
            int value = Comparer.Default.Compare(this.Property.GetValue(x), this.Property.GetValue(y));
            return this.Descending ? -value : value;
        }
    }
}
