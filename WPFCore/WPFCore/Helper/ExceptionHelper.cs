using System;
using System.Data;
using System.Data.Common;

namespace WPFCore.Helper
{
    public static class ExceptionHelper
    {
        /// <summary>
        ///     Add's a command's parameters to an exceptions data dictionary
        /// </summary>
        /// <param name="e"></param>
        /// <param name="cmd"></param>
        public static void AddCommandParameters(this Exception e, DbCommand cmd)
        {
            e.AddData("sql", cmd.CommandText);
            e.AddData("connection", cmd.Connection.ConnectionString);
            foreach (IDbDataParameter p in cmd.Parameters)
                e.AddData(p.ParameterName, p.Value);
        }

        /// <summary>
        ///     Add's some data to an exception's data dictionary
        /// </summary>
        /// <param name="e"></param>
        /// <param name="key"></param>
        /// <param name="item"></param>
        public static void AddData(this Exception e, string key, object item)
        {
            e.Data.Add(key, item);
        }
    }
}