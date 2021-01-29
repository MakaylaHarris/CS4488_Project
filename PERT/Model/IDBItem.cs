using System.Data.SqlClient;

namespace PERT.Model
{
    public abstract class IDBItem
    {
        private SqlConnection connection;
        protected SqlCommand OpenConnection(string query)
        {
            connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            connection.Open();
            return new SqlCommand(query, connection);
        }

        protected int ExecuteSql(string query)
        {
            SqlCommand cmd = OpenConnection(query);
            int result = (int)cmd.ExecuteNonQuery();
            connection.Close();
            return result;
        }

        protected void CloseConnection()
        {
            connection.Close();
        }

        abstract protected void Update();
        abstract protected int Insert();
        abstract protected void Delete();

    }
}
