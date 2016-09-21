using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;
using WPFCore.ViewModelSupport;

namespace WPFCore.StatusText
{
    /// <summary>
    /// Represents a listener for messages published by the <see cref="StatusTextBroker" />. A listener registers for a specific channel
    /// of the <c>StatusTextBroker</c> and will receive all updates sent to this channel.
    /// </summary>
    [DontTraceMe]
    public class StatusTextListener : INotifyPropertyChanged
    {
        // stores the objects dispatcher
        private readonly Dispatcher myDispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Occurs when the status of the channel has been updated.
        /// </summary>
        public event StatusUpdateEventHandler StatusUpdated;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly string channel;
        private string statusText;
        private double percent;

        private readonly Dictionary<string, string> categoryStatusText = new Dictionary<string, string>();

        /// <summary>
        /// Gets a list of all categories registered for the channel along woth the last message posted to each category.
        /// </summary>
        /// <value>
        /// The category status text.
        /// </value>
        public Dictionary<string, string> CategoryStatusText { get { return this.categoryStatusText; } }

        /// <summary>
        /// The default text shown when the channel (or a category) has been cleared. May be overridden.
        /// </summary>
        protected string NoStatusText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusTextListener"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        public StatusTextListener(string channel)
        {
            this.channel = channel;
            StatusTextBroker.AddStatusListener(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="StatusTextListener"/> class.
        /// </summary>
        ~StatusTextListener()
        {
            StatusTextBroker.RemoveStatusListener(this);
        }

        /// <summary>
        /// Called by the <see cref="StatusTextBroker"/> when the status of the channel has been updated
        /// </summary>
        /// <remarks>
        /// The channel is NOT updated if the owning dispatcher is currently being shut down.
        /// </remarks>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="StatusUpdateEventArgs"/> instance containing the event data.</param>
        internal void OnStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            if (this.myDispatcher.HasShutdownStarted) return;

            //Helper.DispatcherHelper.InvokeIfRequired(this.myDispatcher, () =>
            //    {
                    switch (e.StatusUpdateType)
                    {
                        case StatusUpdateType.UpdateStatusText:
                            if (string.IsNullOrEmpty(e.Category))
                            {
                                if (e.ClearStatus)
                                    this.StatusText = this.NoStatusText;
                                else
                                    this.StatusText = e.StatusText;
                            }
                            else
                            {
                                if (e.ClearStatus)
                                    this.CategoryStatusText[e.Category] = this.NoStatusText;
                                else
                                    this.CategoryStatusText[e.Category] = e.StatusText;

                                this.OnPropertyChanged("CategoryStatusText");
                            }
                            break;
                        case StatusUpdateType.UpdateBusyIdle:
                            if (e.IsBusy != this.isBusy)
                                this.IsBusy = e.IsBusy;
                            break;
                        case StatusUpdateType.UpdatePercent:
                            this.Percent = e.Percent;
                            break;
                    }

                    // Das Ereignis auch weiterleiten (wobei der ursprüngliche sender behalten wird!)

                    Helper.DispatcherHelper.InvokeOnAppDispatcher(() =>
                        {
                            if (this.StatusUpdated != null)
                                this.StatusUpdated(sender, e);
                        });
                //});
        }

        /// <summary>
        /// Gets the channel name.
        /// </summary>
        public string Channel
        {
            get { return this.channel; }
        }

        /// <summary>
        /// Gets the current status text.
        /// </summary>
        public string StatusText
        {
            get { return this.statusText; }
            private set
            {
                this.statusText = value;
                this.OnPropertyChanged("StatusText");
            }
        }

        private bool isBusy = false;
        /// <summary>
        /// Gets a value indicating whether the sender is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the sender is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get { return this.isBusy; }
            private set
            {
                if (this.isBusy == value) return;

                this.isBusy = value;

                if (this.isBusy)
                    this.SignalBusy();
                else
                    this.SignalIdle();

                this.OnPropertyChanged("IsBusy");
            }
        }

        /// <summary>
        /// Gets the percent value used for progress updates
        /// </summary>
        /// <value>
        /// The percent value.
        /// </value>
        public double Percent
        {
            get { return this.percent; }
            private set
            {
                this.percent = value;
                this.OnPropertyChanged("Percent");
            }
        }

        #region Busy state event (handling)
        /// <summary>
        /// This delegate describes the event handler for the <see cref="BusyStateChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="bool"/> instance containing the event data.</param>
        public delegate void BusyStateChangeDelegate(object sender, bool e);

        /// <summary>
        /// Occurs when busy state of the sender changed.
        /// </summary>
        public event BusyStateChangeDelegate BusyStateChanged;

        /// <summary>
        /// Signals the busy status.
        /// </summary>
        protected void SignalBusy()
        {
            if (this.BusyStateChanged != null)
                this.BusyStateChanged(this, true);
        }

        /// <summary>
        /// Signals the idle status.
        /// </summary>
        protected void SignalIdle()
        {
            if (this.BusyStateChanged != null)
                this.BusyStateChanged(this, false);
        }
        #endregion

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
