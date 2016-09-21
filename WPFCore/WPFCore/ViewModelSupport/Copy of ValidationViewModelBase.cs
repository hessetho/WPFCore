using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace WPFCore.ViewModelSupport
{
    /// <summary>
    ///     Extends the <see cref="ViewModelBase" /> class by property validation functions.
    /// </summary>
    public abstract class ValidationViewModelBase : ViewModelBase, IDataErrorInfo, IValidationExceptionHandler
    {
        private readonly Dictionary<string, InstanceValidationAttribute[]> instanceValidators;
        private readonly Dictionary<string, Func<ValidationViewModelBase, object>> propertyGetters;
        private readonly Dictionary<string, ValidationAttribute[]> validators;
        private readonly ClassValidationAttribute[] classValidators;
        private bool isValid;
        private int validationExceptionCount;

        /// <summary>
        /// Occurs when a property error was detected.
        /// </summary>
        public event PropertyErrorEventHandler PropertyError;

        /// <summary>
        ///     Constructor.
        /// </summary>
        [DebuggerStepThrough]
        public ValidationViewModelBase()
        {
            this.validators = this.GetType()
                .GetProperties()
                .Where(p => GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name, GetValidations);

            this.propertyGetters = this.GetType()
                .GetProperties()
                .Where(p => GetValidations(p).Length != 0)
                .ToDictionary(p => p.Name, GetValueGetter);

            this.instanceValidators = this.GetType()
                .GetProperties()
                .Where(p => GetInstanceValidations(p).Length != 0)
                .ToDictionary(p => p.Name, GetInstanceValidations);

            var getters = this.GetType()
                .GetProperties()
                .Where(p => GetInstanceValidations(p).Length != 0)
                .ToDictionary(p => p.Name, GetValueGetter);

            foreach (var getter in getters.Where(g => !this.propertyGetters.ContainsKey(g.Key)))
                this.propertyGetters.Add(getter.Key, getter.Value);

            this.classValidators = GetClassValidations(this.GetType());
        }

        /// <summary>
        ///     Gets (or sets) a flag indicating if the validations were successful (<c>True</c>) or failed (<c>False</c>)
        /// </summary>
        [DoesNotAffectChangesFlag]
        public bool IsValid
        {
            get { return this.isValid; }
            private set
            {
                //if (this.isValid == value) return;
                this.isValid = value;
                this.OnPropertyChanged("IsValid");
                this.OnPropertyChanged("Error");
            }
        }

        /// <summary>
        ///     Returns the total number of valid properties
        /// </summary>
        public int ValidPropertiesCount
        {
            get
            {
                var query = new List<object>();
                query.AddRange(
                    from validator in this.validators
                    where validator.Value.All(attribute => attribute.IsValid(this.propertyGetters[validator.Key](this)))
                    select validator.Value);
                query.AddRange(
                    from validator in this.instanceValidators
                    where
                        validator.Value.All(
                            attribute => attribute.IsValid(this, this.propertyGetters[validator.Key](this)))
                    select validator.Value);

                var count = query.Count() - this.validationExceptionCount;

                return count;
            }
        }

        /// <summary>
        ///     Returns the number of properties that do have validation rules (attributes)
        /// </summary>
        public int TotalPropertiesWithValidationCount
        {
            get { return this.validators.Count + this.instanceValidators.Count(); }
        }

        /// <summary>
        ///     Returns all error messages of a single property. If all
        ///     validations are passed successfully the result will be an empty string.
        /// </summary>
        /// <remarks>
        ///     This property effectively triggers the validation of a single property.
        /// </remarks>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>Error message text or an empty string if the validations were successful</returns>
        public virtual string this[string propertyName]
        {
            get
            {
                if (!this.propertyGetters.ContainsKey(propertyName))
                    return string.Empty;

                var propertyValue = this.propertyGetters[propertyName](this);
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
                        .Where(v => !v.IsValid(this, propertyValue))
                        .Select(v => v.ErrorMessage));
                }

                var error = string.Join(Environment.NewLine, errormessages);

                // raise the PropertyError event in case we detected an error
                if (!string.IsNullOrEmpty(error) && this.PropertyError != null)
                    this.PropertyError(this, new PropertyErrorEventArgs(propertyName, error));

                return error;
            }
        }

        /// <summary>
        ///     Returns all error messages of all properties
        ///     which failed validation.
        /// </summary>
        /// <remarks>
        ///     This property effectively triggers the validation of all properties.
        /// </remarks>
        [DoesNotAffectChangesFlag]
        public string Error
        {
            get
            {
                var errors = new List<string>();
                errors.AddRange(
                    from validator in this.validators
                    from attribute in validator.Value
                    where !attribute.IsValid(this.propertyGetters[validator.Key](this))
                    select attribute.ErrorMessage);
                errors.AddRange(
                    from validator in this.instanceValidators
                    from attribute in validator.Value
                    where !attribute.IsValid(this, this.propertyGetters[validator.Key](this))
                    select attribute.ErrorMessage);
                errors.AddRange(
                    from validator in this.classValidators
                    where !validator.IsValid(this)
                    select validator.ErrorMessage);

                return string.Join(Environment.NewLine, errors);
            }
        }

        /// <summary>
        ///     --- ??? ---
        /// </summary>
        /// <param name="count"></param>
        public void ValidationExceptionsChanged(int count)
        {
            this.validationExceptionCount = count;
            this.OnPropertyChanged("ValidPropertiesCount");
        }

        /// <summary>
        ///     Returns the property validation attributes for a property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static ValidationAttribute[] GetValidations(PropertyInfo property)
        {
            return (ValidationAttribute[]) property.GetCustomAttributes(typeof (ValidationAttribute), true);
        }

        /// <summary>
        ///     Returns the instance validation attributes for a property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static InstanceValidationAttribute[] GetInstanceValidations(PropertyInfo property)
        {
            return (InstanceValidationAttribute[]) property.GetCustomAttributes(typeof (InstanceValidationAttribute), true);
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

        /// <summary>
        ///     This method is called after a property has been changed (and the <see cref="ViewModelBase.PropertyChanged" />
        ///     event has been triggered). It's purpose is to trigger the object's validation rules by
        ///     accessing the <see cref="Error" /> propertty. If this is not empty, the <see cref="IsValid" />
        ///     property is set to <c>False</c> (<c>True</c> otherwise).
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void PropertyChangedCompleted(string propertyName)
        {
            if (this.IsInitializing) return;

            if (this.PropertiesDoNotAffectChangesFlag.Contains(propertyName) == false)
            {
                if (string.IsNullOrEmpty(this.Error) &&
                    this.ValidPropertiesCount == this.TotalPropertiesWithValidationCount)
                    this.IsValid = true;
                else
                    this.IsValid = false;
            }
        }

        /// <summary>
        ///     Initialize validation flag
        /// </summary>
        protected void Validate()
        {
            Debug.Assert(this.IsInitializing == false, "WARNING! InitializeValidation should NOT be called while IsInitialized is True. Call EndInit() prior.");

            if (string.IsNullOrEmpty(this.Error)
                && this.ValidPropertiesCount == this.TotalPropertiesWithValidationCount)
                this.IsValid = true;
            else
                this.IsValid = false;
        }
    }
}