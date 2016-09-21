namespace WPFCore.ViewModelSupport
{
    /// <summary>
    /// Event handler delegate for the <see cref="ValidationViewModelBase.PropertyError"/> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyErrorEventArgs"/> instance containing the event data.</param>
    public delegate void PropertyErrorEventHandler (object sender, PropertyErrorEventArgs e);

    /// <summary>
    /// Event arguments for the <see cref="ValidationViewModelBase.PropertyError"/> event.
    /// </summary>
    public class PropertyErrorEventArgs
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName { get; private set; }
        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyErrorEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="errorMessage">The error message.</param>
        public PropertyErrorEventArgs(string propertyName, string errorMessage)
        {
            this.PropertyName = propertyName;
            this.ErrorMessage = errorMessage;
        }
    }
}
