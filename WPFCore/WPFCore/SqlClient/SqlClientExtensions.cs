using System;
using System.Data.SqlClient;
using System.Linq;
using WPFCore.Helper;

namespace WPFCore.SqlClient
{
    /// <summary>
    /// Extension methods for various elements of the sql client namespace
    /// </summary>
    public static class SqlClientExtensions
    {
        /// <summary>
        /// Adds a nullable value to the end of the <see cref="System.Data.SqlClient.SqlParameterCollection"/>
        /// </summary>
        /// <param name="parameters">The <see cref="System.Data.SqlClient.SqlParameterCollection"/> to add the value</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="value">The nullable value to be added</param>
        public static void AddWithNullableValue(this SqlParameterCollection parameters, string parameterName,
            object value)
        {
            if (value == null || (value is string && (string.IsNullOrEmpty((string) value))))
                parameters.AddWithValue(parameterName, DBNull.Value);
            else
                parameters.AddWithValue(parameterName, value);
        }

        public static System.Data.SqlClient.SqlCommand[] GetCommandCollection<T>(this T TableAdapter)
        where T : global::System.ComponentModel.Component
        {
            var c = typeof(T).GetProperty("CommandCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance).GetValue(TableAdapter, null) as System.Data.SqlClient.SqlCommand[];

            if(c==null)
                throw new ArgumentException("The specified object is not a TableAdapter!");

            return c;
        }

        public static void LogSelectCommand(this SqlCommand[] commandCollection)
        {
            foreach (var cmd in commandCollection.Where(c => c.CommandText.TrimStart().ToUpper().StartsWith("SELECT")))
                Constants.SqlTraceSource.TraceDebug(cmd.CommandText);
        }

    }
}
