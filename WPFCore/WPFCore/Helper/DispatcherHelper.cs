using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace WPFCore.Helper
{
    //[DebuggerStepThrough]
    public static class DispatcherHelper
    {

        /// <summary>
        /// Stellt sicher, dass eine Methode im (Ersteller-)Thread des Controls ausgeführt wird.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="methodCall"></param>
        public static void InvokeIfRequired(this DependencyObject control, Action methodCall)
        {
            InvokeIfRequired(control.Dispatcher, DispatcherPriority.Background, methodCall);
        }

        /// <summary>
        /// Stellt sicher, dass eine Methode im (Ersteller-)Thread des Controls ausgeführt wird.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="methodCall"></param>
        /// <param name="timeout"></param>
        public static void InvokeIfRequired(this DependencyObject control, Action methodCall, TimeSpan timeout)
        {
            InvokeIfRequired(control.Dispatcher, DispatcherPriority.Background, methodCall, timeout);
        }


        public static void InvokeIfRequired(Dispatcher dispatcher, Action methodCall)
        {
            InvokeIfRequired(dispatcher, DispatcherPriority.Background, methodCall);
        }

        public static void InvokeIfRequired(Dispatcher dispatcher, Action methodCall, TimeSpan timeout)
        {
            InvokeIfRequired(dispatcher, DispatcherPriority.Background, methodCall, timeout);
        }

        public static void InvokeIfRequired(Dispatcher dispatcher, DispatcherPriority priority, Action methodCall)
        {
            if (dispatcher.CheckAccess())
                methodCall();
            else
            {
#if DEBUG && DEBUGTHREADS
                Debug.WriteLine(string.Format("DispatcherHelper\r\n\tCurrent dispatcher: ({3}) \"{4}\"\r\n\tTarget dispatcher: ({1}) \"{2}\": {0}", dispatcher.Thread.ThreadState, dispatcher.Thread.ManagedThreadId, dispatcher.Thread.Name, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name));
                
                if(Application.Current==null || Application.Current.Dispatcher.HasShutdownStarted)
                    Debug.WriteLine("DispatcherHelper.InvokeIfRequired(): WARNING! Application is shutting down!");

                if (Application.Current != null && Application.Current.Dispatcher.Thread.ThreadState.IsFlagSet(System.Threading.ThreadState.Stopped))
                    Debug.WriteLine(string.Format("DispatcherHelper.InvokeIfRequired(): WARNING! Application thread is stopped! ({0})", Application.Current.Dispatcher.Thread.Name));
#endif

                dispatcher.Invoke(priority, methodCall);
            }

        }

        public static void InvokeIfRequired(Dispatcher dispatcher, DispatcherPriority priority, Action methodCall, TimeSpan timeout)
        {
            // we'll not switch the context if the current thread is being aborted
            //if ((Thread.CurrentThread.ThreadState & System.Threading.ThreadState.AbortRequested) == System.Threading.ThreadState.AbortRequested)
            //{
            //    TraceHelper.TraceWarning(string.Format("InvokeIfRequired cannot execute methodCall for thread '{0}'. Current thread ('{1}') is in state '{2}'.", dispatcher.Thread.Name, Thread.CurrentThread.Name, Thread.CurrentThread.ThreadState));
            //    return;
            //}

            if (dispatcher.Thread != Thread.CurrentThread)
                dispatcher.Invoke(priority, timeout, methodCall);
            else
                methodCall();
        }

        /// <summary>
        /// Ensures that a method/action is run on the main application thread.
        /// </summary>
        /// <param name="methodCall"></param>
        public static void InvokeOnAppDispatcher(Action methodCall)
        {
            if (Application.Current != null)
            {
#if DEBUG
                if (Application.Current.Dispatcher.Thread.ThreadState.IsFlagSet(System.Threading.ThreadState.Stopped))
                {
                    Debug.WriteLine(string.Format("DispatcherHelper.InvokeOnAppRequired(): WARNING! Application thread is stopped! {0}", Application.Current.Dispatcher.Thread.Name));
                    return;
                }
#endif

                if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, methodCall);
                else
                    methodCall();
            }
            else
                TraceHelper.TraceWarning("InvokeOnAppDispatcher cannot execute methodCall. Application.Current is NULL.");
        }
    }
}
