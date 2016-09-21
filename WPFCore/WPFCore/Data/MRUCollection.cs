// ==========================================================================
// <summary>
// WpfHelper: Sammlung von Hilfsfunktionen und Klassen zur Verwendung mit WPF
// </summary>
//
// $Rev: 91 $
// $Id: MRUCollection.cs 91 2010-10-12 12:43:47Z  $
// 
// <copyright file="MRUCollection.cs" company="ICEP GmbH">
//      2009-2012 ICEP GmbH, T. Hesse
// </copyright>
// ==========================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using Microsoft.Win32;
using WPFCore.Helper;

namespace WPFCore.Data
{
    /// <summary>
    ///     Definiert eine Liste "kürzlich verwendeter" Objekte (most recently used = MRU).
    ///     Die Liste wird in der Registry unter einem eigenen Key gespeichert.
    /// </summary>
    public class MRUCollection : IEnumerable, IEnumerator, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        ///     Die Liste der MRU-Elemente
        /// </summary>
        private readonly LinkedList<string> items = new LinkedList<string>();

        /// <summary>
        ///     Feld zur Speicherung des Namens der MRU-Liste. Entspricht ihrem Schlüssel in der Registry
        /// </summary>
        private readonly string keyName = string.Empty;

        /// <summary>
        ///     Feld zur Speicherung der maximal in der MRU-Liste zu haltender Elemente
        /// </summary>
        private readonly int maxNumber = 4;

        /// <summary>
        ///     Speichert den Dispatcher, dem die MRUCollection "gehört"
        /// </summary>
        private readonly Dispatcher myDispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        ///     Feld zur Speicherung des aktuellen Knoten für die Enumeration
        /// </summary>
        private LinkedListNode<string> current;

        /// <summary>
        ///     Legt eine MRU-Liste an
        /// </summary>
        /// <param name="keyName">Name des Registry-Keys für die MRU-Liste</param>
        public MRUCollection(string keyName)
        {
            this.keyName = keyName;
            this.LoadEntries();
        }

        /// <summary>
        ///     Legt eine MRU-Liste an und legt die maximale Anzahl von Einträgen fest.
        /// </summary>
        /// <param name="keyName">Name des Registry-Keys für die MRU-Liste</param>
        /// <param name="maxNumberOfEntries">Maximale Anzahl Einträge</param>
        public MRUCollection(string keyName, int maxNumberOfEntries)
        {
            this.maxNumber = maxNumberOfEntries;
            this.keyName = keyName;
            this.LoadEntries();
        }

        /// <summary>
        ///     Liefert die Anzahl in der Liste enthaltener Elemente.
        /// </summary>
        /// <value>Die Anzahl.</value>
        public int Count
        {
            get { return this.items.Count; }
        }

        //public string this[int index]
        //{
        //    get { return base[index]; }
        //    set { base[index] = value; }
        //}

        /// <summary>
        ///     Liefert den <see cref="System.String" /> an einem angegebenen Index in der MRU-Liste
        /// </summary>
        /// <param name="index">Die gewünschte POsition in der Liste.</param>
        /// <value>Der text an der geforderten Position.</value>
        public string this[int index]
        {
            get
            {
                int cnt = 0;

                LinkedListNode<string> node = this.items.First;
                while (node != null && cnt != index)
                {
                    cnt++;
                    node = node.Next;
                }

                if (node != null)
                {
                    return node.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        #region IEnumerable Members

        /// <summary>
        ///     Gibt einen Enumerator zurück, der eine Auflistung durchläuft.
        /// </summary>
        /// <returns>
        ///     Ein <see cref="T:System.Collections.IEnumerator" />-Objekt, das zum Durchlaufen der Auflistung verwendet werden kann.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            this.Reset();
            return this;
        }

        #endregion

        #region IEnumerator Members

        /// <summary>
        ///     Ruft das aktuelle Element in der Auflistung ab.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     Das aktuelle Element in der Auflistung.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        ///     Der Enumerator ist vor dem ersten Element oder hinter dem letzten Element der Auflistung positioniert.
        ///     – oder –
        ///     Die Auflistung wurde nach dem Erstellen des Enumerators geändert.
        /// </exception>
        public object Current
        {
            get { return this.current.Value; }
        }

        /// <summary>
        ///     Setzt den Enumerator auf das nächste Element der Auflistung.
        /// </summary>
        /// <returns>
        ///     true, wenn der Enumerator erfolgreich auf das nächste Element gesetzt wurde, false, wenn der Enumerator das Ende der Auflistung überschritten hat.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        ///     Die Auflistung wurde nach dem Erstellen des Enumerators geändert.
        /// </exception>
        public bool MoveNext()
        {
            if (this.current == null)
            {
                this.current = this.items.First;
            }
            else
            {
                this.current = this.current.Next;
            }

            return this.current != null;
        }

        /// <summary>
        ///     Setzt den Enumerator auf seine anfängliche Position vor dem ersten Element in der Auflistung.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        ///     Die Auflistung wurde nach dem Erstellen des Enumerators geändert.
        /// </exception>
        public void Reset()
        {
            this.current = null;
        }

        #endregion

        #region INotifyCollectionChanged Members

        /// <summary>
        ///     Tritt ein, wenn die Auflistung geändert wird.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        ///     Tritt ein, wenn sich ein Eigenschaftenwert ändert.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        ///     Fügt einen Eintrag zur MRU-Liste hinzu. Der Eintrag wird auf Position 1 gesetzt,
        ///     auch wenn er vorher bereits in der Liste enthalten war.
        /// </summary>
        /// <param name="itemText">Der zu ergänzende Text.</param>
        public void Add(string itemText)
        {
            string mruentry = this.Find(itemText);

            // wenn der Text bereits bekannt ist, wird er entfernt
            if (mruentry != null)
            {
                this.Remove(mruentry);
            }
            else
            {
                mruentry = itemText;
            }

            // Liste auf die maximale Anzahl Elemente trimmen, wo noch ein Element hinzugefügt werden kann
            while (this.items.Count >= this.maxNumber)
            {
                this.Remove(this.items.Last());
            }

            // Eintrag an den Anfang der Liste stellen
            this.items.AddFirst(mruentry);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");

            // und die geänderte Liste sofort speichern
            this.SaveEntries();

            // Ereignis auslösen
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, mruentry, 0);
        }

        /// <summary>
        ///     Entfernt einen Eintrag aus der MRU-Liste
        /// </summary>
        /// <param name="itemText">Der zu entfernende Eintrag.</param>
        public void Remove(string itemText)
        {
            string mruentry = this.Find(itemText);

            // wenn der Text vorhanden ist, kann er entfernt werden
            if (mruentry != null)
            {
                int index = this.IndexOf(mruentry);

                // Ereignis auslösen
                this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, mruentry, index);

                // Aus der Liste entfernen
                this.items.Remove(mruentry);
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");

                // und die geänderte Liste sofort speichern
                this.SaveEntries();
            }
        }

        /// <summary>
        ///     Sucht einen Text in der MRU-Liste
        /// </summary>
        /// <param name="itemText">Der gesuchte Text.</param>
        /// <returns>
        ///     Der gesuchten Text, oder <c>null</c> wenn nicht vorhanden
        /// </returns>
        public string Find(string itemText)
        {
            IEnumerable<string> result = this.items.Where(mru => mru == itemText);

            if (result.Count() == 0)
                return null;

            if (result.Count() == 1)
                return result.ElementAt(0);

            throw new ApplicationException("Doppelter Text in MRUCollection!");
        }

        /// <summary>
        ///     Liefert die Position eines Texts in der MRU-Liste
        /// </summary>
        /// <param name="itemText">Der gesuchte Text.</param>
        /// <returns>Position des gesuchten Texts.</returns>
        public int IndexOf(string itemText)
        {
            int cnt = 0;

            LinkedListNode<string> node = this.items.First;
            while (node != null && node.Value != itemText)
            {
                cnt++;
                node = node.Next;
            }

            if (node != null)
                return cnt;

            return -1;
        }

        /// <summary>
        ///     Schreibt die MRU-Liste in die Registry
        /// </summary>
        private void SaveEntries()
        {
            RegistryKey key;
            key = Registry.CurrentUser.CreateSubKey(AppContext.RegistryPath + this.keyName);

            // Zuerst alle Einträge löschen
            string[] mru = key.GetValueNames();
            for (int i = 0; i < mru.GetLength(0); i++)
            {
                key.DeleteValue(mru[i]);
            }

            // Nun die aktuellen Einträge sichern
            int rank = 1;
            foreach (string mrufile in this.items)
            {
                string name = string.Format("MRU{0}", rank++);

                key.SetValue(name, mrufile);

                if (rank > this.maxNumber)
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Liest die MRU-Liste aus der Registry
        /// </summary>
        private void LoadEntries()
        {
            RegistryKey key;
            key = Registry.CurrentUser.CreateSubKey(AppContext.RegistryPath + this.keyName);

            string[] mru = key.GetValueNames();

            for (int i = 0; i < mru.GetLength(0); i++)
            {
                var filename = (string) key.GetValue(mru[i]);

                this.items.AddLast(filename);
            }

            // Ereignis auslösen
            this.OnCollectionChanged(NotifyCollectionChangedAction.Reset, null, -1);
        }

        /// <summary>
        ///     Wird aufgerufen, wenn sich die Liste ändert.
        /// </summary>
        /// <param name="action">Art der Veränderung.</param>
        /// <param name="changedItem">Das veränderte Element.</param>
        /// <param name="index">Position des veränderten Elements.</param>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            if (this.CollectionChanged != null)
            {
                DispatcherHelper.InvokeIfRequired(this.myDispatcher,
                                                  () =>
                                                  this.CollectionChanged(this,
                                                                         new NotifyCollectionChangedEventArgs(action,
                                                                                                              changedItem,
                                                                                                              index)));
            }
        }

        /// <summary>
        ///     Wird aufgerufen, wenn sich ein Eigenschaftswert ändert.
        /// </summary>
        /// <param name="propertyName">Name der Eigenschaft.</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                DispatcherHelper.InvokeIfRequired(this.myDispatcher,
                                                  () =>
                                                  this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
            }
        }
    }
}