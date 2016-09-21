using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace WPFCore.ViewModelSupport
{
    public abstract class ExtendedValidationViewModelBase<TBaseDataType> : ValidationViewModelBase
    {
        private readonly Collection<string> collectedProperties = new Collection<string>();

        /// <summary>
        ///     list of all properties with dependencies
        /// </summary>
        private readonly Dictionary<string, string[]> propertyDependencies = new Dictionary<string, string[]>();

        /// <summary>
        ///     value buffer for all properties
        /// </summary>
        private readonly Dictionary<string, object> propertyValues = new Dictionary<string, object>();

        protected ExtendedValidationViewModelBase(TBaseDataType baseElement)
        {
            this.BaseDataElement = baseElement;

            // scan all properties
            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                // find all properties with the DependsOn attribute
                string[] props = this.GetType().GetProperties()
                                          .Where(x => x.GetCustomAttributes(typeof (DependsOnAttribute), false)
                                                       .Cast<DependsOnAttribute>()
                                                       .Any(y => y.Properties.Any(z => z == property.Name)))
                                          .Select(x => x.Name).ToArray();

                // if there's any property that depends on the current property, add this property 
                // and its dependencies to the dependencies list
                if (props.Any())
                    this.propertyDependencies.Add(property.Name, props);
            }
        }

        /// <summary>
        ///     Returns the
        /// </summary>
        public TBaseDataType BaseDataElement { get; protected set; }

        /// <summary>
        ///     Setter. Sets the value of a property and triggers dependency updates
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="value">New value of the property</param>
        /// <param name="member">Name of the calling property</param>
        /// <returns>
        ///     <c>True</c> if the properties value has changed
        /// </returns>
        protected bool Set<T>(T value, [CallerMemberName] string member = "")
        {
            // Validierung
            if (string.IsNullOrEmpty(member))
                throw new ArgumentException("Value does not fall within the expected range.", "member");

            if (this.GetType().GetProperty(member) == null)
                throw new InvalidOperationException("member is not a public property.");

            // Thread-safe
            Monitor.Enter(LockObj);

            T currentValue = default(T);
            object savedValue;

            // get the current value of the property
            if (this.propertyValues.TryGetValue(member, out savedValue))
                currentValue = (T) savedValue;

            // check if we are changing the value
            if (!Equals(value, currentValue))
            {
                // store the new value
                if (this.propertyValues.ContainsKey(member))
                    this.propertyValues[member] = value;
                else
                    this.propertyValues.Add(member, value);
            }

            // during initialization, we're only collecting the property, otherwise we'll signal the change to others
            if (base.IsInitializing)
            {
                if (!this.collectedProperties.Contains(member))
                    this.collectedProperties.Add(member);
            }
            else
            {
                OnPropertyChanged(member);
            }

            // if we have dependencies from other properties, signal a change of them
            if (this.propertyDependencies.ContainsKey(member))
                foreach (string propName in this.propertyDependencies[member])
                    OnPropertyChanged(propName);

            Monitor.Exit(LockObj);

            return !Equals(value, currentValue);
        }

        /// <summary>
        ///     Getter. Returns the current value of a property
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="member">Name of the calling property</param>
        /// <returns>The actual value of the property</returns>
        protected T Get<T>([CallerMemberName] string member = "")
        {
            if (this.propertyValues.ContainsKey(member))
                return (T) this.propertyValues[member];

            return default(T);
        }

        protected internal Dictionary<string, object> GetStoredValues()
        {
            return new Dictionary<string, object>(this.propertyValues);
        }
    }
}