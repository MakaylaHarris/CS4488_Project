using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERT.DAL
{
    abstract class IQuery
    {

        private SqlConnection connection;

        protected SqlCommand OpenConnection(string query)
        {
            connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            connection.Open();
            return new SqlCommand(query, connection);
        }

        protected void CloseConnection()
        {
            connection.Close();
        }

        public abstract void Insert();
        public abstract void Delete();
        public abstract void Update();
        public abstract void Select();
    }
}
