namespace WPFCore.ViewModelSupport
{
    /// <summary>
    /// Gives access to the validation exceptions of an object
    /// </summary>
    interface IValidationExceptionHandler
    {
        /// <summary>
        /// Set's the total number of validation exceptions.
        /// </summary>
        /// <param name="count"></param>
        void ValidationExceptionsChanged(int count);
    }
}
