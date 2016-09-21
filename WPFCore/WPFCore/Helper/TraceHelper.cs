using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WPFCore.Helper
{
    public static class TraceHelper
    {
        public static void TraceInformation(this TraceSource ts, string eventMessage)
        {
            ts.TraceEvent(TraceEventType.Information, 0, PimpMessage(eventMessage));
        }

        public static void TraceWarning(this TraceSource ts, string eventMessage)
        {
            ts.TraceEvent(TraceEventType.Warning, 0, PimpMessage(eventMessage));
        }

        public static void TraceError(this TraceSource ts, string eventMessage)
        {
            ts.TraceEvent(TraceEventType.Error, 0, PimpMessage(eventMessage));
        }

        public static void TraceDebug(this TraceSource ts, string eventMessage)
        {
            ts.TraceEvent(TraceEventType.Verbose, 0, PimpMessage(eventMessage));
        }

        public static void TraceException(this TraceSource ts, Exception e)
        {
            ts.TraceError(e.Message);
            Console.WriteLine("Exception encountered:");
            Console.WriteLine(e.Message);
        }

        #region trace method entry and exit
        public static bool TraceMethods { get; set; }

        public static void TraceMethodEntry([CallerMemberName] string methodName = null)
        {
            if(TraceMethods)
                Constants.CoreTraceSource.TraceDebug(string.Format("entering {0}", methodName));
        }

        public static void TraceMethodExit([CallerMemberName] string methodName = null)
        {
            if (TraceMethods)
                Constants.CoreTraceSource.TraceDebug(string.Format("leaving {0}", methodName));
        }
        #endregion trace method entry and exit

        #region CoreTraceSource Trace methods
        public static void TraceInformation(string eventMessage)
        {
            Constants.CoreTraceSource.TraceEvent(TraceEventType.Information, 0, PimpMessage(eventMessage));
        }

        public static void TraceWarning(string eventMessage)
        {
            Constants.CoreTraceSource.TraceEvent(TraceEventType.Warning, 0, PimpMessage(eventMessage));
        }

        public static void TraceError(string eventMessage)
        {
            Constants.CoreTraceSource.TraceEvent(TraceEventType.Error, 0, PimpMessage(eventMessage));
        }

        public static void TraceDebug(string eventMessage)
        {
            Constants.CoreTraceSource.TraceEvent(TraceEventType.Verbose, 0, PimpMessage(eventMessage));
        }
        #endregion CoreTraceSource Trace methods

        private static string PimpMessage(string message)
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss}\t{1}", DateTime.Now, message);
        }
    }
}