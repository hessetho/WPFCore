using System;
using System.Collections.Generic;
using System.Linq;

namespace WPFCore.ViewModelSupport
{
    /// <summary>
    /// This attribute describes the dependencies of the decorated property on other properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DependsOnAttribute : Attribute
    {
        /// <summary>
        /// List of properties on which this property depends on
        /// </summary>
        public List<string> Properties { get; private set; }

        /// <summary>
        /// Constructor. Takes a list of property names
        /// </summary>
        /// <param name="propertyNames"></param>
        public DependsOnAttribute(params string[] propertyNames)
        {
            this.Properties = propertyNames.ToList();
        }
    }
}
