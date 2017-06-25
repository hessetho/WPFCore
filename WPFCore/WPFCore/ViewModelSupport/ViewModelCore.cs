using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WPFCore.ViewModelSupport
{
    /// <summary>
    /// Abstract class that implements <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is advisable to ALWAYS implement <c>INotifyPropertyChanged</c> if a class is used in any binding
    /// operation - otherwise the UI might keep hooks on its instances and GC might not be
    /// able to collect the instances.
    /// </para>
    /// <para>
    /// To simplify the implementation of <c>INotifyPropertyChanged</c>, this abstract class provides a basic
    /// interface.
    /// </para>
    /// </remarks>
    public abstract class ViewModelCore : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a property of this instance changed. If an empty string is passed, 
        /// all properties are regarded as having changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.VerifyPropertyName(propertyName);

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// This helper verifies that the provided property name
        /// refers to an existing property of the class. (Debug only)
        /// </summary>
        /// <param name="propertyName"></param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        protected void VerifyPropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;

            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new ArgumentException(string.Format("({0}) Invalid property name: {1}", this.GetType().Name, propertyName));
        }
    }
}
