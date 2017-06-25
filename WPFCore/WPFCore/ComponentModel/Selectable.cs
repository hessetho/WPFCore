using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFCore.ViewModelSupport;

namespace WPFCore.ComponentModel
{
    /// <summary>
    /// Repreäsentiert ein auswählbares Element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Selectable<T> : ViewModelCore, ISelectable where T : IUserFriendly
    {
        private T item;
        private bool isSelected;

        public Selectable(T item)
        {
            this.item = item;
        }

        /// <summary>
        /// Liefert die anzuzeigende Bezeichnung der Instanz.
        /// </summary>
        public string DisplayName
        {
            get { return this.item.DisplayName; }
        }

        /// <summary>
        /// Liefert bzw. setzt den Auswahlzustand der Instanz.
        /// </summary>
        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                if (this.isSelected == value) return;

                this.isSelected = value;
                base.OnPropertyChanged("IsSelected");

                this.SelectionStateChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Liefert das Element dieser Instanz.
        /// </summary>
        public T Item
        {
            get { return this.item; }
        }

        /// <summary>
        /// Liefert das Element dieser Instanz.
        /// </summary>
        public object SelectedItem
        {
            get { return this.item; }
        }

        /// <summary>
        /// Ereignis tritt ein, wenn sich der Auswhalzustand der Instanz ändert.
        /// </summary>
        public event EventHandler SelectionStateChanged;
    }
}
