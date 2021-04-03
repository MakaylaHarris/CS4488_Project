using System;
using System.Data.SqlClient;

namespace SmartPert.Model
{
    /// <summary>
    /// Functions to safely cast null values in the database
    /// Created 1/31/2021 by Robert Nelson
    /// </summary>
    class DBFunctions
    {

        public static DateTime? DateCast(SqlDataReader reader, string column)
        {
            int index = reader.GetOrdinal(column);
            if (reader.IsDBNull(index))
                return null;
            return reader.GetDateTime(index);
        }

        public static string StringCast(SqlDataReader reader, string column)
        {
            int index = reader.GetOrdinal(column);
            if (reader.IsDBNull(index))
                return "";
            return reader.GetString(index);
        }

        /// <summary>
        /// Reads the column value and casts to int
        /// </summary>
        /// <param name="reader">Open SqlDataReader</param>
        /// <param name="column">column name</param>
        /// <param name="null_val">what to return if null</param>
        /// <returns>int</returns>
        public static int IntCast(SqlDataReader reader, string column, int null_val=0)
        {
            int index = reader.GetOrdinal(column);
            if (reader.IsDBNull(index))
                return null_val;
            return reader.GetInt32(index);
        }
    }
}