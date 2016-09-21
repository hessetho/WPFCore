using System;

namespace WPFCore.ViewModelSupport
{
    /// <summary>
    /// This attribute may be used by classes deriving from <see cref="ViewModelBase"/>
    /// to suppress trace outputs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class DontTraceMeAttribute : Attribute
    {
    }
}
