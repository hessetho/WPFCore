using System;
using System.Collections.Generic;
using System.Linq;

namespace WPFCore.StatusText
{
    public delegate void StatusUpdateEventHandler(object sender, StatusUpdateEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public static class StatusTextBroker
    {
        /// <summary>
        /// The events attached to the channels
        /// </summary>
        private static readonly Dictionary<string, StatusUpdateEventHandler> ChanneledEvents = new Dictionary<string, StatusUpdateEventHandler>();

        /// <summary>
        /// The last status sent through each channel
        /// </summary>
        private static readonly Dictionary<string, StatusUpdateEventArgs> LastChanneledStatus = new Dictionary<string, StatusUpdateEventArgs>();

        /// <summary>
        /// A unique channel identifier, used to create unique channel names
        /// </summary>
        private static int uniqueChannelId;

        /// <summary>
        /// Returns a list of all known channel names
        /// </summary>
        public static List<string> Channels
        {
            get { return ChanneledEvents.Select(c => c.Key).ToList(); }
        }

        /// <summary>
        ///     Ereignis wird ausgelöst, wenn ein neuer Kanal erzeugt worden ist.
        /// </summary>
        public static event EventHandler<string> ChannelAdded;

        /// <summary>
        /// Creates a unique channel name.
        /// </summary>
        /// <remarks>
        /// The channel name is of the form "Channel_nn" with "nn" being an integer
        /// </remarks>
        /// <returns></returns>
        public static string GetUniqueChannelName()
        {
            return string.Format("Channel_{0}", uniqueChannelId++);
        }

        /// <summary>
        /// Creates a (prefixed) unique channel name.
        /// </summary>
        /// <param name="channelPrefix">The channel prefix.</param>
        /// <returns></returns>
        public static string GetUniqueChannelName(string channelPrefix)
        {
            return string.Format("{0}_{1}", channelPrefix, uniqueChannelId++);
        }

        /// <summary>
        /// Updates the status text.
        /// </summary>
        /// <param name="channel">Der Kanal</param>
        /// <param name="sender"></param>
        /// <param name="statusText">Der Statustext.</param>
        public static void UpdateStatusText(string channel, object sender, string statusText)
        {
            SetChannelStatus(channel, sender, StatusUpdateEventArgs.CreateStatusUpdateEventArgs(statusText));
        }

        /// <summary>
        /// Updates the status text.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="statusCategory">The status category.</param>
        /// <param name="statusText">The status text.</param>
        public static void UpdateStatusText(string channel, object sender, string statusCategory, string statusText)
        {
            SetChannelStatus(channel, sender, StatusUpdateEventArgs.CreateStatusUpdateEventArgs(statusCategory, statusText));
        }

        /// <summary>
        /// Updates the status data.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="data">The data.</param>
        public static void UpdateStatusData(string channel, object sender, object data)
        {
            SetChannelStatus(channel, sender, StatusUpdateEventArgs.CreateDataStatusEventArgs(data));
        }

        /// <summary>
        /// Updates the status data.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="statusCategory">The status category.</param>
        /// <param name="data">The data.</param>
        public static void UpdateStatusData(string channel, object sender, string statusCategory, object data)
        {
            SetChannelStatus(channel, sender, StatusUpdateEventArgs.CreateDataStatusEventArgs(statusCategory, data));
        }

        /// <summary>
        /// Clears the status of a channel.
        /// </summary>
        /// <remarks>
        /// Does not send a ClearStatus, if this was sent already previously.
        /// </remarks>
        /// <param name="channel">Der Kanal</param>
        /// <param name="sender"></param>
        public static void ClearStatus(string channel, object sender)
        {
            var lastStatus = GetChannelStatus(channel);
            if(lastStatus==null || !lastStatus.ClearStatus) 
                SetChannelStatus(channel, sender, StatusUpdateEventArgs.CreateClearStatusEventArgs());
        }

        /// <summary>
        /// Clears the status of a channels category.
        /// </summary>
        /// <remarks>
        /// Does not send a ClearStatus, if this was sent already previously.
        /// </remarks>
        /// <param name="channel">The channel.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="category">The category.</param>
        public static void ClearStatus(string channel, object sender, string category)
        {
            var lastStatus = GetChannelStatus(channel);
            if (lastStatus == null || !lastStatus.ClearStatus)
                SetChannelStatus(channel, sender, StatusUpdateEventArgs.CreateClearStatusEventArgs(category));
        }

        /// <summary>
        /// Signals the busy status of the sender.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sender">The sender.</param>
        public static void SignalBusy(string channel, object sender)
        {
            SetChannelStatus(channel, sender, StatusUpdateEventArgs.CreateBusyStatusEventArgs());
        }

        /// <summary>
        /// Signals the idle status of the sender.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sender">The sender.</param>
        public static void SignalIdle(string channel, object sender)
        {
            SetChannelStatus(channel, sender, StatusUpdateEventArgs.CreateIdleStatusEventArgs());
        }

        /// <summary>
        /// Shows the progress.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="percent">The percent.</param>
        public static void ShowProgress(string channel, object sender, double percent)
        {
            SetChannelStatus(channel, sender, StatusUpdateEventArgs.CreatePercentStatusEventArgs(percent));
        }

        /// <summary>
        ///     Fügt einem Kanal einen Listener hinzu
        /// </summary>
        /// <param name="listener"></param>
        internal static void AddStatusListener(StatusTextListener listener)
        {
            var evt = GetChannel(listener.Channel);
            ChanneledEvents[listener.Channel] = (StatusUpdateEventHandler)Delegate.Combine(evt, (StatusUpdateEventHandler)listener.OnStatusUpdate);
        }

        /// <summary>
        ///     Entfernt einen Listener für einen Kanal
        /// </summary>
        /// <param name="listener">Der zu entfernende Listener</param>
        public static void RemoveStatusListener(StatusTextListener listener)
        {
            var evt = GetChannel(listener.Channel);
            ChanneledEvents[listener.Channel] = (StatusUpdateEventHandler)Delegate.Remove(evt, (StatusUpdateEventHandler)listener.OnStatusUpdate);
        }

        /// <summary>
        ///     Liefert den/die Handler für einen Channel. Dieser wird ggf. zuerst angelegt
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        private static StatusUpdateEventHandler GetChannel(string channel)
        {
            try
            {
                if (!ChanneledEvents.ContainsKey(channel))
                {
                    ChanneledEvents.Add(channel, null);

                    if (ChannelAdded != null)
                        ChannelAdded(null, channel);
                }

                return ChanneledEvents[channel];
            }
            catch (Exception e)
            {
                e.Data.Add("channel", channel);
                throw e;
            }
        }

        /// <summary>
        /// Sets the current channel status.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="status">The <see cref="StatusUpdateEventArgs"/> instance containing the event data.</param>
        private static void SetChannelStatus(string channel, object sender, StatusUpdateEventArgs status)
        {
            if (!LastChanneledStatus.ContainsKey(channel))
                LastChanneledStatus.Add(channel, status);
            else
                LastChanneledStatus[channel] = status;

            var evt = GetChannel(channel);
            if (evt != null)
                evt(sender, status);
        }

        /// <summary>
        /// Gets the current channel status.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        public static StatusUpdateEventArgs GetChannelStatus(string channel)
        {
            if (!LastChanneledStatus.ContainsKey(channel))
                return null;
            else
                return LastChanneledStatus[channel];
        }

        #region Deprecated
        /// <summary>
        ///     Fügt einem Kanal einen Listener hinzu
        /// </summary>
        /// <param name="channel">Der Kanal</param>
        /// <param name="listenerCallback">Der Ereignis-Handler</param>
        [Obsolete("Please instantiate the appropriate StatusTextListener and use its properties and events")]
        public static void AddChannelListener(string channel, StatusUpdateEventHandler listenerCallback)
        {
            var evt = GetChannel(channel);
            ChanneledEvents[channel] = (StatusUpdateEventHandler)Delegate.Combine(evt, listenerCallback);
        }

        /// <summary>
        ///     Entfernt einen Listener für einen Kanal
        /// </summary>
        /// <param name="channel">Der Kanal</param>
        /// <param name="listenerCallback">Der zu entfernende Ereignis-Handler</param>
        [Obsolete("Please instantiate the appropriate StatusTextListener and use its properties and events")]
        public static void RemoveChannelListener(string channel, StatusUpdateEventHandler listenerCallback)
        {
            var evt = GetChannel(channel);
            ChanneledEvents[channel] = (StatusUpdateEventHandler)Delegate.Remove(evt, listenerCallback);
        }
        #endregion Deprecated
    }
}