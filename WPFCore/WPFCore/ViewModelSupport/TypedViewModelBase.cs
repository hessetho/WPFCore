using System.Diagnostics;

namespace WPFCore.ViewModelSupport
{
    /// <summary>
    /// Represents Aa typed ViewModelBase
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerStepThrough]
    public abstract class TypedViewModelBase<T> : ViewModelBase
    {
        /// <summary>
        /// The base element of this view model item.
        /// </summary>
        public readonly T BaseElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedViewModelBase{T}"/> class.
        /// </summary>
        /// <param name="baseElement">The base element.</param>
        public TypedViewModelBase(T baseElement)
        {
            this.BaseElement = baseElement;
        }
    }
}
