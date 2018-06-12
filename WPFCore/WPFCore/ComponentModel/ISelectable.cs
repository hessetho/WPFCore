using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.ComponentModel
{
    /// <summary>
    /// Schnittstelle für auswählbare Objekte.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Liefert bzw. setzt den Auswahlzustand der Instanz.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Ereignis tritt ein, wenn sich der Auswhalzustand der Instanz ändert.
        /// </summary>
        event EventHandler SelectionStateChanged;

        /// <summary>
        /// Liefert diese Instanz. Bei generischen Objekten, die <c>ISelectable</c> implementieren wird das eigentliche Objekt geliefert.
        /// </summary>
        object SelectedItem { get; }
    }
}
