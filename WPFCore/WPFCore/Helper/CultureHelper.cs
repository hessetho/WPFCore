using System;
using System.Globalization;
using System.Threading;

namespace WPFCore.Helper
{
    /// <summary>
    /// Helper functions for <see cref="System.Globalization"/>
    /// </summary>
    public static class CultureHelper
    {

        /// <summary>
        /// Changes the culture of the current thread while preserving the original in an <see cref="IDisposable"/> object.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <remarks>
        /// The returned value stores the preserved culture. The class <see cref="PreservedCulture"/> implements the
        /// interface <see cref="IDisposable"/> so that this method can be used in a <c>using(...)</c> clause. This
        /// guarantees a save reversion to the original culture.
        /// </remarks>
        /// <returns></returns>
        public static PreservedCulture UseSpecificCulture(string culture)
        {
            var proxy = new PreservedCulture();
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            return proxy;
        }

        /// <summary>
        /// Changes the culture of the current thread to <see cref="CultureInfo.InvariantCulture"/> while preserving the original in an <see cref="IDisposable"/> object.
        /// </summary>
        /// <remarks>
        /// The returned value stores the preserved culture. The class <see cref="PreservedCulture"/> implements the
        /// interface <see cref="IDisposable"/> so that this method can be used in a <c>using(...)</c> clause. This
        /// guarantees a save reversion to the original culture.
        /// </remarks>
        /// <returns></returns>
        public static PreservedCulture UseInvariantCulture()
        {
            var proxy = new PreservedCulture();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            return proxy;
        }

        /// <summary>
        /// Resets to the original culture.
        /// </summary>
        /// <param name="cultureProxy">The culture proxy.</param>
        public static void ResetToOriginalCulture(PreservedCulture cultureProxy)
        {
            cultureProxy.Dispose();
        }

        #region class PreservedCulture

        /// <summary>
        /// Saves the current culture of the current thread and reverts to it when being disposed.
        /// </summary>
        public sealed class PreservedCulture : IDisposable
        {
            private readonly CultureInfo originalCulture;

            /// <summary>
            /// Initializes a new instance of the <see cref="PreservedCulture"/> class and saves the current culture of the current thread.
            /// </summary>
            public PreservedCulture()
            {
                this.originalCulture = Thread.CurrentThread.CurrentCulture;
            }

            /// <summary>
            /// Reverts the culture of the current thread to the prevered culture.
            /// </summary>
            public void Dispose()
            {
                Thread.CurrentThread.CurrentCulture = this.originalCulture;
            }
        }

        #endregion class PreservedCulture
    }
}
