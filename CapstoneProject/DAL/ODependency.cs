using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using CapstoneProject.Models;
using System.Windows;

namespace CapstoneProject.DAL
{
    public class ODependency
    {
        SqlConnection conn;

        public ODependency()
        {
            conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\database\SmartPertDB.mdf;Integrated Security=True");
        }

        public int Insert(Dependency newDependency)
        {
            conn.Open();
            string query = $"insert into Dependency(TaskId, DepOnTaskId) values(@taskId,@depontaskid)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@taskId", newDependency.TaskId);
            cmd.Parameters.AddWithValue("@depontaskid", newDependency.DepOnTaskId);
            int effectedIds = cmd.ExecuteNonQuery();
            conn.Close();
            return effectedIds;
            
        }
        public int Delete(int id)
        {
            conn.Open();
            string query = "Delete from Dependency Where DepOnTaskId= " + id;
            SqlCommand cmd = new SqlCommand(query, conn);
            int effectedIds = cmd.ExecuteNonQuery();
            conn.Close();
            return effectedIds;
        }

        public int DeleteAllForDepOnTask(int taskId)
        {
            conn.Open();
            string query = "Delete from Dependency Where DepOnTaskId=@taskid";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@taskid", taskId);
            int effectedIds = cmd.ExecuteNonQuery();
            conn.Close();
            return effectedIds;
        }

        public int DeleteAllTaskDependencies(int taskId)
        {
            conn.Open();
            string query = "Delete from Dependency Where TaskId=@taskid or DepOnTaskId=@taskid";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@taskid", taskId);
            int effectedIds = cmd.ExecuteNonQuery();
            conn.Close();
            return effectedIds;
        }

        public int Update(Dependency updatedDependency)
        {

            conn.Open();
            string query = "update Dependency set TaskId='" + updatedDependency.TaskId + "' Where Id=" + updatedDependency.DepOnTaskId;
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@taskId", updatedDependency.TaskId);
            int effectedIds = cmd.ExecuteNonQuery();
            conn.Close();
            return effectedIds;

        }

        public List<Dependency> Select(int TaskId)
        {
            List<Dependency> dependencyList = new List<Dependency>();
            conn.Open();
            string query = "Select * from Dependency where TaskId = @taskid";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@taskid", TaskId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Dependency dep = new Dependency();
                dep.DepOnTaskId = (int)reader["DepOnTaskId"];
                dep.TaskId = (int)reader["TaskId"];
                dependencyList.Add(dep);
            }
            conn.Close();
            return dependencyList;
        }

        public void SelectAll()
        {
            List<Dependency> dependencyList = new List<Dependency>();
            conn.Open();
            string query = "Select * from Dependency";
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Dependency dep = new Dependency();
                dep.DepOnTaskId = (int)reader["DepOnTaskId"];
                dep.TaskId = (int)reader["TaskId"];
                dependencyList.Add(dep);
            }
            conn.Close();
            foreach(Dependency d in dependencyList) { MessageBox.Show(d.DepOnTaskId + " " + d.TaskId); }
        }
    }
}
