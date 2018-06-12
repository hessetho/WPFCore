using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using WPFCore.Helper;

namespace WPFCore.MySql
{
    /// <summary>
    ///     Extension methods for various elements of the sql client namespace
    /// </summary>
    public static class MySqlClientExtensions
    {
        /// <summary>
        ///     Adds a nullable value to the end of the <see cref="System.Data.SqlClient.SqlParameterCollection" />
        /// </summary>
        /// <param name="parameters">The <see cref="System.Data.SqlClient.SqlParameterCollection" /> to add the value</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="value">The nullable value to be added</param>
        public static void AddWithNullableValue(this MySqlParameterCollection parameters, string parameterName,
            object value)
        {
            if (value == null || (value is string && (string.IsNullOrEmpty((string)value))))
                parameters.AddWithValue(parameterName, DBNull.Value);
            else
                parameters.AddWithValue(parameterName, value);
        }

        public static MySqlCommand[] GetCommandCollection<T>(this T tableAdapter)
            where T : Component
        {
            var c =
                typeof(T).GetProperty("CommandCollection",
                    BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance)
                    .GetValue(tableAdapter, null) as MySqlCommand[];

            if (c == null)
                throw new ArgumentException("The specified object is not a TableAdapter!");

            return c;
        }

        public static void LogSelectCommand(this MySqlCommand[] commandCollection)
        {
            foreach (var cmd in commandCollection.Where(c => c.CommandText.TrimStart().ToUpper().StartsWith("SELECT")))
                Constants.SqlTraceSource.TraceDebug(cmd.CommandText);
        }
    }
}
