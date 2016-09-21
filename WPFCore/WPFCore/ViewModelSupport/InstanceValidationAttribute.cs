using System;

namespace WPFCore.ViewModelSupport
{
    /// <summary>
    /// This attribute is used to decorate properties which need to be
    /// validated in conjunction with other properties of the same instance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public abstract class InstanceValidationAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets an error message to associate with a validation control if validation fails.
        /// </summary>
        public virtual string ErrorMessage { get; set; }

        /// <summary>
        /// Determines whether the specified value of the object is valid. 
        /// </summary>
        /// <param name="instance">The instance, which holds the current value.</param>
        /// <param name="value">The current value of the property</param>
        /// <returns><c>True</c> if the value passed the validation, <c>False</c> otherwise.</returns>
        public abstract bool IsValid(object instance, object value);
    }
}
