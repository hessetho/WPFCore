using System;
using System.Collections.Generic;

namespace WPFCore.ViewModelSupport
{
    public static class DialogServiceExt
    {
        public delegate bool OpenDialogDelegate(object sender, object dataItem, object state);

        public delegate void OpenWindowDelegate(object sender, object dataItem, object state);

        private static readonly Dictionary<object, OpenWindowDelegate> RegisteredWindows =
            new Dictionary<object, OpenWindowDelegate>();

        private static readonly Dictionary<object, OpenDialogDelegate> RegisteredDialogs =
            new Dictionary<object, OpenDialogDelegate>();

        /// <summary>
        ///     Register a window for a specific data type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RegisterWindow<T>(OpenWindowDelegate openWindowCallback)
        {
            var callback = GetOpenWindowCallback(typeof(T));

            RegisteredWindows[typeof (T)] = openWindowCallback;
        }

        public static void RegisterDialog<T>(OpenDialogDelegate openDialogCallback)
        {
            var callback = GetOpenDialogCallback(typeof(T));

            RegisteredDialogs[typeof(T)] = openDialogCallback;
        }

        public static void OpenWindow(object sender, object dataItem)
        {
            if (dataItem == null) throw new ArgumentNullException("dataItem");

            var callback = GetOpenWindowCallback(dataItem.GetType());
            if (callback == null) throw new InvalidOperationException("no window callback registered");

            callback(sender, dataItem, null);
        }

        public static void OpenWindow(object sender, object dataItem, object state)
        {
            if (dataItem == null) throw new ArgumentNullException("dataItem");

            var callback = GetOpenWindowCallback(dataItem.GetType());
            if (callback == null) throw new InvalidOperationException("no window callback registered");

            callback(sender, dataItem, state);
        }

        public static bool OpenDialog(object sender, object dataItem)
        {
            if (dataItem == null) throw new ArgumentNullException("dataItem");

            var callback = GetOpenDialogCallback(dataItem.GetType());
            if (callback == null) throw new InvalidOperationException("no dialog callback registered");

            return callback(sender, dataItem, null);
        }

        public static bool OpenDialog(object sender, object dataItem, object state)
        {
            if (dataItem == null) throw new ArgumentNullException("dataItem");

            var callback = GetOpenDialogCallback(dataItem.GetType());
            if (callback == null) throw new InvalidOperationException("no dialog callback registered");

            return callback(sender, dataItem, state);
        }

        /// <summary>
        /// Returns the callback registered for a specific data type
        /// </summary>
        /// <param name="dataItemType">Data type.</param>
        /// <returns></returns>
        private static OpenWindowDelegate GetOpenWindowCallback(Type dataItemType)
        {
            if (!RegisteredWindows.ContainsKey(dataItemType))
                RegisteredWindows.Add(dataItemType, null);

            return RegisteredWindows[dataItemType];
        }

        /// <summary>
        /// Returns the callback registered for a specific data type
        /// </summary>
        /// <param name="dataItemType">Data type.</param>
        /// <returns></returns>
        private static OpenDialogDelegate GetOpenDialogCallback(Type dataItemType)
        {
            if (!RegisteredDialogs.ContainsKey(dataItemType))
                RegisteredDialogs.Add(dataItemType, null);

            return RegisteredDialogs[dataItemType];
        }
    
    }
}