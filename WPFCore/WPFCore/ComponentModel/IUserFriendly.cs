using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.ComponentModel
{
    /// <summary>
    /// Schnittstelle für Objekte, die eine "benutzerfreundliche" Bezeichnung offenlegen.
    /// </summary>
    public interface IUserFriendly
    {
        /// <summary>
        /// Liefert die eindeutige Id des Objekts.
        /// </summary>
        int UniqueId { get; }

        /// <summary>
        /// Liefert eine benutzerfreundliche Bezeichnung.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Liefert <c>True</c> wenn das Objekt als gültiger Wert akzeptiert ist, andernfalls <c>False</c>.
        /// </summary>
        bool IsValid { get; }
    }
}
