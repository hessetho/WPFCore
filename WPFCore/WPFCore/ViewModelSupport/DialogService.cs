using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFCore.ViewModelSupport
{
    public static class DialogService
    {
        private static OpenDialogDelegate openDialogCallback;
        private static OpenWindowDelegate openWindowCallback;

        public static void RegisterCallbacks(OpenDialogDelegate openDialogCallback,
            OpenWindowDelegate openWindowCallback)
        {
            if (DialogService.openWindowCallback != null)
            {
                throw new InvalidOperationException(
                    "it is not allowed to call DialogService.RegisterCallbacks() again, after DialogService was already initialized.");
            }
            if (openDialogCallback == null)
                throw new ArgumentNullException("openDialogCallback");
            if (openWindowCallback == null)
                throw new ArgumentNullException("openWindowCallback");

            DialogService.openDialogCallback = openDialogCallback;
            DialogService.openWindowCallback = openWindowCallback;
        }

        /// <summary>
        ///     Requests to open a window containing the provided content control
        /// </summary>
        /// <param name="contentControl"></param>
        /// <param name="title"></param>
        public static void OpenWindow(ContentControl contentControl, string title)
        {
            if (openWindowCallback == null)
            {
                throw new InvalidOperationException(
                    "Use DialogService.RegisterCallbacks() to initialize the DialogService");
            }

            openWindowCallback(contentControl, title);
        }

        /// <summary>
        ///     Requests to open a dialog containing the provided content control and returning <c>True</c> or <c>False</c>
        /// </summary>
        /// <param name="contentControl"></param>
        /// <returns></returns>
        public static bool OpenDialog(ContentControl contentControl, string title)
        {
            if (openWindowCallback == null)
            {
                throw new InvalidOperationException(
                    "Use DialogService.RegisterCallbacks() to initialize the DialogService");
            }

            return openDialogCallback(contentControl, title);
        }

        public static MessageBoxResult MessageBox(string messageBoxText, string caption, MessageBoxButton messageBoxButton, MessageBoxImage icon)
        {
            return System.Windows.MessageBox.Show(messageBoxText, caption, messageBoxButton, icon);
        }
    }
}