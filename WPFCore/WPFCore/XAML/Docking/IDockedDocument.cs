using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WPFCore.XAML.Ribbon;

namespace WPFCore.XAML.Docking
{
    public interface IDockedDocument
    {
        /// <summary>
        /// Titel des Dokuments
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Symbol des Dokuments
        /// </summary>
        ImageSource Icon { get; }

        /// <summary>
        /// Definition der Kontextmenü's des Dokuments
        /// </summary>
        RibbonContextualTabGroupViewModel ContextualTabGroup { get; }

        void HandleCancellableEvent(CancelEventArgs e);

        bool CanClose { get; }

        /// <summary>
        /// Wird aufgerufen, wenn ein Dokument "aktiviert" wird (d.h. es bekommt den Fokus)
        /// </summary>
        void OnActivated();

        void OnDeactivated();
    }
}
