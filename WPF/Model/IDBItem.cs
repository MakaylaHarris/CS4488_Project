using System.Data.SqlClient;

namespace SmartPert.Model
{
    /// <summary>
    /// Objects inherit from IDBItem if they are updated, inserted, and deleted from the sql database
    /// Created 1/28/2021 by Robert Nelson
    /// </summary>
    public abstract class IDBItem
    {

        private SqlConnection connection;

        #region Protected Methods
        protected SqlCommand OpenConnection(string query)
        {
            connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            connection.Open();
            return new SqlCommand(query, connection);
        }

        protected int ExecuteSql(string query)
        {
            SqlCommand cmd = OpenConnection(query);
            int result = cmd.ExecuteNonQuery();
            connection.Close();
            return result;
        }

        protected void CloseConnection()
        {
            connection.Close();
        }
        #endregion

        #region Abstract Methods
        abstract protected void Update();
        abstract protected int Insert();
        abstract public void Delete();

        #endregion
    }
}
