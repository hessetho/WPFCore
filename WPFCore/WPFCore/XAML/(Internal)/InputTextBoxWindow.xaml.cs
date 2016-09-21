using System;
using System.ComponentModel;
using System.Windows;

namespace WPFCore.XAML
{
    /// <summary>
    /// Interaction logic for InputTextBox.xaml
    /// </summary>
    internal partial class InputTextBoxWindow : Window, IDataErrorInfo
    {
        public static DependencyProperty MessageTextProperty =
                        DependencyProperty.Register("MessageText", typeof(string), typeof(InputTextBoxWindow));

        public static DependencyProperty InputTextProperty =
                        DependencyProperty.Register("InputText", typeof(string), typeof(InputTextBoxWindow));

        private Func<string, string> getValidationResult;

        public InputTextBoxWindow()
        {
            InitializeComponent();
        }

        public string MessageText
        {
            get { return ((string)(this.GetValue(InputTextBoxWindow.MessageTextProperty))); }
            set { this.SetValue(InputTextBoxWindow.MessageTextProperty, value); }
        }

        public string InputText
        {
            get { return ((string)(this.GetValue(InputTextBoxWindow.InputTextProperty))); }
            set { this.SetValue(InputTextBoxWindow.InputTextProperty, value); }
        }

        internal void SetValidationFunction(Func<string, string> getValidationResult)
        {
            this.getValidationResult = getValidationResult;
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OkayClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        #region IDataErrorInfo
        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public string Error
        {
            get 
            {
                if (this.getValidationResult != null)
                {
                    return this.getValidationResult(this.InputText);
                }
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string this[string columnName]
        {
            get
            {
                var result = string.Empty;

                if (this.getValidationResult != null)
                {
                    result = this.getValidationResult(this.InputText);
                }

                if(this.IsLoaded)
                    this.OkayButton.IsEnabled = string.IsNullOrEmpty(result);

                return result;
            }
        }

        #endregion IDataErrorInfo
    }
}
