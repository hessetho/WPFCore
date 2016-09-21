using System;

namespace WPFCore.XAML.Controls
{
    public static class InputTextBox
    {
        public static string Show(string messageText)
        {
            return Show(messageText, string.Empty, string.Empty);
        }

        public static string Show(string messageText, string defaultText)
        {
            return Show(messageText, defaultText, string.Empty);
        }

        public static string Show(string messageText, string defaultText, string title)
        {
            var inputTextWin = new XAML.InputTextBoxWindow
            {
                MessageText = messageText,
                InputText = defaultText,
            };

            if (inputTextWin.ShowDialog() == true)
                return inputTextWin.InputText;

            return string.Empty;
        }

        #region with validation function
        public static string ShowWithValidation(string messageText, Func<string, string> inputValidation)
        {
            return ShowWithValidation(messageText, string.Empty, string.Empty, inputValidation);
        }

        public static string ShowWithValidation(string messageText, string defaultText, Func<string, string> inputValidation)
        {
            return ShowWithValidation(messageText, defaultText, string.Empty, inputValidation);
        }

        public static string ShowWithValidation(string messageText, string defaultText, string title, Func<string, string> inputValidation)
        {
            var inputTextWin = new XAML.InputTextBoxWindow
            {
                MessageText = messageText,
                InputText = defaultText,
            };
            inputTextWin.SetValidationFunction(inputValidation);

            if (inputTextWin.ShowDialog() == true)
                return inputTextWin.InputText;

            return string.Empty;
        }
        #endregion with validation function
    
    }
}
