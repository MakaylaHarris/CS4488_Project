using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SmartPert.Model
{
    /// <summary>
    /// A PERT/Gantt project with tasks
    /// Created 1/28/2021 by Robert Nelson
    /// </summary>
    public class Project : TimedItem
    {
        private List<Task> tasks;

        public List<Task> Tasks { get => tasks; }

        #region Project
        public Project(string name, DateTime start, DateTime? end, string description = "", int id = -1, bool insert = true, bool track=true, IItemObserver observer=null) 
            : base(name, start, end, description, id, observer)
        {
            tasks = new List<Task>();
            PostInit(insert, track);
        }
        #endregion

        #region Task Methods
        public void AddTask(Task t)
        {
            if(!tasks.Contains(t))
                tasks.Add(t);
        }

        public void RemoveTask(Task t) => tasks.Remove(t);
        #endregion

        #region Workers
        /// <summary>
        /// Adds the worker to the project
        /// Implemented 2/9/2021 by Robert Nelson
        /// </summary>
        /// <param name="worker">User on project to add</param>
        /// <returns>true if added</returns>
        public override bool AddWorker(User worker)
        {
            bool added = false;
            if(!workers.Contains(worker))
            {
                try
                {
                    SqlCommand command = OpenConnection("INSERT INTO UserProject (UserName, ProjectId) VALUES(@username, @projectId);");
                    command.Parameters.AddWithValue("@username", worker.Username);
                    command.Parameters.AddWithValue("@projectId", this.Id);
                    command.ExecuteNonQuery();
                    CloseConnection();
                    workers.Add(worker);
                    added = true;
                    Model.Instance.OnModelUpdate(this);
                }
                catch (SqlException) { }
            }
            return added;
        }

        /// <summary>
        /// Removes the worker from the project (todo, remove from all tasks on project?)
        /// Implemented 2/9/2021 by Robert Nelson
        /// </summary>
        /// <param name="worker">User to remove</param>
        /// <returns>true if removed successfully</returns>
        public override bool RemoveWorker(User worker)
        {
            bool removed = false;
            if(workers.Contains(worker))
            {
                try
                {
                    SqlCommand command = OpenConnection("DELETE FROM UserProject WHERE ProjectId = @projectId AND UserName = @username);");
                    command.Parameters.AddWithValue("@projectId", Id);
                    command.Parameters.AddWithValue("@username", worker.Username);
                    command.ExecuteNonQuery();
                    CloseConnection();
                    workers.Remove(worker);
                    removed = true;
                    Model.Instance.OnModelUpdate(this);
                } catch (SqlException) { }
            }
            return removed;
        }

        #endregion

        #region Database
        /// <summary>
        /// Deletes the project from the database
        /// </summary>
        protected override void PerformDelete()
        {
            SqlCommand command = OpenConnection("EXEC ProjectDelete @ProjectID");
            command.Parameters.AddWithValue("@ProjectID", Id);
            command.ExecuteNonQuery();
            CloseConnection();
        }

        /// <summary>
        /// Inserts the project into the database
        /// </summary>
        /// <throws>Exception if the call was unsuccessful</throws>
        /// <returns>the inserted id on success</returns>
        protected override int PerformInsert()
        {
            string query = "EXEC CreateProject @ProjectName, @StartDate, @EndDate, @Description, @Creator, @CreationDate OUT, @Result OUT, @ResultId OUT";
            SqlCommand command = OpenConnection(query);
            command.Parameters.AddWithValue("@ProjectName", Name);
            command.Parameters.AddWithValue("@StartDate", StartDate);
            if (EndDate != null)
                command.Parameters.AddWithValue("@EndDate", EndDate);
            else
                command.Parameters.AddWithValue("@EndDate", DBNull.Value);
            command.Parameters.AddWithValue("@Description", Description);
            creator = Model.Instance.GetCurrentUser();
            if (creator != null)
                command.Parameters.AddWithValue("@Creator", creator.Username);
            else
                command.Parameters.AddWithValue("@Creator", DBNull.Value);
            var createDate = command.Parameters.Add("@CreationDate", System.Data.SqlDbType.DateTime);
            createDate.Direction = System.Data.ParameterDirection.Output;
            var insertedId = command.Parameters.Add("@ResultId", System.Data.SqlDbType.Int);
            insertedId.Direction = System.Data.ParameterDirection.Output;
            var result = command.Parameters.Add("@Result", System.Data.SqlDbType.Bit);
            result.Direction = System.Data.ParameterDirection.Output;
            command.ExecuteNonQuery(); 
            if (!(bool)result.Value)
                throw new InsertionError("Failed to insert project " + Name + " into database!");
            creationDate = (DateTime) createDate.Value;
            id = (int) insertedId.Value;
            return id;
        }

        /// <summary>
        /// Updates the project properties in the database
        /// </summary>
        protected override void PerformUpdate()
        {
            SqlCommand command = OpenConnection("UPDATE Project SET Name=@Name, Description=@Description, StartDate=@StartDate, EndDate=@EndDate WHERE ProjectId=@Id;");
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@Description", Description);
            command.Parameters.AddWithValue("@StartDate", StartDate);
            if (EndDate == null)
                command.Parameters.AddWithValue("@EndDate", DBNull.Value);
            else
                command.Parameters.AddWithValue("@EndDate", EndDate);
            command.Parameters.AddWithValue("@Id", Id);
            command.ExecuteNonQuery();
            CloseConnection();
        }

        static public Project Parse(SqlDataReader reader, Dictionary<string, User> users)
        {
            string creator = DBFunctions.StringCast(reader, "Creator");
            User user = users != null && creator != "" ? users[creator] : null;
            Project p = new Project(
                (string)reader["Name"],
                (DateTime)reader["StartDate"],
                DBFunctions.DateCast(reader, "EndDate"),
                DBFunctions.StringCast(reader, "Description"),
                (int)reader["ProjectId"], insert: false);
            p.creator = user;
            p.creationDate = (DateTime)DBFunctions.DateCast(reader, "CreationDate");
            return p;
        }

        /// <summary>
        /// Parses the project data
        /// </summary>
        /// <param name="reader">Sql data reader</param>
        /// <returns>true if updated</returns>
        public override bool PerformParse(SqlDataReader reader)
        {
            string name = (string)reader["Name"];
            DateTime start = (DateTime)reader["StartDate"];
            DateTime? end = DBFunctions.DateCast(reader, "EndDate");
            string desc = DBFunctions.StringCast(reader, "Description");
            if(name != Name || start != StartDate || end != EndDate || desc != Description)
            {
                Name = name;
                StartDate = start;
                EndDate = end;
                Description = desc;
                return true;
            }
            return false;
        }

        #endregion
    }
}
