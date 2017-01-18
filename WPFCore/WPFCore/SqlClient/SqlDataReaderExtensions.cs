using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using WPFCore.Helper;

namespace WPFCore.SqlClient
{
    public static class SqlDataReaderExtensions
    {

        public static bool ContainsColumn(this SqlDataReader reader, string columnName)
        {
            for(int idx = 0; idx<reader.FieldCount; idx++)
                if (reader.GetName(idx) == columnName)
                    return true;

            return false;
        }

        public static object GetObjectNullable(this SqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return string.Empty;
            else
                return reader[index];
        }

        #region GetString extensions
        public static string GetString(this SqlDataReader reader, string columnName)
        {
            var index = reader.GetOrdinal(columnName);
            return reader.GetString(index);
        }

        public static string GetStringNullable(this SqlDataReader reader, int index)
        {
            return GetStringNullable(reader, index, string.Empty);
        }

        public static string GetStringNullable(this SqlDataReader reader, string columnName)
        {
            var index = reader.GetOrdinal(columnName);
            return GetStringNullable(reader, index, string.Empty);
        }

        public static string GetStringNullable(this SqlDataReader reader, int index, string defaultText)
        {
            if (reader.IsDBNull(index))
                return defaultText;
            else
                return reader.GetString(index);
        }

        public static string GetStringNullable(this SqlDataReader reader, string columnName, string defaultText)
        {
            var index = reader.GetOrdinal(columnName);
            return GetStringNullable(reader, index, defaultText);
        }
        #endregion GetString extensions

        #region GetInt32 extensions

        public static Int32 GetInt32(this SqlDataReader reader, string columnName)
        {
            var index = reader.GetOrdinal(columnName);

            if (reader.GetDataTypeName(index) == "smallint")
                return (int) reader.GetInt16(index);
            else
                return reader.GetInt32(index);
        }

        public static Int32? GetInt32Nullable(this SqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return (int?)null;
            else
            {
                if (reader.GetDataTypeName(index) == "smallint")
                    return (int)reader.GetInt16(index);
                else
                    return reader.GetInt32(index);
            }
        }

        public static Int32? GetInt32Nullable(this SqlDataReader reader, string columnName)
        {
            var index = reader.GetOrdinal(columnName);
            return GetInt32Nullable(reader, index);
        }

        public static Int32 GetInt32(this SqlDataReader reader, int index, Int32 defaultValue)
        {
            if (reader.IsDBNull(index))
                return defaultValue;
            else
            {
                if (reader.GetDataTypeName(index) == "smallint")
                    return (int)reader.GetInt16(index);
                else
                    return reader.GetInt32(index);
            }
        }

        public static Int32 GetInt32(this SqlDataReader reader, string columnName, Int32 defaultValue)
        {
            var index = reader.GetOrdinal(columnName);
            return GetInt32(reader, index, defaultValue);
        }
        #endregion GetInt32 extensions

        #region GetDouble extensions

        public static double? GetDouble(this SqlDataReader reader, string columnName)
        {
            var index = reader.GetOrdinal(columnName);
            return reader.GetDouble(index);
        }

        public static double? GetDoubleNullable(this SqlDataReader reader, int index)
        {
            return GetDoubleNullable(reader, index, null);
        }

        public static double? GetDoubleNullable(this SqlDataReader reader, string columnName)
        {
            var index = reader.GetOrdinal(columnName);
            return GetDoubleNullable(reader, index);
        }

        public static double? GetDoubleNullable(this SqlDataReader reader, int index, double? defaultValue)
        {
            try
            {
                if (reader.IsDBNull(index))
                    return defaultValue;

                return Convert.ToDouble(reader[index]);
            }
            catch (Exception e)
            {
                e.AddData("index", index);
                e.AddData("defaultValue", defaultValue);
                e.AddData("column value", reader[index]);

                throw;
            }
        }

        public static double? GetDoubleNullable(this SqlDataReader reader, string columnName, double defaultValue)
        {
            var index = reader.GetOrdinal(columnName);
            return GetDoubleNullable(reader, index, defaultValue);
        }
        #endregion GetDouble extensions

        #region GetSmallInt extensions
        public static int? GetSmallIntNullable(this SqlDataReader reader, int index)
        {
            try
            {
                if (reader.IsDBNull(index))
                    return null;
                return reader.GetInt16(index);
            }
            catch (Exception e)
            {
                e.AddData("index", index);
                e.AddData("column value", reader[index]);

                throw;
            }
        }
        #endregion GetSmallInt extensions

        #region GetDateTime extensions

        public static DateTime GetDateTime(this SqlDataReader reader, string columnName)
        {
            var index = reader.GetOrdinal(columnName);
            return reader.GetDateTime(index);
        }

        public static DateTime? GetDateTimeNullable(this SqlDataReader reader, int index)
        {
            return GetDateTimeNullable(reader, index, null);
        }

        public static DateTime? GetDateTimeNullable(this SqlDataReader reader, string columnName)
        {
            var index = reader.GetOrdinal(columnName);
            return reader.GetDateTimeNullable(index);
        }

        public static DateTime? GetDateTimeNullable(this SqlDataReader reader, int index, DateTime? defaultValue)
        {
            if (reader.IsDBNull(index))
                return defaultValue;
            return reader.GetDateTime(index);
        }

        public static DateTime? GetDateTimeNullable(this SqlDataReader reader, string columnName, DateTime? defaultValue)
        {
            var index = reader.GetOrdinal(columnName);
            return GetDateTimeNullable(reader, index, defaultValue);
        }

        #endregion GetDateTime extensions

        #region GetBoolean extensions

        public static bool? GetBooleanNullable(this SqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            else
                return reader.GetBoolean(index);
        }

        #endregion GetBoolean extensions

        [Conditional("DEBUG")]
        public static void DumpValues(this SqlDataReader reader)
        {
            for (int idx = 0; idx < reader.VisibleFieldCount; idx++)
                Debug.Write(string.Format("{0}\t", reader.IsDBNull(idx) ? "<null>" : reader[idx]));

            Debug.WriteLine("");
        }

    }
}
