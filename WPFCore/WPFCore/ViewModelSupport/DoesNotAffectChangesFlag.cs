using System;

namespace WPFCore.ViewModelSupport
{
    /// <summary>
    /// Used to decorate properties which will not affect the <see cref="ViewModelBase.HasChanges"/> property
    /// of the <see cref="ViewModelBase"/> class and its descendants.
    /// </summary>
    public class DoesNotAffectChangesFlagAttribute : Attribute
    {
    }
}
