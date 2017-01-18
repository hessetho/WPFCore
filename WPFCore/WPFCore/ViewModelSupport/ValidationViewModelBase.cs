using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace WPFCore.ViewModelSupport
{
    /// <summary>
    ///     Extends the <see cref="ViewModelBase" /> class by property validation functions.
    /// </summary>
    public abstract class ValidationViewModelBase : ViewModelBase, IDataErrorInfo, IValidationExceptionHandler
    {
        private static Dictionary<Type, Validator> TypeValidators = new Dictionary<Type, Validator>();

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
            RegisterValidator(this.GetType());
        }

        private static object lockObj = new object();

        private static void RegisterValidator(Type classType)
        {
            Monitor.Enter(lockObj);
            if (!TypeValidators.ContainsKey(classType))
                TypeValidators.Add(classType, new Validator(classType));
            Monitor.Exit(lockObj);
        }

        private Validator Validator { get { return TypeValidators[this.GetType()]; } }

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
                return this.Validator.GetValidPropertiesCount(this) - this.validationExceptionCount;
            }
        }

        /// <summary>
        ///     Returns the number of properties that do have validation rules (attributes)
        /// </summary>
        public int TotalPropertiesWithValidationCount
        {
            get { return this.Validator.TotalPropertiesWithValidationCount; }
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
                var error = this.Validator.GetPropertyError(this, propertyName);

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
                return this.Validator.GetError(this);
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

        private readonly List<string> ignoreList = new List<string> { "IsValid", "Error", "IsInitializing", "IsInitialized" };

        protected override void OnPropertyChanged(string propertyName)
        {
            // validate this instance first (unless initializing, ignore IsValid itself)
            if(IsInitialized && !this.ignoreList.Contains(propertyName))
                this.Validate();
            // then notify of changes
            base.OnPropertyChanged(propertyName);
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
        protected internal void Validate()
        {
            Debug.Assert(this.IsInitializing == false, "WARNING! Validate should NOT be called while IsInitialized is True. Call EndInit() prior.");

            if (string.IsNullOrEmpty(this.Error)
                && this.ValidPropertiesCount == this.TotalPropertiesWithValidationCount)
                this.IsValid = true;
            else
                this.IsValid = false;
        }
    }
}