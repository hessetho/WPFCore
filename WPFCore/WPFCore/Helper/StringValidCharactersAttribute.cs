using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WPFCore.Helper
{
    public class StringValidCharactersAttribute : ValidationAttribute
    {
        private Regex checkRegex;

        /// <summary>
        /// Checks for valid characters in a string
        /// </summary>
        /// <param name="validCharactersRegex">A regular expression used to check the whole string</param>
        public StringValidCharactersAttribute(string validCharactersRegex)
        {
            this.checkRegex = new Regex(validCharactersRegex);
        }

        public override bool IsValid(object value)
        {
            if (value is string)
            {
                var text = (string)value;
                return this.checkRegex.IsMatch(text);
            }
            return false;
        }
    }
}
