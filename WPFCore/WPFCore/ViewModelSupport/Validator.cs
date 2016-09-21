using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace WPFCore.ViewModelSupport
{
    internal class Validator
        {
            private readonly Dictionary<string, InstanceValidationAttribute[]> instanceValidators;
            private readonly Dictionary<string, Func<ValidationViewModelBase, object>> propertyGetters;
            private readonly Dictionary<string, ValidationAttribute[]> validators;
            private readonly ClassValidationAttribute[] classValidators;

            public Validator(Type type)
            {
                if (!type.IsSubclassOf(typeof(ValidationViewModelBase)))
                    throw new InvalidOperationException("Validator expects ValidationViewModelBase as derived type");

                this.validators = type
                    .GetProperties()
                    .Where(p => GetValidations(p).Length != 0)
                    .ToDictionary(p => p.Name, GetValidations);

                this.propertyGetters = type
                    .GetProperties()
                    .Where(p => GetValidations(p).Length != 0)
                    .ToDictionary(p => p.Name, GetValueGetter);

                this.instanceValidators = type
                    .GetProperties()
                    .Where(p => GetInstanceValidations(p).Length != 0)
                    .ToDictionary(p => p.Name, GetInstanceValidations);

                var getters = type
                    .GetProperties()
                    .Where(p => GetInstanceValidations(p).Length != 0)
                    .ToDictionary(p => p.Name, GetValueGetter);

                foreach (var getter in getters.Where(g => !this.propertyGetters.ContainsKey(g.Key)))
                    this.propertyGetters.Add(getter.Key, getter.Value);

                this.classValidators = GetClassValidations(type);
            }

            //public Dictionary<string, Func<ValidationViewModelBase, object>> PropertyGetters { get { return propertyGetters; } }
            //public Dictionary<string, InstanceValidationAttribute[]> InstanceValidators { get { return instanceValidators; } }
            //public Dictionary<string, ValidationAttribute[]> Validators { get { return validators; } }
            //public ClassValidationAttribute[] ClassValidators { get { return classValidators; } }

            public int TotalPropertiesWithValidationCount
            {
                get { return this.validators.Count + this.instanceValidators.Count(); }
            }

            public int GetValidPropertiesCount(ValidationViewModelBase itm)
            {
                var query = new List<object>();
                query.AddRange(
                    from validator in this.validators
                    where validator.Value.All(attribute => attribute.IsValid(this.propertyGetters[validator.Key](itm)))
                    select validator.Value);
                query.AddRange(
                    from validator in this.instanceValidators
                    where
                        validator.Value.All(
                            attribute => attribute.IsValid(itm, this.propertyGetters[validator.Key](itm)))
                    select validator.Value);

                return query.Count();
            }

            public string GetError(ValidationViewModelBase itm)
            {
                var errors = new List<string>();
                errors.AddRange(
                    from validator in this.validators
                    from attribute in validator.Value
                    where !attribute.IsValid(this.propertyGetters[validator.Key](itm))
                    select attribute.ErrorMessage);
                errors.AddRange(
                    from validator in this.instanceValidators
                    from attribute in validator.Value
                    where !attribute.IsValid(itm, this.propertyGetters[validator.Key](itm))
                    select attribute.ErrorMessage);
                errors.AddRange(
                    from validator in this.classValidators
                    where !validator.IsValid(itm)
                    select validator.ErrorMessage);

                return string.Join(Environment.NewLine, errors);
            }

            public string GetPropertyError(ValidationViewModelBase itm, string propertyName)
            {
                if (!this.propertyGetters.ContainsKey(propertyName))
                    return string.Empty;

                var propertyValue = this.propertyGetters[propertyName](itm);
                var errormessages = new List<string>();

                if (this.validators.ContainsKey(propertyName))
                {
                    errormessages.AddRange(this.validators[propertyName]
                        .Where(v => !v.IsValid(propertyValue))
                        .Select(v => v.ErrorMessage));
                }

                if (this.instanceValidators.ContainsKey(propertyName))
                {
                    errormessages.AddRange(this.instanceValidators[propertyName]
                        .Where(v => !v.IsValid(itm, propertyValue))
                        .Select(v => v.ErrorMessage));
                }

                return string.Join(Environment.NewLine, errormessages);
            }
    
            /// <summary>
            ///     Returns the property validation attributes for a property
            /// </summary>
            /// <param name="property"></param>
            /// <returns></returns>
            private static ValidationAttribute[] GetValidations(PropertyInfo property)
            {
                return (ValidationAttribute[])property.GetCustomAttributes(typeof(ValidationAttribute), true);
            }

            /// <summary>
            ///     Returns the instance validation attributes for a property
            /// </summary>
            /// <param name="property"></param>
            /// <returns></returns>
            private static InstanceValidationAttribute[] GetInstanceValidations(PropertyInfo property)
            {
                return (InstanceValidationAttribute[])property.GetCustomAttributes(typeof(InstanceValidationAttribute), true);
            }

            /// <summary>
            ///     Returns the instance validation attributes for a property
            /// </summary>
            /// <param name="property"></param>
            /// <returns></returns>
            private static ClassValidationAttribute[] GetClassValidations(Type type)
            {
                return (ClassValidationAttribute[])type.GetCustomAttributes(typeof(ClassValidationAttribute), true);
            }

            /// <summary>
            ///     Returns a function to access the value of a property
            /// </summary>
            /// <param name="property">Name of the property</param>
            /// <returns>The getter-function to access the properties value</returns>
            private static Func<ValidationViewModelBase, object> GetValueGetter(PropertyInfo property)
            {
                return viewmodel => property.GetValue(viewmodel, null);
            }
    }
}
