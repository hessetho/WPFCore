using System;

namespace WPFCore.ViewModelSupport
{
    /// <summary>
    /// This attribute is used to decorate classes which need to be
    /// validated in conjunction with other properties of the same instance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public abstract class ClassValidationAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets an error message to associate with a validation control if validation fails.
        /// </summary>
        public virtual string ErrorMessage { get; set; }

        /// <summary>
        /// Determines whether the instance of a class is valid.
        /// </summary>
        /// <param name="instance">The instance, which holds the current value.</param>
        /// <returns><c>True</c> if the value passed the validation, <c>False</c> otherwise.</returns>
        public abstract bool IsValid(object instance);
    }
}
