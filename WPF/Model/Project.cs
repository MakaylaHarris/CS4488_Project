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
        public Project(string name, DateTime start, DateTime? end, string description = "", 
            User creator = null, DateTime? creationTime = null, int id = -1) 
            : base(name, start, end, description, creator, creationTime, id)
        {
            tasks = new List<Task>();
            if (id == -1)
                this.id = Insert();
        }
        public Project(Project project, int id = -1) : base(project, id)
        {
            tasks = new List<Task>();
            foreach (Task t in project.tasks)
                tasks.Add(t);
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
        /// <param name="updateDB">Also adds the user to the database</param>
        /// <returns>true if added</returns>
        public override bool AddWorker(User worker, bool updateDB=true)
        {
            bool added = false;
            if(!workers.Contains(worker))
            {
                try
                {
                    if (updateDB)
                    {
                        SqlCommand command = OpenConnection("INSERT INTO UserProject (UserName, ProjectId) VALUES(@username, @projectId);");
                        command.Parameters.AddWithValue("@username", worker.Username);
                        command.Parameters.AddWithValue("@projectId", this.Id);
                        command.ExecuteNonQuery();
                        CloseConnection();
                    }
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
        /// <param name="updateDB">Update the database</param>
        /// <returns>true if removed successfully</returns>
        public override bool RemoveWorker(User worker, bool updateDB=true)
        {
            bool removed = false;
            if(workers.Contains(worker))
            {
                try
                {
                    if (updateDB)
                    {
                        SqlCommand command = OpenConnection("DELETE FROM UserProject WHERE ProjectId = @projectId AND UserName = @username);");
                        command.Parameters.AddWithValue("@projectId", Id);
                        command.Parameters.AddWithValue("@username", worker.Username);
                        command.ExecuteNonQuery();
                        CloseConnection();
                    }
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
        public override void Delete()
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
        protected override int Insert()
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
                command.Parameters.AddWithValue("@Creator", creator);
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
                throw new Exception("Failed to insert project " + Name + " into database!");
            creationDate = (DateTime) createDate.Value;
            return (int) insertedId.Value;
        }

        protected override void Update()
        {
            // todo: fix this hot mess
            ExecuteSql("update Project set Name = '" + Name
                + "', Description = '" + Description +
                "', StartDate='" + StartDate +
                "', EndDate='" + EndDate +
                "'Where ProjectId=" + Id + ";");
            Model.Instance.OnModelUpdate(this);
        }

        static public Project Parse(SqlDataReader reader, Dictionary<string, User> users)
        {
            string creator = DBFunctions.StringCast(reader, "Creator");
            User user = users != null && creator != "" ? users[creator] : null;
            return new Project(
                (string)reader["Name"],
                (DateTime)reader["StartDate"],
                DBFunctions.DateCast(reader, "EndDate"),
                DBFunctions.StringCast(reader, "Description"),
                user,
                DBFunctions.DateCast(reader, "CreationDate"),
                (int)reader["ProjectId"]); ;
        }

        #endregion
    }
}
