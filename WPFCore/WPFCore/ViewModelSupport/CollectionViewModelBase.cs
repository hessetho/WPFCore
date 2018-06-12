using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace WPFCore.ViewModelSupport
{
    /// <summary>
    ///     Represents a dynamic data collection that provides notifications when items are added/removed and
    ///     which can be sorted and filtered.
    /// </summary>
    /// <remarks>
    ///     Instances of this class should not be bound directly to item containers (such as listboxes, comboboxes,
    ///     data grids etc.). Although the property <see cref="Rows" /> is intended as binding source for such
    ///     controls.
    ///     Internally this class uses an <see cref="ObservableCollection{T}" /> to store the items.
    ///     A <see cref="CollectionViewSource" /> is used to provide sorting and filtering facilities, the property
    ///     <see cref="Rows" /> returns its data elements.
    ///     Direct write access to the source collection is prohibited; the methods <see cref="Add" />,
    ///     <see cref="Remove" /> and <see cref="Clear" /> must be used instead. These take care that the view on
    ///     the data elements is refreshed.
    ///     If required/desired the source collection can be accessed using the <see cref="Source" /> property, which
    ///     is of type <see cref="ReadOnlyObservableCollection&lt;T&gt;" />.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    //[DebuggerStepThrough]
    public abstract class CollectionViewModelBase<T> : ViewModelBase, INotifyCollectionChanged where T : ViewModelBase
    {
        /// <summary>
        ///     Internal view on the collection
        ///     (see the <see cref="Rows" /> property)
        /// </summary>
        private CollectionViewSource collectionViewSource;

        /// <summary>
        ///     Provides a readonly interface to the collection's data elements
        ///     (see the <see cref="Source" /> property)
        /// </summary>
        private ReadOnlyObservableCollection<T> nativeRows;

        /// <summary>
        ///     Internal storage of the collection's data elements
        /// </summary>
        private ObservableCollection<T> observableCollection;

        /// <summary>
        ///     Controls whether changing the current item triggers the <see cref="CurrentChanging" /> event.
        ///     Used internally only.
        /// </summary>
        private bool ignoreCurrentChange;
        private bool isValid;

        /// <summary>
        /// Is raised when a refresh of the collection's presentation is suggested (i.e. due to change of filter relevant data)
        /// </summary>
        public event EventHandler SuggestRefresh;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <remarks>
        ///     The constructor asserts that the object is instantiated on the application's main thread.
        /// </remarks>
        protected CollectionViewModelBase()
        {
//#if DEBUG
//            if (Application.Current.Dispatcher != MyDispatcher)
//                this.MyTraceSource.TraceError(string.Format("{0}: Instances of CollectionViewSource should always be created on the main thread, use InvokeOnAppDispatcher().", this.GetType().Name));
//#endif

            InvokeOnAppDispatcher(() =>
                {
                    this.observableCollection = new ObservableCollection<T>();
                    this.observableCollection.CollectionChanged += this.OnCollectionChanged;

                    this.nativeRows = new ReadOnlyObservableCollection<T>(this.observableCollection);

                    this.collectionViewSource = new CollectionViewSource();
                    this.collectionViewSource.Source = this.observableCollection;
                    this.collectionViewSource.View.CurrentChanged += this.View_CurrentChanged;
                    this.collectionViewSource.View.CurrentChanging += this.View_CurrentChanging;
                });

            this.Initialized += (ds, de) =>
            {
                this.OnPropertyChanged("TotalRowsCount");
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("HasRows");
            };
        }

        /// <summary>
        ///     Returns the rows of the collection. These may be sorted and/or filtered
        /// </summary>
        public ICollectionView Rows
        {
            get 
            {
                return this.collectionViewSource.View;
            }
        }

        /// <summary>
        ///     (OBSOLETE) Returns the collection from which this CollectionViewModel is created from.
        /// </summary>
        [Obsolete("Please use the Source property instead.")]
        public ReadOnlyObservableCollection<T> NativeRows
        {
            get { return this.nativeRows; }
        }

        /// <summary>
        ///     Returns the collection from which this CollectionViewModel is created from.
        /// </summary>
        public ReadOnlyObservableCollection<T> Source
        {
            get { return this.nativeRows; }
        }

        /// <summary>
        /// (Obsolete) Returns the total number of rows in the base collection.
        /// </summary>
        [Obsolete("Use the property Count instead.")]
        [DoesNotAffectChangesFlag]
        public int TotalRowsCount
        {
            get { return this.observableCollection.Count; }
        }

        /// <summary>
        ///     returns the total number of rows of the underlying collection
        /// </summary>
        [DoesNotAffectChangesFlag]
        public int Count
        {
            get { return this.observableCollection.Count; }
        }

        /// <summary>
        ///     Gets or sets the currently selected row
        /// </summary>
        [DoesNotAffectChangesFlag]
        public T SelectedRow
        {
            get { return (T) this.collectionViewSource.View.CurrentItem; }
            set 
            {
                if (value!=null && !this.Source.Contains(value))
                    throw new InvalidOperationException("The requested item does not exist in the Collection.");

                if(this.collectionViewSource.View.CurrentItem != value)
                    this.collectionViewSource.View.MoveCurrentTo(value); 
            }
        }

        /// <summary>
        ///     Returns <c>True</c> if a row is currently selected, otherwise <c>False</c>
        /// </summary>
        [DoesNotAffectChangesFlag]
        public bool HasSelection
        {
            get
            {
                return this.collectionViewSource.View.CurrentItem != null;
                //return this.selectedRow != null; 
            }
        }

        ///// <summary>
        ///// Returns <c>True</c> if the collection is fully initialized, otherwise <c>False</c>
        ///// </summary>
        //[DoesNotAffectChangesFlag]
        //public new bool IsInitialized
        //{
        //    // note: we have to use "new"!
        //    get { return isInitialized; }
        //    private set
        //    {
        //        isInitialized = value;

        //        if (isInitialized)
        //        {
        //            OnPropertyChanged("TotalRowsCount");
        //            OnPropertyChanged("Count");
        //            OnPropertyChanged("HasRows");
        //            OnInitialized();
        //        }

        //        OnPropertyChanged("IsInitialized");
        //    }
        //}

        /// <summary>
        ///     Returns <c>True</c> if the collection contains at least one row, otherwise <c>False</c>
        /// </summary>
        [DoesNotAffectChangesFlag]
        public bool HasRows
        {
            get { return this.observableCollection.Count > 0; }
        }

        /// <summary>
        ///     Returns the sort descriptions
        /// </summary>
        public SortDescriptionCollection SortDescriptions
        {
            get { return this.collectionViewSource.SortDescriptions; }
        }

        /// <summary>
        ///     Select the first row of the collection view
        /// </summary>
        public void SelectFirstRow()
        {
            InvokeOnAppDispatcher(() => { this.Rows.MoveCurrentToFirst(); });
        }

        /// <summary>
        ///     Returns <c>True</c> if a row is currently selected, otherwise <c>False</c>
        /// </summary>
        /// <returns></returns>
        protected bool IsRowSelected()
        {
            return this.collectionViewSource.View.CurrentItem != null;
        }

        /// <summary>
        /// Occurs when an item of the has been changed.
        /// </summary>
        public event EventHandler<T> ItemChanged;

        /// <summary>
        /// This event is raised before the current item changes. Event handler can cancel this event.
        /// </summary>
        public event CurrentChangingEventHandler CurrentChanging;

        /// <summary>
        /// This event is raised after the current item has changed.
        /// </summary>
        public event EventHandler<T> CurrentChanged;

        /// <summary>
        ///     Provides filtering logic
        /// </summary>
        public event FilterEventHandler Filter
        {
            add { InvokeOnAppDispatcher(() => { this.collectionViewSource.Filter += value; }); }
            remove { InvokeOnAppDispatcher(() => { this.collectionViewSource.Filter -= value; }); }
        }

        /// <summary>
        ///     Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        ///     Add's an item to the collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            ((ViewModelBase)item).HasChanged += this.CollectionItemHasChanged;

            InvokeOnAppDispatcher(() =>
            {
                //this.FireCollectionChanged(NotifyCollectionChangedAction.Add, item);
                this.observableCollection.Add(item);

                if(this.IsInitialized)
                    this.Refresh();
            });
        }

        protected void ThreadSecureAdd(T item)
        {
            ((ViewModelBase)item).HasChanged += this.CollectionItemHasChanged;

            //this.FireCollectionChanged(NotifyCollectionChangedAction.Add, item);
            this.observableCollection.Add(item);

            if (this.IsInitialized)
                this.Refresh();
        }

        /// <summary>
        /// Handles the <see cref="ViewModelBase.HasChanged"/> event of the collections items.
        /// It raises the <see cref="ItemChanged"/> event with the changed item as event data.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="string"/> instance containing the event data.</param>
        private void CollectionItemHasChanged(object sender, string e)
        {
            this.ItemChanged?.Invoke(this, (T)sender);

            this.SetChangeFlag();
        }

        /// <summary>
        ///     Removes an item from the collection
        /// </summary>
        /// <remarks>
        ///     The removal of the item will not trigger the <see cref="CurrentChanging" /> event.
        /// </remarks>
        /// <param name="item">The item to remove</param>
        public void Remove(T item)
        {
            InvokeOnAppDispatcher(() =>
            {
                this.ignoreCurrentChange = true;
                    // suppress the handling of the CurrentChanging event of the ICollectionView

                //this.FireCollectionChanged(NotifyCollectionChangedAction.Remove, item);
                this.observableCollection.Remove(item);
                this.Refresh();

                this.ignoreCurrentChange = false;
            });
        }

        /// <summary>
        ///     Removes all items from the collection
        /// </summary>
        public void Clear()
        {
            InvokeOnAppDispatcher(() =>
            {
                // suppress the handling of the CurrentChanging event of the ICollectionView
                this.ignoreCurrentChange = true;

                this.observableCollection.Clear();
                this.collectionViewSource.View.Refresh();

                this.ignoreCurrentChange = false;
            });
        }

        /// <summary>
        ///     Force a refresh of the view
        /// </summary>
        public void Refresh()
        {
            InvokeOnAppDispatcher(() =>
            {
                this.collectionViewSource.View.Refresh();
                if (this.SuggestRefresh != null)
                    this.SuggestRefresh(this, new EventArgs());
            });
        }

        /// <summary>
        ///     Remove all sort descriptions
        /// </summary>
        protected void ClearSort()
        {
            InvokeOnAppDispatcher(this.collectionViewSource.SortDescriptions.Clear);
        }

        /// <summary>
        ///     Add a sort description
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="direction"></param>
        protected void AddSort(string columnName, ListSortDirection direction)
        {
            InvokeOnAppDispatcher(() => this.collectionViewSource.SortDescriptions.Add(new SortDescription(columnName, direction)));
        }

        protected void AddGroup(string columnName)
        {
            InvokeOnAppDispatcher(() => this.collectionViewSource.GroupDescriptions.Add(new PropertyGroupDescription(columnName)));
        }

        /// <summary>
        /// Validates this instance by validating each contained item.
        /// </summary>
        public void Validate()
        {
            if (typeof(T).IsSubclassOf(typeof(ValidationViewModelBase)))
            {
                foreach (var item in this.Source.Cast<ValidationViewModelBase>())
                    item.Validate();

                this.IsValid = !this.Source.Cast<ValidationViewModelBase>().Any(itm => itm.IsValid == false);
            }
        }

        /// <summary>
        /// Indicates, whether this instance has validated content.
        /// </summary>
        /// <value>
        /// It is always <c>true</c>, if this is not a validating collection.
        /// </value>
        [DoesNotAffectChangesFlag]
        public bool IsValid
        {
            get { return this.isValid; }
            private set
            {
                if (value == this.isValid) return;

                this.isValid = value;
                base.OnPropertyChanged("IsValid");
            }
        }

        /// <summary>
        ///     This event handler is triggered before the currently selected row is changed.
        ///     It raises the <see cref="CurrentChanging" /> event, which allows the application
        ///     to cancel the row change action.
        /// </summary>
        /// <remarks>
        ///     When the currently selected data element (row) is deleted, the selected row is moved to
        ///     the next available data element, thus triggering this method. In this case, the application
        ///     should not be allowed to cancel this row change action. This is controlled through the
        ///     field <see cref="ignoreCurrentChange" />, which is used by the method <see cref="Remove" />.
        /// </remarks>
        /// <param name="sender">The event triggering object</param>
        /// <param name="e">Event data</param>
        private void View_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            if (e.IsCancelable == false || this.CurrentChanging == null || this.ignoreCurrentChange) return;

            // call the change event handler
            this.CurrentChanging(sender, e);
        }

        /// <summary>
        ///     This event handler is called after the current item pointer was moved to a different item.
        ///     It signal's the changes to the properties <see cref="SelectedRow" /> and <see cref="HasSelection" />
        /// </summary>
        /// <param name="sender">The event triggering object</param>
        /// <param name="e">Event data</param>
        private void View_CurrentChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("SelectedRow");
            this.OnPropertyChanged("HasSelection");

            this.CurrentChanged?.Invoke(this, this.SelectedRow);
        }

        /// <summary>
        ///     Propagates the CollectionChanged event of the underling collection and signals
        ///     changes to the properties <see cref="Count" /> and <see cref="HasRows" />.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("TotalRowsCount");
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("HasRows");

            // let the event bubble
            this.CollectionChanged?.Invoke(this, e);
        }

        #region EXPERIMENTAL ... defer refresh
        /* 

        private IDisposable deferHook;
        public new void BeginInit()
        {
            base.BeginInit();

            if (this.deferHook == null)
                this.InvokeIfRequired(() =>
                    {
                        this.deferHook = this.collectionViewSource.DeferRefresh();
                    });
        }

        public new void EndInit()
        {
            base.EndInit();

            if (this.deferHook != null && this.IsInitialized)
            {
                this.InvokeIfRequired(() =>
                    {
                        // this'll trigger a refresh and thus need to be done in the owner's dispatcher context
                        this.deferHook.Dispose();
                        this.deferHook = null;
                    });
            }
        }
        */
        #endregion EXPERIMENTAL ...

        #region INotifyCollectionChanged

        /// <summary>
        ///     Triggers the <see cref="CollectionChanged" /> event.
        /// </summary>
        /// <param name="action">The changing action</param>
        /// <param name="item">The affected item</param>
        protected void FireCollectionChanged(NotifyCollectionChangedAction action, object item)
        {
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item));
        }

        /// <summary>
        /// Triggers the <see cref="CollectionChanged" /> event for a collection reset.
        /// </summary>
        protected void FireCollectionReset()
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion INotifyCollectionChanged

        #region Implementation of ICollectionViewModel

        public IEnumerable<object> AsIEnumerable()
        {
            return this.Source.Cast<object>();
        }

        #endregion
    }
}