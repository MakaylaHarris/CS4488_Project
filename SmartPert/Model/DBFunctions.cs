using System;
using System.Data.SqlClient;

namespace Pert.Model
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
    }
}