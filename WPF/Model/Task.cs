using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SmartPert.Model
{
    /// <summary>
    /// A Task is a single item on the Pert chart that has a most likely duration, minimum duration, and maximum duration.
    /// Dependencies are Tasks that cannot be started until this one is finished.
    /// Created 1/29/2021 by Robert Nelson
    /// </summary>
    public class Task : TimedItem
    {
        private Project project;
        private HashSet<Task> dependencies;
        private Task parentTask;
        private int projectRow;    // The nth child of a parent

        #region Properties
        /// <summary>
        /// Gets the project the task is on
        /// Added 2/13/2021 by Robert Nelson
        /// </summary>
        public Project Project
        {
            get => project;
            private set
            {
                project = value;
                project.AddTask(this);
            }
        }
        public HashSet<Task> Dependencies { get => dependencies; }
        public Task ParentTask { get => parentTask; }

        public int ProjectRow { get => projectRow; 
            private set
            {
                projectRow = value;
                DB_UpdateRow();
                NotifyUpdate();
            } }
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
            : base(name, start, end, description, id, observer, duration, maxDuration, minDuration)
        {
            this.Project = project != null ? project : Model.Instance.GetProject();
            if (this.project == null)
                throw new ArgumentNullException("project");
            dependencies = new HashSet<Task>();
            projectRow = 0;
            PostInit(insert, track);
        }

        #endregion

        #region Task Methods
        /// <summary>
        /// Gets the last subtask's row in the project, this being the parent task. 
        /// If it has no subtasks retrieves this task's current row.
        /// </summary>
        /// <returns>last row</returns>
        public int GetLastSubtaskRow()
        {
            if (Tasks.Count <= 0)
                return projectRow;
            List<Task> tasks = Project.SortedTasks;
            int i;
            for (i = tasks.IndexOf(this) + 1; i < tasks.Count; i++)
                if (tasks[i].parentTask != this)
                    break;
            return tasks[i - 1].projectRow;
        }

        /// <summary>
        /// Shifts the task to the row in the project
        /// </summary>
        /// <param name="row">the target row</param>
        public void ShiftToRow(int row)
        {
            if (row == projectRow)      // We're already done!
                return;
            List<Task> tasks = Project.SortedTasks;
            // where am I?
            int myId = tasks.IndexOf(this);
            // Target row is further down, so shift up
            if (row > projectRow)
                for (int i = myId + 1; tasks[i].projectRow <= row; i++)
                    tasks[i].ProjectRow -= 1;
            else // Target row is up, so shift others down
                for (int i = myId - 1; tasks[i].projectRow >= row; i--)
                    tasks[i].ProjectRow += 1;
            // And finally update mine
            ProjectRow = row;
        }

        /// <summary>
        /// Adds sub task to tasks
        /// </summary>
        /// <param name="t">Task</param>
        public bool AddSubTask(Task t)
        {
            if (!tasks.Contains(t))
            {
                t.ShiftToRow(GetLastSubtaskRow() + 1);
                t.parentTask = this;
                tasks.Add(t);
                t.Subscribe(this);
                t.InsertSubTask();
                NotifyUpdate();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes sub task from tasks
        /// </summary>
        /// <param name="t">task to remove</param>
        public bool RemoveSubTask(Task t)
        {
            if (tasks.Remove(t))
            {
                t.UnSubscribe(this);
                t.DeleteSubTask();
                t.parentTask = null;
                NotifyUpdate();
                return true;
            }
            return false;
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
                    Project.AddWorker(worker);  // Add to project too
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
        private void DB_UpdateRow()
        {
            string query = "UPDATE dbo.[Task] SET ProjectRow=" + projectRow + " WHERE TaskId=" + Id + ";";
            ExecuteSql(query);
        }

        /// <summary>
        /// Updates the task data in the database
        /// </summary>
        protected override void PerformUpdate()
        {
            string query = "UPDATE dbo.[Task] SET Name=@Name, StartDate=@StartDate, EndDate=@EndDate" + 
                ", Description=@Description, MinEstDuration=" + MinDuration + ", MaxEstDuration=" + MaxDuration + ", MostLikelyEstDuration=" +
                LikelyDuration + ", ProjectRow=" + projectRow + " WHERE TaskId = " + Id + ";";                
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
            string query = "EXEC dbo.CreateTask @Name, @Description, " + MinDuration + ", " + LikelyDuration + ", " + MaxDuration + 
                ", @StartDate, @EndDate, @ProjectId, @Creator, @CreationDate out, @Result out, @ResultId out, " +
                "@ProjectRow out, @HasParent, @ParentTaskId";
            SqlCommand command = OpenConnection(query);
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@Description", Description);
            command.Parameters.AddWithValue("@ProjectId", Project.Id);
            if (creator == null)
                command.Parameters.AddWithValue("@Creator", DBNull.Value);
            else
                command.Parameters.AddWithValue("@Creator", creator.Username);
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
            var childRow = command.Parameters.Add("@ProjectRow", System.Data.SqlDbType.Int);
            childRow.Direction = System.Data.ParameterDirection.Output;
            if(parentTask != null)
            {
                command.Parameters.AddWithValue("@HasParent", 1);
                command.Parameters.AddWithValue("@ParentTaskId", parentTask.Id);
            } else
            {
                command.Parameters.AddWithValue("@HasParent", 0);
                command.Parameters.AddWithValue("@ParentTaskId", 0);
            }
            command.ExecuteNonQuery();
            if (!(bool)result.Value)
                throw new InsertionError("Failed to insert task " + Name);
            creationDate = (DateTime) createDate.Value;
            id = (int) resultId.Value;
            projectRow = (int)childRow.Value;
            CloseConnection();

            // If the parent task exists, be sure to move it next to it
            if(parentTask != null)
                ShiftToRow(parentTask.GetLastSubtaskRow() + 1);
            return id;
        }

        private void InsertSubTask()
        {
            if (parentTask != null)
            {
                SqlCommand command = OpenConnection("INSERT INTO [dbo].[SubTask] (SubTaskId, ParentTaskId) Values (@SubId, @ParentId);");
                command.Parameters.AddWithValue("@SubId", Id);
                command.Parameters.AddWithValue("@ParentId", parentTask.Id);
                command.ExecuteNonQuery();
                CloseConnection();
            }
        }

        private void DeleteSubTask()
        {
            ExecuteSql("DELETE FROM [dbo].[SubTask] WHERE SubTaskId=" + Id);
        }

        /// <summary>
        /// Deletes a task
        /// </summary>
        protected override void PerformDelete()
        {
            string query = "EXEC dbo.TaskDelete " + Id + ";";
            ExecuteSql(query);
            if(parentTask != null)
                parentTask.RemoveSubTask(this);
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
            int mostLikely = (int)reader["MostLikelyEstDuration"];
            Task t = new Task(
                (string)reader["Name"],
                (DateTime)reader["StartDate"],
                DBFunctions.DateCast(reader, "EndDate"),
                mostLikely,
                DBFunctions.IntCast(reader, "MaxEstDuration", mostLikely),
                DBFunctions.IntCast(reader, "MinEstDuration", mostLikely),
                DBFunctions.StringCast(reader, "Description"),
                project: proj,
                id: (int)reader["TaskId"],
                insert: false);

            t.creator = user;
            t.creationDate = (DateTime)DBFunctions.DateCast(reader, "CreationDate");
            t.projectRow = (int)reader["ProjectRow"];
            proj.AddTask(t);
            return t;
        }

        public override bool PerformParse(SqlDataReader reader)
        {
            bool result = base.PerformParse(reader);
            int projectRow = (int)reader["ProjectRow"];
            if(projectRow != this.projectRow)
            {
                this.projectRow = projectRow;
                this.isUpdated = true;
                return true;
            }
            return result;
        }

        /// <summary>
        /// Updates Subtasks (dbreader only)
        /// </summary>
        /// <param name="subtasks">updated</param>
        public void DB_UpdateSubtasks(HashSet<Task> subtasks)
        {
            // If tasks are gone, remove them 
            if(subtasks == null)
            {
                foreach (Task t in Tasks)
                {
                    t.parentTask = null;
                    t.isUpdated = true;
                }
                Tasks.Clear();
                isUpdated = true;
            } else if(!tasks.SetEquals(subtasks)) // If the subtasks have changed, update the ones that have changed
            {
                IEnumerable<Task> added = tasks.Except(subtasks);
                foreach(Task t in added)
                {
                    t.parentTask = this;
                    t.isUpdated = true;
                }
                IEnumerable<Task> removed = subtasks.Except(tasks);
                foreach(Task t in removed)
                {
                    if(t.ParentTask == this)
                    {
                        t.parentTask = null;
                        t.isUpdated = true;
                    }
                }
                tasks = subtasks;
                isUpdated = true;
            }
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
            DateTime maxEstimate = MaxEstDate;
            return EndDate == null || maxEstimate > EndDate ? maxEstimate : (DateTime)EndDate;
        }
    }
}
