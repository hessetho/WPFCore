using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WPFCore.Helper
{
    public static class StringHelper
    {
        /// <summary>
        ///     Zerlegt einen String unter Berücksichtigung von Quotes (") als Text-Begrenzer
        /// </summary>
        /// <param name="input">Die zu zerlegende Zeile</param>
        /// <param name="delimiter">Das Trennzeichen</param>
        /// <returns>Die Einzelstrings</returns>
        public static string[] SplitQuoted(this string input, char delimiter)
        {
            int lastPos = 0;
            int currentPos = 0;
            var items = new List<string>();
            bool inQuotes = false;

            while (currentPos < input.Length)
            {
                if (input[currentPos] == '\"')
                    inQuotes = !inQuotes;
                else if (input[currentPos] == delimiter && !inQuotes)
                {
                    items.Add(input.Substring(lastPos, currentPos - lastPos));
                    lastPos = currentPos + 1;
                }
                currentPos++;
            }
            items.Add(input.Substring(lastPos, currentPos - lastPos));

            return items.ToArray();
        }

        /// <summary>
        /// Wiederholt einen <c>string</c> eine beliebige Anzahl mal hintereinander
        /// </summary>
        /// <param name="replicationString">Der zu wiederholende Strig</param>
        /// <param name="num">Anzahl der Wiederholungen</param>
        /// <returns></returns>
        public static string Replicate(string replicationString, int num)
        {
            if (string.IsNullOrEmpty(replicationString))
                throw new ArgumentException("empty strings are not allowed", "replicationString");
            if (num < 0)
                throw new ArgumentException("only positive values allowed", "num");

            var result = new StringBuilder(num * replicationString.Length);
            for (var i = 0; i < num; i++)
                result.Append(replicationString);

            return result.ToString();
        }

        /// <summary>
        /// Wandelt die angegebene Zeichenfolge in Schreibung mit großen Anfangsbuchstaben um
        /// </summary>
        /// <param name="strText">Umzuwandelnde Zeichenfolge</param>
        /// <returns></returns>
        public static string InitialCaps(this string strText)
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;
            return ci.TextInfo.ToTitleCase(strText.ToLower());
        }

        /// <summary>
        /// Konvertiert einen String in ein Byte-Array (ASCII-Encoding)
        /// </summary>
        /// <param name="input">Der String</param>
        /// <returns>Das Byte-Array</returns>
        public static byte[] ToByteArray(this string input)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(input);
        }

        /// <summary>
        /// Konvertiert einen Byte-Array in einen String (ASCII-Encoding)
        /// </summary>
        /// <param name="arr">Das Byte-Array.</param>
        /// <returns>Der String.</returns>
        public static string FromByteArray(byte[] arr)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetString(arr);
        }

        /// <summary>
        ///     Prüft ob ein <see cref="string" /> einen Zahlenwert darstellt, welcher in <see cref="double" /> konvertiret werden kann.
        ///     Die Prüfung erfolgt mittels <see cref="Regex" />.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsDouble(string val)
        {
            string grpSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator;
            string decSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (grpSep == ".") grpSep = @"\.";
            if (decSep == ".") decSep = @"\.";

            var re = new Regex(@"^[0-9]{1,3}(" + grpSep + "[0-9]{3})*" + decSep + "[0-9]+$");

            return re.IsMatch(val);
        }

        public static bool ValidCharacters(this string text, string validCharactersRegex)
        {
            var checkRegex = new Regex(validCharactersRegex);
            return checkRegex.IsMatch(text);
        }
    }
}
