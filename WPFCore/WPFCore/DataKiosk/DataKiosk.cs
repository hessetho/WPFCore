using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WPFCore.DataKiosk
{
    /// <summary>
    /// Represents the method that will receive a published value.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DataPublishedEventArgs"/> instance containing the event data.</param>
    public delegate void DataPublishedEventHandler(object sender, DataPublishedEventArgs e);

    /// <summary>
    /// This class provides "subscribers" with values published by "publishers". 
    /// Similar to a magazine kiosk, which publishes different magazine types, this class publishes
    /// different data types, i.e. a subscriber may subscribe to a specific data type.
    /// </summary>
    /// <remarks>
    /// This static class implements the Subscriber-Publisher pattern
    /// </remarks>
    public static class DataKiosk
    {
        /// <summary>
        /// This list maintains the subscriberEventHandler
        /// </summary>
        private static readonly Dictionary<Type, DataPublishedEventHandler> Subscribers = new Dictionary<Type, DataPublishedEventHandler>();
        /// <summary>
        /// This list keeps the last received value for the data types
        /// </summary>
        private static readonly Dictionary<Type, object> LastPublishedValues = new Dictionary<Type, object>();

        /// <summary>
        /// Subscribe to a specific data type
        /// </summary>
        /// <typeparam name="T">the data type for which to receive values</typeparam>
        /// <param name="subscriber">the subscribing element</param>
        public static void Subscribe<T>(IDataSubscriber subscriber)
        {
            //Debug.WriteLine(string.Format("DataKiosk subscriber: {0}, type {1}", subscriber.GetType().Name, typeof(T)));

            var subscriberEventHandler = GetSubscriptions(typeof(T));
            Subscribers[typeof(T)] =
                (DataPublishedEventHandler)Delegate.Combine(subscriberEventHandler, (DataPublishedEventHandler)subscriber.OnDataPublished);

        }

        /// <summary>
        /// Subscribe to a specific data type
        /// </summary>
        /// <typeparam name="T">the data type for which to receive values</typeparam>
        /// <param name="dataPublishCallback">a callback to handle the subsribed value</param>
        public static void Subscribe<T>(DataPublishedEventHandler dataPublishCallback)
        {
            //Debug.WriteLine(string.Format("DataKiosk subscriber: {0}, type {1}", subscriber.GetType().Name, typeof(T)));

            var subscriberEventHandler = GetSubscriptions(typeof(T));
            Subscribers[typeof(T)] =
                (DataPublishedEventHandler)Delegate.Combine(subscriberEventHandler, dataPublishCallback);

        }

        /// <summary>
        /// Unsubscribe from a specific data type
        /// </summary>
        /// <typeparam name="T">the data type for which to receive values</typeparam>
        /// <param name="subscriber">the unsubscribing element</param>
        public static void Unsubscribe<T>(IDataSubscriber subscriber)
        {
            var subscriberEventHandler = GetSubscriptions(typeof(T));
            if (subscriberEventHandler == null) return;

            Subscribers[typeof(T)] = (DataPublishedEventHandler)Delegate.Remove(subscriberEventHandler, (DataPublishedEventHandler)subscriber.OnDataPublished);

            // if the last subscriber has been removed, then we remove the last value as well
            if (subscriberEventHandler.GetInvocationList().Count() == 0)
                LastPublishedValues.Remove(typeof(T));
        }

        /// <summary>
        /// Publish a data element
        /// </summary>
        /// <param name="sender">the sender of the published data element</param>
        /// <param name="dataItem">data element to publish</param>
        public static void Publish(object sender, object dataItem)
        {
            System.Diagnostics.Debug.Assert(dataItem != null, "dataItem is NULL. Use PublishNull instead!");

            object currentValue = null;
            if(!LastPublishedValues.TryGetValue(dataItem.GetType(), out currentValue))
                LastPublishedValues.Add(dataItem.GetType(), null);

            // do not re-send the same item
            if (currentValue == dataItem) return;

            var subscriberEventHandler = GetSubscriptions(dataItem.GetType());
            if (subscriberEventHandler != null)
                subscriberEventHandler(sender, new DataPublishedEventArgs(dataItem));

            // keep a copy of the published value
            LastPublishedValues[dataItem.GetType()] = dataItem;
        }

        /// <summary>
        /// Publish a null value for a specific data type.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="sender">The sender of the null value.</param>
        public static void PublishNull<T>(object sender)
        {
            var subscriberEventHandler = GetSubscriptions(typeof(T));
            if (subscriberEventHandler != null)
                subscriberEventHandler(sender, new DataPublishedEventArgs(null));

            // remove copy of the last published value (i.e. set the last value to null)
            if (!LastPublishedValues.ContainsKey(typeof(T)))
                LastPublishedValues.Add(typeof(T), null);
            else
                LastPublishedValues[typeof(T)] = null;
        }

        /// <summary>
        /// Gets the last published value for a specific data type.
        /// </summary>
        /// <typeparam name="T">The data type for which to get the value.</typeparam>
        /// <returns></returns>
        public static object GetLastPublishedValue<T>()
        {
            if (LastPublishedValues.ContainsKey(typeof(T)))
                return LastPublishedValues[typeof(T)];

            return null;
        }

        /// <summary>
        /// Clear all internal buffers
        /// </summary>
        public static void CleanUp()
        {
            Subscribers.Clear();
            LastPublishedValues.Clear();
        }

        /// <summary>
        /// Returns the subscription event handler for a specific data type
        /// </summary>
        /// <param name="dataItemType">Data type.</param>
        /// <returns></returns>
        private static DataPublishedEventHandler GetSubscriptions(Type dataItemType)
        {
            if (!Subscribers.ContainsKey(dataItemType))
                Subscribers.Add(dataItemType, null);

            return Subscribers[dataItemType];
        }
    }
}
