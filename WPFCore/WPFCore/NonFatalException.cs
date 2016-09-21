using System;

namespace WPFCore
{
    /// <summary>
    /// Represents a "non-fatal" exception used in conjunction with <see cref="XAML.Controls.ExceptionGallery"/>
    /// </summary>
    /// <remarks>
    /// Use this exception to wrap any application exception that should be treated as "non-fatal", i.e. after showing
    /// the exception and closing the window (e.g. <see cref="AMRisk.Core.UI.Common.ExceptionWindow"/>) the application will not be
    /// shut down.
    /// </remarks>
    public class NonFatalException : Exception
    {
        /// <summary>
        /// Gets a value indicating whether to remove this wrapper exception.
        /// </summary>
        /// <value>
        /// <c>true</c> if [remove wrapper exception]; otherwise, <c>false</c>.
        /// </value>
        public bool RemoveWrapperException { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonFatalException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public NonFatalException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.RemoveWrapperException = false;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NonFatalException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public NonFatalException(Exception innerException)
            : base("Non-Fatal exception", innerException) 
        {
            this.RemoveWrapperException = true;
        }
    }
}
