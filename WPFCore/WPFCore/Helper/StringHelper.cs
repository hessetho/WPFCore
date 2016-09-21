using System.Text;
using System.Text.RegularExpressions;

namespace WPFCore.Helper
{
    public static class StringHelper
    {
        public static bool ValidCharacters(this string text, string validCharactersRegex)
        {
            var checkRegex = new Regex(validCharactersRegex);
            return checkRegex.IsMatch(text);
        }

        public static string Replicate(string replicationString, int num)
        {
            var result = new StringBuilder(num * replicationString.Length);
            for (int i = 0; i < num; i++)
                result.Append(replicationString);

            return result.ToString();
        }
    }
}
