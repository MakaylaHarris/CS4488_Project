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
        private HashSet<Task> dependencies;

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
                    PerformUpdate();
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
                    PerformUpdate();
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
                    PerformUpdate();
                }
            }
        }
        public HashSet<Task> Dependencies { get => dependencies; }
        #endregion

        #region Constructor
        /// <summary>
        /// Task constructor
        /// </summary>
        /// <param name="name">Task name (must be unique on project)</param>
        /// <param name="start">Start Date</param>
        /// <param name="end">EndDate (optional)</param>
        /// <param name="duration">Likely estimated duration</param>
        /// <param name="maxDuration">Maximum estimated duration (Optional)</param>
        /// <param name="minDuration">Minimum estimated duration (Optional)</param>
        /// <param name="description">Task Description (Optional)</param>
        /// <param name="project">Project associated</param>
        /// <param name="id">Task id</param>
        /// <param name="insert">flag to insert it into database</param>
        /// <param name="track">flag to track item</param>
        /// <param name="observer">observer for updates</param>
        public Task(string name, DateTime start, DateTime? end, int duration, int maxDuration = 0, int minDuration = 0, 
            string description = "", Project project=null, int id = -1, bool insert=true, bool track=true, IItemObserver observer=null) 
            : base(name, start, end, description, id, observer)
        {
            this.project = project != null ? project : Model.Instance.GetProject();
            if (this.project == null)
                throw new ArgumentNullException("project");
            if (duration == 0)
                mostLikelyDuration = 1;
            else
                mostLikelyDuration = duration;
            if (maxDuration <= 0)
                this.maxDuration = mostLikelyDuration;
            else
            {
                if (maxDuration < mostLikelyDuration)
                    throw new ArgumentOutOfRangeException("MaxDuration", maxDuration, "Must be greater than likely duration!");
                this.maxDuration = maxDuration;
            }
            if (minDuration <= 0)
                this.minDuration = mostLikelyDuration;
            else
            {
                if (minDuration > mostLikelyDuration)
                    throw new ArgumentOutOfRangeException("MinDuration", minDuration, "Must be less than likely duration!");
                this.minDuration = minDuration;
            }
            dependencies = new HashSet<Task>();
            PostInit(insert, track);
        }

        #endregion

        #region Workers
        /// <summary>
        /// Adds worker to task
        /// </summary>
        /// <param name="worker">user</param>
        /// <returns>true if it was added</returns>
        public override bool AddWorker(User worker)
        {
            bool added = false;
            if (!workers.Contains(worker))
            {
                // Attempt to add...
                try
                {
                    Proj.AddWorker(worker);  // Add to project too
                    SqlCommand command = OpenConnection("INSERT INTO dbo.UserTask (UserName, TaskId) Values(@username, @taskId);");
                    command.Parameters.AddWithValue("@username", worker.Username);
                    command.Parameters.AddWithValue("@taskId", this.Id);
                    command.ExecuteNonQuery();
                    CloseConnection();
                    workers.Add(worker);
                    added = true;
                    NotifyUpdate();
                }
                catch (SqlException) { }
            }
            return added;
        }

        /// <summary>
        /// Removes the worker from the task
        /// </summary>
        /// <param name="worker"></param>
        /// <returns></returns>
        public override bool RemoveWorker(User worker)
        {
            bool removed = false;
            if (workers.Contains(worker))
            {
                try
                {
                    SqlCommand command = OpenConnection("DELETE FROM UserTask WHERE UserTask.TaskId=@taskId AND UserTask.UserName=@username");
                    command.Parameters.AddWithValue("@taskId", this.Id);
                    command.Parameters.AddWithValue("@username", worker.Username);
                    command.ExecuteNonQuery();
                    CloseConnection();
                    workers.Remove(worker);
                    removed = true;
                    NotifyUpdate();
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
        protected override void PerformUpdate()
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
        }

        /// <summary>
        /// Inserts a task into the database
        /// 2/13/2021 by Robert Nelson
        /// </summary>
        /// <returns>Task id</returns>
        /// <throws>InsertionError on error (task name is already taken in project or project does not exist)</throws>
        protected override int PerformInsert()
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
                throw new InsertionError("Failed to insert task " + Name);
            creationDate = (DateTime) createDate.Value;
            id = (int) resultId.Value;
            return id;
        }

        /// <summary>
        /// Deletes a task
        /// </summary>
        protected override void PerformDelete()
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
        /// <param name="projects">projects</param>
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
                project: proj,
                id: (int)reader["TaskId"],
                insert: false);

            t.creator = user;
            t.creationDate = (DateTime)DBFunctions.DateCast(reader, "CreationDate");
            proj.AddTask(t);
            return t;
        }

        /// <summary>
        /// Parses the task data
        /// </summary>
        /// <param name="reader">Sql Data reader</param>
        /// <returns>true if updated</returns>
        public override bool PerformParse(SqlDataReader reader)
        {
            string name = (string)reader["Name"];
            DateTime start = (DateTime)reader["StartDate"];
            DateTime? end = DBFunctions.DateCast(reader, "EndDate");
            int likely = (int)reader["MostLikelyEstDuration"];
            int max = (int)reader["MaxEstDuration"];
            int min = (int)reader["MinEstDuration"];
            string desc = DBFunctions.StringCast(reader, "Description");
            if(name != Name || start != StartDate || end != EndDate || likely != mostLikelyDuration || max != maxDuration || min != minDuration || desc != Description)
            {
                this.Name = name;
                this.StartDate = start;
                EndDate = end;
                LikelyDuration = mostLikelyDuration;
                maxDuration = max;
                minDuration = min;
                Description = desc;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates dependencies (dbreader only)
        /// </summary>
        /// <param name="updatedDependencies">updated</param>
        public void DB_UpdateDependencies(HashSet<Task> updatedDependencies)
        {
            if(updatedDependencies == null)
            {
                dependencies.Clear();
                isUpdated = true;
            }
            else if(!dependencies.SetEquals(updatedDependencies))
            {
                dependencies = updatedDependencies;
                isUpdated = true;
            }
        }

        #endregion

        /// <summary>
        /// Calculates the last date for the task if none exists
        /// Implemented by: Makayla Linnastruth and Robert Nelson
        /// </summary>
        /// <returns></returns>
        public DateTime CalculateLastTaskDate()
        {
            if (this.EndDate != null)
            {
                return (DateTime)this.EndDate;
            }
            else
            {
                return this.StartDate.AddDays(maxDuration);
            }
        }
    }
}
