using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPert.Model
{
    public class DBConnection : IDisposable
    {
        private static bool canConnect = true;
        protected SqlConnection connection;
        private SqlCommand command;

        public static bool CanConnect { get => canConnect; set => canConnect = value; }
        public bool IsConnected { get => connection.State != System.Data.ConnectionState.Closed; }

        public SqlCommand Command { get => command; }

        public DBConnection(bool connect=false)
        {
            connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            if (connect && CanConnect)
                connection.Open();
        }

        public DBConnection(string query) : this(true)
        {
            if(IsConnected)
                command = new SqlCommand(query, connection);
        }
        
        public void Close() => connection.Close();

        public bool Connect()
        {
            if (CanConnect)
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Executes the query and closes connection (no sql params)
        /// </summary>
        /// <param name="query">The sql statement</param>
        /// <returns>the ExecuteNonQuery result</returns>
        public static void ExecuteNonQuery(string query)
        {
            using (var connection = new DBConnection(query))
                connection.Command.ExecuteNonQuery();
        }

        public static object ExecuteScalar(string query)
        {
            object ret = null;
            using (var connection = new DBConnection(query))
                ret = connection.Command.ExecuteScalar();
            return ret;
        }

        public void Dispose() => connection.Dispose();
    }

    public class DBConnectionReader : DBConnection
    {
        private SqlDataReader reader;

        public SqlDataReader Reader { get => reader; }

        public DBConnectionReader(bool connect=false) : base(connect) { }
        public DBConnectionReader(string query) : base(query)
        {
            if (IsConnected)
                reader = Command.ExecuteReader();
        }

        public SqlDataReader OpenReader(string query)
        {
            if (Connect())
            {
                SqlCommand command = new SqlCommand(query, connection);
                reader = command.ExecuteReader();
                return reader;
            }
            return null;
        }

        public SqlDataReader ReadTable(string table)
        {
            return OpenReader("Select * From " + table + ";");
        }

    }

}