using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WPFCore.SqlClient
{
    public static class SQLCodeWorks
    {
        /// <summary>
        /// Eliminiert sämtliche Kommentare aus einem SQL-Befehl.
        /// </summary>
        /// <remarks>
        /// Es werden Kommentare eliminiert, welche mit "--" eingeleitet werden
        /// bzw. mit "/*" und "*/" eingefasst sind. Geschachtelte Kommentare werden
        /// nicht erkannt, d.h. ein "*/" beendet stets einen Kommentar, unabhängig
        /// davon, wieviele "/*" zuvor vorhanden waren.
        /// </remarks>
        /// <param name="sqlCode">Der SQL-Befehl.</param>
        /// <returns>Den SQL-Befehl ohne Kommentare</returns>
        public static string StripComments(string sqlCode)
        {
            string s = "";
            bool isMultiLineCommented = false;
            bool isSingleLineCommented = false;

            int p = 0;

            while (p < sqlCode.Length - 1) // nur bis zum Vorletzten Zeichen scannen
            {
                // nach */ suchen
                if (isMultiLineCommented)
                {
                    // prüfen, ob hier der Kommentar endet
                    if (sqlCode.Substring(p, 2) == "*/")
                    {
                        isMultiLineCommented = false;
                        p++; // das 2. Zeichen ("/") überlesen
                    }
                }
                else 
                    if (isSingleLineCommented)
                    {
                        // nach dem Zeilenende suchen
                        if (sqlCode[p] == '\r' || sqlCode[p] == '\n')
                        {
                            isSingleLineCommented = false;
                            s += sqlCode[p];
                        }
                    }
                    
                    else
                    {
                        // aktuelles Zeichen ist nicht innerhalb eines Kommentars

                        // prüfen, ob hier ein Kommentar beginnt
                        if (sqlCode.Substring(p, 2) == "/*")
                        {
                            isMultiLineCommented = true;
                            p++; // das 2. Zeichen ("*") überlesen
                        }
                        else if (sqlCode.Substring(p, 2) == "--")
                        {
                            isSingleLineCommented = true;
                            p++; // das 2. Zeichen ("-") überlesen
                        }
                        else // sonst das Zeichen übernehmen
                            s += sqlCode[p];
                    }

                p++;
            }

            // Das allerletzte Zeichen auch noch mitnehmen
            if (!isMultiLineCommented && !isSingleLineCommented)
                s += sqlCode[p];

            return s;
        }

        /// <summary>
        /// Liefert eine Liste mit den Parametern, die in einem
        /// SQL-Befehl verwendet werden.
        /// </summary>
        /// <remarks>
        /// Es werden nur Parameter in der Oracle-Notation erkannt,
        /// d.h. nur solche, welche mit einem Doppelpunkt beginnen.
        /// Parameternamen werden in Grossbuchstaben konvertiert.
        /// </remarks>
        /// <param name="sqlCode">Der SQL-befehl.</param>
        /// <returns>Eine Liste der Parameternamen</returns>
        public static List<string> GetOracleParameterNames(string sqlCode)
        {
            var parameters = new List<string>();
            // Kommentare wegwerfen, sonst werden auskommentierte Parameter erkannt
            var sqlNoComment = StripComments(sqlCode);

            var regex = new Regex(@":\w+");
            foreach (var matchedParameter in
                           from Match match 
                             in regex.Matches(sqlNoComment) 
                          where !parameters.Contains(match.Value.ToUpper()) // schon erkannte ignorieren
                         select match)
            {
                parameters.Add(matchedParameter.Value.ToUpper());
            }

            return parameters;
        }
    }
}