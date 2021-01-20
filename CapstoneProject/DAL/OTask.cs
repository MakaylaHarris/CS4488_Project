using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using CapstoneProject.Models;
using Task = CapstoneProject.Models.Task;
using System.Collections.ObjectModel;
using System.Windows;

/// <summary>
/// Written By: Alankar
/// Updated By: Chris Neeser, Levi Delezene
/// Purpose: To provide a common access layer between the application business logic and the database.
/// </summary>
namespace CapstoneProject.DAL {
    public class OTask {
        //SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\database\SmartPertDB.mdf;Initial Catalog=dbo;Integrated Security=True");

        SqlConnection conn;


        public OTask() {
            conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\database\SmartPertDB.mdf;Integrated Security=True");
        }


        public int Insert(Task newTask) {
            conn.Open();
            string query = "INSERT INTO Task(Name, Description, MinEstDuration, MaxEstDuration, MostLikelyEstDuration, StartDate, EndDate, ModifiedDate, StatusId, UserId, ProjectId, RootNode) VALUES " +
                "   (@name, @description, @minduration, @maxduration, @mostlikelyduration, @starteddate, @completeddate, @modifieddate, @status, @ownerid, @projectid, @rootnode)" +
                "SET @TaskId = @@IDENTITY";
            SqlCommand cmd = new SqlCommand(query, conn);

            SqlParameter outParameter = new SqlParameter("@TaskId", System.Data.SqlDbType.Int);
            outParameter.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(outParameter);

            cmd.Parameters.AddWithValue("@name", newTask.Name);
            cmd.Parameters.AddWithValue("@description", newTask.Description);
            cmd.Parameters.AddWithValue("@minduration", newTask.MinDuration);
            cmd.Parameters.AddWithValue("@maxduration", newTask.MaxDuration);
            cmd.Parameters.AddWithValue("@mostlikelyduration", newTask.MostLikelyDuration);
            cmd.Parameters.AddWithValue("@starteddate", ((object)newTask.StartedDate) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@completeddate", ((object)newTask.CompletedDate) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@modifieddate", DateTime.Now);
            cmd.Parameters.AddWithValue("@status", newTask.Status);
            cmd.Parameters.AddWithValue("@ownerid", newTask.Owner.Id);
            cmd.Parameters.AddWithValue("@projectid", newTask.ProjectId);
            cmd.Parameters.AddWithValue("@rootnode", newTask.RootNode);
            int effectedIds = cmd.ExecuteNonQuery();
            newTask.Id = (int)outParameter.Value;
            conn.Close();
            return effectedIds;
        }

        public void UpdateRootNodeFlagOnAllTasks()
        {
            conn.Open();
            string query = "UPDATE Task SET RootNode = 0 WHERE TaskId IN (SELECT DepOnTaskId FROM Dependency)";
            SqlCommand cmd = new SqlCommand(query, conn);
            int effectedIds = cmd.ExecuteNonQuery();

            query = "UPDATE Task SET RootNode = 1 WHERE TaskId IN (SELECT TaskId FROM Dependency) AND TaskId NOT IN (SELECT DepOnTaskId FROM Dependency)";
            cmd = new SqlCommand(query, conn);
            effectedIds = cmd.ExecuteNonQuery();

            conn.Close();
        }

        public int Delete(int taskId) {
            conn.Open();
            string query = "Delete from Task Where TaskId= " + taskId;
            SqlCommand cmd = new SqlCommand(query, conn);
            int effectedIds = cmd.ExecuteNonQuery();
            conn.Close();
            return effectedIds;
        }
        public int Update(Task updatedTask) {
            conn.Open();
            //TODO: Do we need to add an @taskId parameter to the command?
            string query = "update Task set Name = @name, Description=@description, MinEstDuration=@minduration, MaxEstDuration=@maxduration, MostLikelyEstDuration=@mostlikelyduration,StartDate=@starteddate, EndDate=@completeddate, ModifiedDate=@modifieddate, StatusId=@status, UserId=@ownerid, ProjectId=@projectid, RootNode=@rootnode Where TaskId=@taskid";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@taskid", updatedTask.Id);
            cmd.Parameters.AddWithValue("@name", updatedTask.Name);
            cmd.Parameters.AddWithValue("@description", updatedTask.Description);
            cmd.Parameters.AddWithValue("@minduration", updatedTask.MinDuration);
            cmd.Parameters.AddWithValue("@maxduration", updatedTask.MaxDuration);
            cmd.Parameters.AddWithValue("@mostlikelyduration", updatedTask.MostLikelyDuration);
            cmd.Parameters.AddWithValue("@starteddate", ((object)updatedTask.StartedDate) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@completeddate", ((object)updatedTask.CompletedDate) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@modifieddate", DateTime.Now);
            cmd.Parameters.AddWithValue("@status", updatedTask.Status);
            cmd.Parameters.AddWithValue("@ownerid", updatedTask.Owner.Id);
            cmd.Parameters.AddWithValue("@projectid", updatedTask.ProjectId);
            cmd.Parameters.AddWithValue("@rootnode", updatedTask.RootNode);
            int effectedIds = cmd.ExecuteNonQuery();
            conn.Close();
            return effectedIds;

        }
        public ObservableCollection<Task> Select(int ProjectId) {
            ObservableCollection<Task> taskList = new ObservableCollection<Task>();
            conn.Open();
            string query = "Select * from Task where ProjectId = @projectid";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@projectid", ProjectId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                Task task = new Task();
                task.Id = (int)reader["TaskId"];
                task.Name = (string)reader["Name"];
                task.Description = (string)reader["description"];
                task.MinDuration = (double)reader["MinEstDuration"];
                task.MaxDuration = (double)reader["MaxEstDuration"];
                task.MostLikelyDuration = (double)reader["MostLikelyEstDuration"];
                if ((reader["StartDate"]) != DBNull.Value)
                    task.StartedDate = (DateTime)reader["StartDate"];
                if ((reader["EndDate"]) != DBNull.Value)
                    task.CompletedDate = (DateTime)reader["EndDate"];
                if ((reader["ModifiedDate"]) != DBNull.Value)
                    task.ModifiedDate = (DateTime)reader["ModifiedDate"];
                task.Status = (Status)reader["StatusId"];
                OUser oUser = new OUser();
                task.Owner = oUser.SelectSingleUser((int)reader["UserId"]);
                task.ProjectId = (int)reader["ProjectId"];
                task.RootNode = (Boolean)reader["RootNode"];
                taskList.Add(task);
            }
            conn.Close();
            return taskList;
        }

        /// <summary>
        /// Gets a single task by ID
        /// -Levi Delezene
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task Get(int id) {
            Task task = new Task();
            conn.Open();
            string query = "Select * from Task where TaskId=@id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader reader = cmd.ExecuteReader();
            bool foundTask = false;
            while (reader.Read()) {
                foundTask = true;
                task.Id = (int)reader["TaskId"];
                task.Name = (string)reader["Name"];
                task.Description = (string)reader["description"];
                task.MinDuration = (double)reader["MinEstDuration"];
                task.MaxDuration = (double)reader["MaxEstDuration"];
                task.MostLikelyDuration = (double)reader["MostLikelyEstDuration"];
                if ((reader["StartDate"]) != DBNull.Value)
                    task.StartedDate = (DateTime)reader["StartDate"];
                if ((reader["EndDate"]) != DBNull.Value)
                    task.CompletedDate = (DateTime)reader["EndDate"];
                if ((reader["ModifiedDate"]) != DBNull.Value)
                    task.ModifiedDate = (DateTime)reader["ModifiedDate"];
                task.Status = (Status)reader["StatusId"];
                OUser oUser = new OUser();
                task.Owner = oUser.SelectSingleUser((int)reader["UserId"]);
                task.ProjectId = (int)reader["ProjectId"];
                task.RootNode = (Boolean)reader["RootNode"];
            }
            conn.Close();
            return foundTask ? task : null;
        }
    }
}
