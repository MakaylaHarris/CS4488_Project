using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SmartPert.Model
{
    /// <summary>
    /// A Task is a single item on the Pert chart that has a most likely duration, minimum duration, and maximum duration.
    /// Dependencies are Tasks that cannot be started until this one is finished.
    /// Created 1/29/2021 by Robert Nelson
    /// </summary>
    public class Task : TimedItem
    {
        private readonly Project project;
        private int mostLikelyDuration;
        private int maxDuration;
        private int minDuration;
        private List<Task> dependencies;

        #region Properties
        /// <summary>
        /// Gets the project the task is on
        /// Added 2/13/2021 by Robert Nelson
        /// </summary>
        public Project Proj
        {
            get => project;
        }

        public int LikelyDuration
        {
            get => mostLikelyDuration;
            set
            {
                if(mostLikelyDuration != value)
                {
                    if (value < minDuration)
                        minDuration = value;
                    else if (value > MaxDuration)
                        maxDuration = value;
                    mostLikelyDuration = value;
                    Update();
                }
            }
        }
        public int MaxDuration { get => maxDuration; 
            set {
                if(maxDuration != value)
                {
                    if (value < mostLikelyDuration)
                        LikelyDuration = value;
                    maxDuration = value;
                    Update();
                }
            } 
        }

        public int MinDuration
        {
            get => minDuration; set
            {
                if (minDuration != value)
                {
                    if (value > LikelyDuration)
                        LikelyDuration = value;
                    minDuration = value;
                    Update();
                }
            }
        }
        public List<Task> Dependencies { get => dependencies; }
        #endregion

        #region Constructor
        public Task(string name, DateTime start, DateTime? end, int duration, int maxDuration = 0, int minDuration = 0, 
            string description = "", User creator = null, DateTime? creationTime = null, Project project=null, int id = -1) 
            : base(name, start, end, description, creator, creationTime, id)
        {
            this.project = project;
            // todo: move duration logic into properties (min <= likely <= max)
            if (duration == 0)
                mostLikelyDuration = 1;
            else
                mostLikelyDuration = duration;
            if (maxDuration <= 0)
                this.maxDuration = mostLikelyDuration;
            else
                this.maxDuration = maxDuration;
            if (minDuration <= 0)
                this.minDuration = mostLikelyDuration;
            else
                this.minDuration = minDuration;
            dependencies = new List<Task>();
            if (id == -1)
                this.id = Insert();
        }

        public Task(Task task, int id = -1) : base(task, id)
        {
            mostLikelyDuration = task.LikelyDuration;
            minDuration = task.MinDuration;
            maxDuration = task.MaxDuration;
            project = task.Proj;
        }
        #endregion

        #region Workers
        /// <summary>
        /// Adds worker to task
        /// </summary>
        /// <param name="worker">user</param>
        /// <param name="updateDB">Update the database</param>
        /// <returns>true if it was added</returns>
        public override bool AddWorker(User worker, bool updateDB=true)
        {
            bool added = false;
            if (!workers.Contains(worker))
            {
                // Attempt to add...
                try
                {
                    if (updateDB)
                    {
                        Proj.AddWorker(worker);  // Add to project too
                        SqlCommand command = OpenConnection("INSERT INTO dbo.UserTask (UserName, TaskId) Values(@username, @taskId);");
                        command.Parameters.AddWithValue("@username", worker.Username);
                        command.Parameters.AddWithValue("@taskId", this.Id);
                        command.ExecuteNonQuery();
                        CloseConnection();
                    }
                    workers.Add(worker);
                    added = true;
                    Model.Instance.OnModelUpdate(this.Proj);
                }
                catch (SqlException) { }
            }
            return added;
        }

        /// <summary>
        /// Removes the worker from the task
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="updateDB"></param>
        /// <returns></returns>
        public override bool RemoveWorker(User worker, bool updateDB=true)
        {
            bool removed = false;
            if (workers.Contains(worker))
            {
                try
                {
                    if (updateDB)
                    {
                        SqlCommand command = OpenConnection("DELETE FROM UserTask WHERE UserTask.TaskId=@taskId AND UserTask.UserName=@username");
                        command.Parameters.AddWithValue("@taskId", this.Id);
                        command.Parameters.AddWithValue("@username", worker.Username);
                        command.ExecuteNonQuery();
                        CloseConnection();
                    }
                    workers.Remove(worker);
                    removed = true;
                    Model.Instance.OnModelUpdate(this.Proj);
                } catch(SqlException) { }
            }
            return removed;
        }
        #endregion

        #region Dependencies
        public void UpdateDependencies()
        {

        }

        public void AddDependency(Task dependency)
        {

        }

        public void RemoveDependency(Task dependency)
        {

        }
        #endregion

        #region Database Methods
        /// <summary>
        /// Updates the task data in the database
        /// </summary>
        protected override void Update()
        {
            string query = "UPDATE dbo.[Task] SET Name=@Name, StartDate=@StartDate, EndDate=@EndDate" + 
                ", Description=@Description, MinEstDuration=" + MinDuration + ", MaxEstDuration=" + MaxDuration + ", MostLikelyEstDuration=" +
                mostLikelyDuration + " WHERE TaskId = " + Id + ";";                
            SqlCommand command = OpenConnection(query);
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@Description", Description);
            var sd = command.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime);
            sd.Value = StartDate;
            var ed = command.Parameters.Add("@EndDate", System.Data.SqlDbType.DateTime);
            if (EndDate != null)
                ed.Value = EndDate;
            else
                ed.Value = DBNull.Value;
            command.ExecuteNonQuery();
            CloseConnection();
            Model.Instance.OnModelUpdate(this.project);
        }

        /// <summary>
        /// Inserts a task into the database
        /// 2/13/2021 by Robert Nelson
        /// </summary>
        /// <returns>Task id</returns>
        /// <throws>Exception on error (task name is already taken in project or project does not exist)</throws>
        protected override int Insert()
        {
            string query = "EXEC dbo.CreateTask @Name, @Description, " + MinDuration + ", " + mostLikelyDuration + ", " + MaxDuration + 
                ", @StartDate, @EndDate, @ProjectId, @Creator, @CreationDate out, @Result out, @ResultId out";
            SqlCommand command = OpenConnection(query);
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@Description", Description);
            command.Parameters.AddWithValue("@ProjectId", Proj.Id);
            if (creator == null)
                command.Parameters.AddWithValue("@Creator", DBNull.Value);
            else
                command.Parameters.AddWithValue("@Creator", creator.Name);
            var sd = command.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime);
            sd.Value = StartDate;
            var ed = command.Parameters.Add("@EndDate", System.Data.SqlDbType.DateTime);
            if (EndDate != null)
                ed.Value = EndDate;
            else
                ed.Value = DBNull.Value;
            var createDate = command.Parameters.Add("@CreationDate", System.Data.SqlDbType.DateTime);
            createDate.Direction = System.Data.ParameterDirection.Output;
            var result = command.Parameters.Add("@Result", System.Data.SqlDbType.Bit);
            result.Direction = System.Data.ParameterDirection.Output;
            var resultId = command.Parameters.Add("@ResultId", System.Data.SqlDbType.Int);
            resultId.Direction = System.Data.ParameterDirection.Output;
            command.ExecuteNonQuery();
            if (!(bool)result.Value)
                throw new Exception("Failed to insert task " + Name);
            creationDate = (DateTime) createDate.Value;
            return (int) resultId.Value;
        }

        /// <summary>
        /// Deletes a task
        /// </summary>
        public override void Delete()
        {
            string query = "EXEC dbo.TaskDelete " + Id + ";";
            ExecuteSql(query);
            project.RemoveTask(this);
        }

        /// <summary>
        /// Parses the task from the sqldatareader
        /// </summary>
        /// <param name="reader">open SqlDataReader</param>
        /// <param name="users">list of all users</param>
        /// <param name="project">project that task is on</param>
        /// <returns>Task</returns>
        static public Task Parse(SqlDataReader reader, Dictionary<string, User> users, Dictionary<int, Project> projects)
        {
            string creator = DBFunctions.StringCast(reader, "CreatorUsername");
            Project proj = projects[(int)reader["ProjectId"]];
            User user = users != null && creator != "" ? users[creator] : null;
            Task t = new Task(
                (string)reader["Name"],
                (DateTime)reader["StartDate"],
                DBFunctions.DateCast(reader, "EndDate"),
                (int)reader["MostLikelyEstDuration"],
                (int)reader["MaxEstDuration"],
                (int)reader["MinEstDuration"],
                DBFunctions.StringCast(reader, "Description"),
                user,
                DBFunctions.DateCast(reader, "CreationDate"),
                proj,
                (int)reader["TaskId"]);
            proj.AddTask(t);
            return t;
        }

        #endregion
    }
}
