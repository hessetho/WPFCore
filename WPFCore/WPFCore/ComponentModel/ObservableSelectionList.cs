using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCore.ComponentModel
{
    public class ObservableSelectionList<T> : ObservableCollection<Selectable<T>>
        where T : IUserFriendly
    {
        /// <summary>
        /// Is raised if any item's selection status changes
        /// </summary>
        public event EventHandler SelectionChanged;

        public ObservableSelectionList()
        {
        }

        public ObservableSelectionList(IEnumerable<T> items)
        {
            foreach (var item in items)
                this.Add(item);
        }

        public bool HasSelectedItems
        {
            get { return this.Any(i => i.IsSelected); }
        }

        public bool AreAllSelected()
        {
            return this.All(i => i.IsSelected);
        }

        public bool IsNoneSelected()
        {
            return !this.Any(i => i.IsSelected);
        }

        public new void Add(Selectable<T> item)
        {
            item.SelectionStateChanged += Item_SelectionStateChanged;
            base.Add(item);
        }

        public void Add(T item)
        {
            this.Add(new Selectable<T>(item));
        }

        public void AddRange(IEnumerable<Selectable<T>> range)
        {
            foreach (var item in range)
                this.Add(item);
        }

        public void Remove(T item)
        {
            var selectable = Find(item);
            base.Remove(selectable);
        }

        public new void Remove(Selectable<T> item)
        {
            item.SelectionStateChanged -= Item_SelectionStateChanged;
            base.Remove(item);
        }

        public void SelectAll()
        {
            foreach (var selectable in this)
                selectable.IsSelected = true;
        }

        public void DeselectAll()
        {
            foreach (var selectable in this)
                selectable.IsSelected = false;
        }

        private Selectable<T> Find(T item)
        {
            foreach (var selectable in this)
                if (selectable.Item.Equals(item))
                    return selectable;

            return null;
        }

        /// <summary>
        /// Liefert die ausgewählten Elemente.
        /// </summary>
        /// <returns></returns>
        public List<T> GetSelectedItems()
        {
            return this.Where(itm => itm.IsSelected).Select(itm => itm.Item).ToList();
        }

        private bool isInitializing;

        public void InitializeSelection(IEnumerable<T> selectedItems)
        {
            this.isInitializing = true;

            // alle Auswahlen erstmal entfernen
            foreach(var item in this)
                item.IsSelected = false;

            // und nun die übergebenen Elemente auswählen
            foreach (var item in this.Where(itm => selectedItems.Contains(itm.Item)))
                item.IsSelected = true;

            this.isInitializing = false;
        }

        private void Item_SelectionStateChanged(object sender, EventArgs e)
        {
            if (this.isInitializing) return;

            this.OnPropertyChanged("HasSelectedItems");
            this.SelectionChanged?.Invoke(this, new EventArgs());
        }

        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
