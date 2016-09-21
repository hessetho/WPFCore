using System;
using System.Diagnostics;

namespace WPFCore.Helper
{
    public static class DiagnosticsHelper
    {
        /// <summary>
        /// Writes a message to the debug output.
        /// </summary>
        /// <param name="msg">The message.</param>
        public static void WriteLine(string msg)
        {
            Debug.WriteLine(string.Format("{0:mm:ss.ffff}\t{1}", DateTime.Now, msg));
        }

        /// <summary>
        /// Writes a message to the debug output.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="values">The values.</param>
        public static void WriteLine(string msg, params object[] values)
        {
            WriteLine(string.Format(msg, values));
        }

        /// <summary>
        /// Writes a message to the debug output.
        /// </summary>
        /// <param name="msg">The message.</param>
        public static void Write(string msg)
        {
            Debug.Write(msg);
        }

        /// <summary>
        /// Writes a message to the debug output.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="values">The values.</param>
        public static void Write(string msg, params object[] values)
        {
            Debug.Write(string.Format(msg, values));
        }
    }
}
