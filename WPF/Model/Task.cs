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
        private static bool calculateDependentsMaxEstimate = true;
        private Project project;
        private HashSet<Task> dependencies;
        private HashSet<Task> dependentOn;
        protected DateTime? dependentEstStartDate;  // Estimated start date based on dependencies
        private Task parentTask;
        private int projectRow;

        #region Properties
        /// <summary>
        /// Gets the actual (if not null) or estimated end date (based on calculating the dependent's start date or ours, whichever is last)
        /// </summary>
        public DateTime ActualOrEstimatedEnd { get
            {
                if(endDate == null)
                {
                    if (calculateDependentsMaxEstimate)
                        return MaxEstStartDate.AddDays(maxDuration);
                    else
                        return MaxEstStartDate.AddDays(LikelyDuration);
                } else
                {
                    return (DateTime)endDate;
                }
            } 
        }

        /// <summary>
        /// This returns the max estimated start date, based on dependents or the start date max date
        /// or the end date
        /// </summary>
        public DateTime MaxEstStartDate
        {
            get {
                DateTime depEst = DependentEstStartDate;
                if (depEst > startDate && (endDate == null || ((DateTime)endDate) > depEst))
                {
                    return depEst;
                }
                return startDate;
            }
        }

        /// <summary>
        /// Gets the set start date (not the calculated one)
        /// </summary>
        public DateTime SetStartDate
        {
            get => startDate; set => StartDate = value;
        }

        public override DateTime StartDate { get => MaxEstStartDate; set => base.StartDate = value; }


        public DateTime DependentEstStartDate { 
            get
            {
                if (dependentEstStartDate == null)
                    dependentEstStartDate = GetDependentEstStartDate();
                return (DateTime) dependentEstStartDate;
            } 
        }

        /// <summary>
        /// List of Subtasks
        /// </summary>
        public List<Task> SubTasks { get => Tasks.ToList(); }

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
            }
        }
        public HashSet<Task> Dependencies { get => dependencies; }
        public HashSet<Task> DependentOn { get => dependentOn; }

        public Task ParentTask { get => parentTask; }

        public int ProjectRow { get => projectRow;
            set
            {
                projectRow = value;
                if(CanConnectDB)
                    DB_UpdateRow();
            } }

        public static bool CalculateDependentsMaxEstimate { get => calculateDependentsMaxEstimate; 
            set { 
                calculateDependentsMaxEstimate = value;
                foreach (Task t in DBReader.Instance.Tasks.Values)
                {   
                    // Reset the est start dates
                    t.dependentEstStartDate = null;
                }
                Project project = Model.Instance.GetProject();
                if(project != null)
                    project.Recalculate();
                Model.Instance.OnModelUpdate(project);
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
            string description = "", Project project=null, int id = -1, bool insert=true, bool track=true, IItemObserver observer=null, bool addToProject=true)
            : base(name, start, end, description, id, observer, duration, maxDuration, minDuration)
        {
            this.Project = project != null ? project : Model.Instance.GetProject();
            //if (this.project == null)
            //    throw new ArgumentNullException("project");
            dependencies = new HashSet<Task>();
            dependentOn = new HashSet<Task>();
            projectRow = -1;
            PostInit(insert, track);
            if (project != null)
            {
                if (addToProject)
                    project.AddTask(this);
            }
        }

        /// <summary>
        /// Creates a shallow copy of a task
        /// </summary>
        /// <param name="task">Task</param>
        public Task(Task task, bool insert=false, bool track=false) : base (task.name, task.startDate, task.endDate, task.description, task.id, null, task.mostLikelyDuration, task.maxDuration, task.minDuration)
        {
            dependencies = task.dependencies;
            dependentOn = task.dependentOn;
            project = task.project;
            projectRow = task.projectRow;
            parentTask = task.parentTask;
            dependentEstStartDate = task.dependentEstStartDate;
            PostInit(insert, track);
        }
        #endregion

        #region Property Callbacks
        protected override void AfterStartDateChanged(DateTime dateTime)
        {
            DateTime likely = LikelyDate, max = MaxEstDate, min = MinEstDate;
            if (parentTask != null)
            {
                parentTask.OnChild_StartDateChange(dateTime);
                parentTask.OnChild_MaxEstDateChange(max);
                parentTask.OnChild_LikelyDateChange(likely);
                parentTask.OnChild_MinEstDateChange(min);
            }
            // Changing the start date also changes all of our estimated duration dates
            if(project != null)
            {
                project.OnChild_StartDateChange(dateTime);
                project.OnChild_MaxEstDateChange(max);
                project.OnChild_LikelyDateChange(likely);
                project.OnChild_MinEstDateChange(min);
            }
            ResetDependentEstStartDate();
        }

        protected override void AfterEndDateChanged(DateTime? newValue)
        {
            if (parentTask != null)
                parentTask.OnChild_CompletedDateChange(newValue);
            if(project != null)
                project.OnChild_CompletedDateChange(newValue);
            // reset dependent's estimations
            foreach (Task t in dependencies)
                t.ResetDependentEstStartDate();
        }
        protected override void AfterLikelyDurationChanged(int newValue)
        {
            if (parentTask != null)
                parentTask.OnChild_LikelyDateChange(LikelyDate);
            if(project != null)
                project.OnChild_LikelyDateChange(LikelyDate);
            if(!calculateDependentsMaxEstimate)
                foreach (Task t in dependencies)
                    t.ResetDependentEstStartDate();
        }
        protected override void AfterMaxDurationChanged(int newValue)
        {
            if (parentTask != null)
                parentTask.OnChild_MaxEstDateChange(MaxEstDate);
            if(project != null)
                project.OnChild_MaxEstDateChange(MaxEstDate);
            if(calculateDependentsMaxEstimate)
                foreach (Task t in dependencies)
                    t.ResetDependentEstStartDate();
        }
        protected override void AfterMinDurationChanged(int newValue)
        {
            if (parentTask != null)
                parentTask.OnChild_MinEstDateChange(MinEstDate);
            if(project != null)
                project.OnChild_MinEstDateChange(MinEstDate);
        }
        #endregion

        #region Task Methods
        /// <summary>
        /// Determines if a task is an ancestor of this task
        /// </summary>
        /// <param name="possibleAncestor">The task to check</param>
        /// <returns>True if ancestor</returns>
        public bool TaskIsAncestor(Task possibleAncestor)
        {
            Task parent = ParentTask;
            while (parent != null)
            {
                if (parent == possibleAncestor)
                    return true;
                parent = parent.ParentTask;
            }
            return false;
        }

        /// <summary>
        /// Returns the number of all of its subtasks including descendants
        /// </summary>
        /// <returns>int</returns>
        public int CountAllSubtasks()
        {
            int sum = 0;
            foreach (Task t in Tasks)
                sum += t.CountAllSubtasks() + 1;
            return sum;
        }

        /// <summary>
        /// Determines if a task can be added as a subtask based on dependencies and ancestors (Does not check project row)
        /// </summary>
        /// <param name="t">The task to add</param>
        /// <returns>true if it can add it</returns>
        public bool CanAddSubTask(Task t)
        {
            Task parent = t.ParentTask;
            if (parent == this) // Already added!
                return false;
            bool result = t.IsDependentDescendant(this) || t.IsDependentTracebackChildren(this, ignoreIfParent: true);
            return !result;
        }

        /// <summary>
        /// Adds sub task to tasks, WARNING: Does not update or check project row
        /// </summary>
        /// <param name="t">Task</param>
        public bool AddSubTask(Task t)
        {
            if (t.parentTask != this)
            {
                if (t.parentTask != null)
                    throw new Exception(string.Format("Task {0} already has a parent, remove it from its parent first!", t));
                t.parentTask = this;
                tasks.Add(t);
                OnChild_Change(t);
                if(CanConnectDB)
                    t.DB_InsertSubTask();
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
                t.parentTask = null;
                if(CanConnectDB)
                    t.DB_DeleteSubTask();
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
                if(!CanConnectDB || DB_AddWorker(worker))
                {
                    Project.AddWorker(worker);  // Add to project too
                    workers.Add(worker);
                    added = true;
                    NotifyUpdate();
                }
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
                if(!CanConnectDB || DB_RemoveWorker(worker)) { 
                    workers.Remove(worker);
                    NotifyUpdate();
                    removed = true;
                }
            }
            return removed;
        }
        #endregion

        #region Dependencies
        public void ResetDependentEstStartDate()
        {
            ResetDependentEstStartDateHelper();
            if(project != null)
                project.Recalculate();
        }
        // Resets estimated dependent date for task, and all of its dependents
        public void ResetDependentEstStartDateHelper()
        {
            this.dependentEstStartDate = null;
            foreach (Task t in dependencies)
                t.ResetDependentEstStartDateHelper();
            foreach (Task t in Tasks)
                t.ResetDependentEstStartDateHelper();
        }


        private DateTime? GetDependentEstStartDate()
        {
            DateTime start = DateTime.MinValue, compare;
            foreach(Task t in dependentOn)
            {
                compare = t.ActualOrEstimatedEnd;
                if (compare > start)
                    start = compare;
            }
            // And check task parent
            if(ParentTask != null)
            {
                compare = ParentTask.StartDate;
                if (compare > start)
                    start = compare;
            }
            return start;
        }

        /// <summary>
        /// Determines if the dependency can be added, checking that its not a subtask or parent and not going to create circular dependencies
        /// </summary>
        /// <param name="dependency">the dependency to add</param>
        /// <returns>true if it can</returns>
        public bool CanAddDependency(Task dependency)
        {
            if (dependencies.Contains(dependency) || IsDependentTracebackChildren(dependency))
                return false;
            return true;
        }

        private bool IsDependentTracebackChildren(Task dependent, HashSet<Task> checkedTasks=null, bool checkThis = true, bool ignoreIfParent=false)
        {
            if (checkedTasks == null)
                checkedTasks = new HashSet<Task>();
            if (checkThis && IsDependentAncestor(dependent, checkedTasks, ignoreIfParent: ignoreIfParent))
                return true;
            foreach (Task t in Tasks)
                if (t.IsDependentTracebackChildren(dependent, checkedTasks, ignoreIfParent:ignoreIfParent))
                    return true;
            return false;
        }

        private bool IsDependentAncestor(Task t, HashSet<Task> checkedTasks=null, bool isParent=false, bool ignoreIfParent=false)
        {
            if (t == this)
                return true;
            if (checkedTasks == null)
                checkedTasks = new HashSet<Task> { this };
            else
                checkedTasks.Add(this);
            // Check those we're dependent on
            foreach (Task task in dependentOn)
            {
                if (!checkedTasks.Contains(task) && task.IsDependentAncestor(t, checkedTasks, ignoreIfParent: ignoreIfParent))
                    return true;
            }
            // As well as parent tasks
             if (!ignoreIfParent && parentTask != null && !checkedTasks.Contains(parentTask) && parentTask.IsDependentAncestor(t, checkedTasks, true))
                return true;
            // If we're not a parent, then we have a solid connection tracing back so we cannot add children
            if (!isParent && IsDependentTracebackChildren(t, checkedTasks, false, ignoreIfParent))
                return true;
            return false;
        }

        private bool IsDependentDescendant(Task t, HashSet<Task> checkedTasks=null)
        {
            if (t == this)
                return true;
            if (checkedTasks == null)
                checkedTasks = new HashSet<Task> { this };
            else
                checkedTasks.Add(this);
            foreach (Task task in dependencies)
                if (!checkedTasks.Contains(task) && task.IsDependentDescendant(t, checkedTasks))
                    return true;
            // Check subtasks as well
            foreach (Task task in SubTasks)
                if (!checkedTasks.Contains(task) && task.IsDependentDescendant(t, checkedTasks))
                    return true;
            return false;
        }


        /// <summary>
        /// Creates a dependency, the parameter is the Root, and the current task is the dependent.
        /// </summary>
        public void AddDependency(Task dependency)
        {
            if (dependencies.Contains(dependency))
            {
                throw new Exception("Dependency already exists");
            }
            if (CanConnectDB)
                DB_AddDependency(dependency);
            dependencies.Add(dependency);
            dependency.dependentOn.Add(this);
            ResetDependentEstStartDate();
        }
        /// <summary>
        /// Removes a dependency, the parameter is the Root, and the current task is the dependent.
        /// </summary>
        public void RemoveDependency(Task dependency)
        {
            if (!(dependencies.Contains(dependency)))
                {
                throw new Exception("Dependency does not exist between these two tasks");
                }
            if (CanConnectDB)
                DB_RemoveDependency(dependency);
            dependencies.Remove(dependency);
            dependency.dependentOn.Remove(this);
            dependency.ResetDependentEstStartDate();
        }
        /// <summary>
        /// Calls sproc DeleteDependency that removes all dependencies associated with the task to be deleted
        /// </summary>
        public void DeleteAllDependencies(Task delTask)
        {
            foreach (Task t in dependencies)
                t.dependentOn.Remove(this);
            dependencies.Clear();
            if(CanConnectDB)
            {
                string query = "EXEC DeleteDependency @taskId";
                using(var conn = new DBConnection(query))
                {
                    SqlCommand command =conn.Command;
                    command.Parameters.AddWithValue("@taskId", delTask.Id);
                    command.ExecuteNonQuery();
                }
            }
            NotifyUpdate();
        }
        #endregion

        #region Database Methods
        private bool DB_RemoveDependency(Task dependency)
        {
            try
            {
                string query = string.Format("EXEC RemoveDependency {0}, {1}", Id, dependency.Id);
                DBConnection.ExecuteNonQuery(query);
                return true;
            } catch(SqlException) { }
            return false;
        }

        private bool DB_AddDependency(Task dependency)
        {
            try
            {
                string query = string.Format("EXEC CreateDependency {0}, {1}", Id, dependency.Id);
                DBConnection.ExecuteNonQuery(query);
                return true;
            } catch(SqlException) { }
            return false;
        }

        private bool DB_AddWorker(User worker)
        {
            try
            {
                using (var con = new DBConnection("INSERT INTO dbo.UserTask (UserName, TaskId) Values(@username, @taskId);"))
                {
                    SqlCommand command = con.Command;
                    command.Parameters.AddWithValue("@username", worker.Username);
                    command.Parameters.AddWithValue("@taskId", this.Id);
                    command.ExecuteNonQuery();
                }
                return true;
            } catch(SqlException)
            { }
            return false;
        }
        private bool DB_RemoveWorker(User worker)
        {
            try
            {
                using (var conn = new DBConnection("DELETE FROM UserTask WHERE UserTask.TaskId=@taskId AND UserTask.UserName=@username")) {
                    SqlCommand command = conn.Command;
                    command.Parameters.AddWithValue("@taskId", this.Id);
                    command.Parameters.AddWithValue("@username", worker.Username);
                    command.ExecuteNonQuery(); 
                } 
                return true;
            }
            catch (SqlException) { }
            return false;
        }

        private void DB_UpdateRow()
        {
            string query = "UPDATE dbo.[Task] SET ProjectRow=" + projectRow + " WHERE TaskId=" + Id + ";";
            DBConnection.ExecuteNonQuery(query);
        }

        /// <summary>
        /// Updates the task data in the database
        /// </summary>
        protected override void PerformUpdate()
        {
            string query = "UPDATE dbo.[Task] SET Name=@Name, StartDate=@StartDate, EndDate=@EndDate" +
                ", Description=@Description, MinEstDuration=" + MinDuration + ", MaxEstDuration=" + MaxDuration + ", MostLikelyEstDuration=" +
                LikelyDuration + ", ProjectRow=" + projectRow + " WHERE TaskId = " + Id + ";";
            using (var conn = new DBConnection(query))
            {
                SqlCommand command = conn.Command;
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
            }
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
            using(var conn = new DBConnection(query))
            {
                SqlCommand command = conn.Command;
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
            }

            // If the parent task exists, be sure to move it next to it
            //if(parentTask != null)
            //    TryShiftToRow(parentTask.GetTaskAfterGroup());
            return id;
        }

        private void DB_InsertSubTask()
        {
            if (parentTask != null)
            {
                DBConnection.ExecuteNonQuery(string.Format("INSERT INTO [dbo].[SubTask] (SubTaskId, ParentTaskId) Values ({0}, {1});", Id, parentTask.Id));
            }
        }

        private void DB_DeleteSubTask()
        {
            DBConnection.ExecuteNonQuery("DELETE FROM [dbo].[SubTask] WHERE SubTaskId=" + Id);
        }

        /// <summary>
        /// Deletes a task
        /// </summary>
        protected override void PerformDelete()
        {
            string query = "EXEC dbo.TaskDelete " + Id + ";";
            DBConnection.ExecuteNonQuery(query);
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
                insert: false,
                addToProject: false);

            // update project
            t.project = proj;
            proj.AddTaskNoCheck(t);
            t.creator = user;
            t.creationDate = (DateTime)DBFunctions.DateCast(reader, "CreationDate");
            t.projectRow = (int)reader["ProjectRow"];
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

        public static List<Task> Set_Diff(HashSet<Task> set, IEnumerable<Task> other)
        {
            List<Task> ret = new List<Task>();
            foreach (Task t in set)
                if (!other.Contains(t))
                    ret.Add(t);
            return ret;
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
                    tasks.Remove(t);
                }
                isUpdated = true;
            } else if(!tasks.SetEquals(subtasks)) // If the subtasks have changed, update the ones that have changed
            {
                foreach (Task t in Set_Diff(subtasks, tasks))
                {
                    t.parentTask = this;
                    tasks.Add(t);
                }
                foreach (Task t in Set_Diff(tasks, subtasks))
                {
                    t.parentTask = null;
                    tasks.Remove(t);
                }
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

        /// <summary>
        /// Updates dependencies (dbreader only)
        /// </summary>
        /// <param name="updatedDependentOn">updated</param>
        public void DB_UpdateDependentOn(HashSet<Task> updatedDependentOn)
        {
            if (updatedDependentOn == null)
            {
                dependentOn.Clear();
                isUpdated = true;
            }
            else if (!dependentOn.SetEquals(updatedDependentOn))
            {
                dependentOn = updatedDependentOn;
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
