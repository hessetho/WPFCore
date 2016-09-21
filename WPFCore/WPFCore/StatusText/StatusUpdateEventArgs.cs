namespace WPFCore.StatusText
{
    public enum StatusUpdateType
    {
        UpdateStatusText,
        UpdateBusyIdle,
        UpdatePercent,
        UpdateData
    }

    /// <summary>
    /// Define the event arguments sent by the <see cref="StatusTextBroker"/> when updating a channel.
    /// </summary>
    public class StatusUpdateEventArgs
    {
        private StatusUpdateEventArgs(StatusUpdateType statusUpdateType, string statusText)
        {
            this.StatusUpdateType = statusUpdateType;
            this.StatusText = statusText;
            this.ClearStatus = false;
            this.IsBusy = false;
            this.IsIdle = true;
            this.Category = string.Empty;
            this.Data = statusText;
        }

        private StatusUpdateEventArgs(StatusUpdateType statusUpdateType, string category, string statusText)
        {
            this.StatusUpdateType = statusUpdateType;
            this.StatusText = statusText;
            this.ClearStatus = false;
            this.IsBusy = false;
            this.IsIdle = true;
            this.Data = statusText;

            this.Category = category;
        }

        private StatusUpdateEventArgs(StatusUpdateType statusUpdateType)
        {
            this.StatusUpdateType = statusUpdateType;
            this.StatusText = string.Empty;
            this.ClearStatus = true;
            this.IsBusy = false;
            this.IsIdle = true;
            this.Data = null;
        }

        private StatusUpdateEventArgs(StatusUpdateType statusUpdateType, double percent)
        {
            this.StatusUpdateType = statusUpdateType;
            this.StatusText = string.Empty;
            this.ClearStatus = true;
            this.IsBusy = false;
            this.IsIdle = true;
            this.Percent = percent;
            this.Data = percent;
        }

        private StatusUpdateEventArgs(StatusUpdateType statusUpdateType, object data)
        {
            this.StatusUpdateType = statusUpdateType;
            this.StatusText = string.Empty;
            this.ClearStatus = false;
            this.IsBusy = false;
            this.IsIdle = true;
            this.Category = string.Empty;
            this.Data = data;
        }

        private StatusUpdateEventArgs(StatusUpdateType statusUpdateType, string category, object data)
        {
            this.StatusUpdateType = statusUpdateType;
            this.StatusText = string.Empty;
            this.ClearStatus = false;
            this.IsBusy = false;
            this.IsIdle = true;
            this.Category = category;
            this.Data = data;
        }

        /// <summary>
        /// Gets the type of the status update.
        /// </summary>
        /// <value>
        /// The type of the status update.
        /// </value>
        public StatusUpdateType StatusUpdateType { get; private set; }

        /// <summary>
        /// Gets the status text.
        /// </summary>
        /// <value>
        /// The status text.
        /// </value>
        public string StatusText { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [clear status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if the status has been cleared; otherwise, <c>false</c>.
        /// </value>
        public bool ClearStatus { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the sender is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the sender is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the sender is idle.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the sender is idle; otherwise, <c>false</c>.
        /// </value>
        public bool IsIdle { get; private set; }

        /// <summary>
        /// Gets the percent value.
        /// </summary>
        /// <value>
        /// The percent value.
        /// </value>
        public double Percent { get; private set; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public object Data { get; private set; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public string Category { get; private set; }

        /// <summary>
        /// Creates the status update event arguments.
        /// </summary>
        /// <param name="statusText">The status text.</param>
        /// <returns></returns>
        public static StatusUpdateEventArgs CreateStatusUpdateEventArgs(string statusText)
        {
            return new StatusUpdateEventArgs(StatusUpdateType.UpdateStatusText, statusText);
        }

        /// <summary>
        /// Creates the status update event arguments.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="statusText">The status text.</param>
        /// <returns></returns>
        public static StatusUpdateEventArgs CreateStatusUpdateEventArgs(string category, string statusText)
        {
            return new StatusUpdateEventArgs(StatusUpdateType.UpdateStatusText, category, statusText);
        }

        /// <summary>
        /// Creates the clear status event arguments.
        /// </summary>
        /// <returns></returns>
        public static StatusUpdateEventArgs CreateClearStatusEventArgs()
        {
            return new StatusUpdateEventArgs(StatusUpdateType.UpdateStatusText);
        }

        /// <summary>
        /// Creates the clear status event arguments.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public static StatusUpdateEventArgs CreateClearStatusEventArgs(string category)
        {
            return new StatusUpdateEventArgs(StatusUpdateType.UpdateStatusText) { Category = category };
        }

        /// <summary>
        /// Creates the busy status event arguments.
        /// </summary>
        /// <returns></returns>
        public static StatusUpdateEventArgs CreateBusyStatusEventArgs()
        {
            return new StatusUpdateEventArgs(StatusUpdateType.UpdateBusyIdle) { IsBusy = true, IsIdle = false };
        }

        /// <summary>
        /// Creates the idle status event arguments.
        /// </summary>
        /// <returns></returns>
        public static StatusUpdateEventArgs CreateIdleStatusEventArgs()
        {
            return new StatusUpdateEventArgs(StatusUpdateType.UpdateBusyIdle) { IsIdle = true, IsBusy = false };
        }

        /// <summary>
        /// Creates the percent status event arguments.
        /// </summary>
        /// <param name="percent">The percent.</param>
        /// <returns></returns>
        public static StatusUpdateEventArgs CreatePercentStatusEventArgs(double percent)
        {
            return new StatusUpdateEventArgs(StatusUpdateType.UpdatePercent, percent);
        }

        /// <summary>
        /// Creates the data status event arguments.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static StatusUpdateEventArgs CreateDataStatusEventArgs(object data)
        {
            return new StatusUpdateEventArgs(StatusUpdateType.UpdateData, data);
        }

        /// <summary>
        /// Creates the data status event arguments.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static StatusUpdateEventArgs CreateDataStatusEventArgs(string category, object data)
        {
            return new StatusUpdateEventArgs(StatusUpdateType.UpdateData, category, data);
        }
    }
}
