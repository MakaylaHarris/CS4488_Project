﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SmartPert.Model
{
    /// <summary>
    /// A PERT/Gantt project with tasks
    /// Created 1/28/2021 by Robert Nelson
    /// </summary>
    public class Project : TimedItem
    {
        private List<Task> sorted;

        public List<Task> SortedTasks
        {
            get
            {
                if (sorted == null)
                    sorted = SortTasks();
                return sorted;
            }
            set
            {
                sorted = value;
                ResortTasks();
                NotifyUpdate();
            }
        }

        #region Project
        public Project(string name, DateTime start, DateTime? end, string description = "", int id = -1, bool insert = true, bool track=true, IItemObserver observer=null, int likelyDuration=0, int maxDuration=0, int minDuration = 0) 
            : base(name, start, end, description, id, observer, likelyDuration, maxDuration, minDuration)
        {
            PostInit(insert, track);
        }

        /// <summary>
        /// Calculates a last date if none exists for project
        /// Implemented by : Makayla Linnastruth and Robert Nelson
        /// </summary>
        /// <returns>Latest date of a project</returns>
        public DateTime CalculateLastProjectDate()
        {
            DateTime maxEstimate = MaxEstDate;
            return EndDate == null || maxEstimate > EndDate ? maxEstimate : (DateTime)EndDate;
        }

        public void Recalculate()
        {
            int likelyDuration = 1, maxDuration = 1, ndays;
            DateTime start = StartDate;
            foreach(Task t in tasks)
            {
                ndays = (t.LikelyDate - start).Days;
                if (ndays > likelyDuration)
                    likelyDuration = ndays;
                ndays = (t.MaxEstDate - start).Days;
                if (ndays > maxDuration)
                    maxDuration = ndays;
            }
            LikelyDuration = likelyDuration;
            MaxDuration = maxDuration;
        }
        #endregion

        #region Task Methods
        /// <summary>
        /// Determines if the sorted task list is a valid order based on having unique project rows and subtasks underlying their parents.
        /// </summary>
        /// <param name="tasks">sorted tasks</param>
        /// <returns>true if valid</returns>
        protected static bool IsValidSort(List<Task> tasks)
        {
            HashSet<int> rows = new HashSet<int>();
            Stack<Task> stack = new Stack<Task>();
            foreach (Task t in tasks)
            {
                // Must have unique row number
                if (rows.Contains(t.ProjectRow))
                    return false;
                rows.Add(t.ProjectRow);

                // test stack, subtasks should be ordered in a stack-like way
                if (t.ParentTask == null)
                    stack.Clear();
                else
                {
                    try
                    {
                        while (t.ParentTask != stack.First())
                            stack.Pop();
                    }
                    catch (InvalidOperationException)
                    {
                        return false;
                    }
                }
                if (t.Tasks.Count > 0)
                    stack.Push(t);
            }
            return true;
        }

        private static void ResortTasksAddRecursively(List<Task> resorted, Task subtaskToAdd)
        {
            resorted.Add(subtaskToAdd);
            foreach (Task t in subtaskToAdd.Tasks.OrderBy(x => x.ProjectRow))
                ResortTasksAddRecursively(resorted, t);
        }

        /// <summary>
        /// Resorts the task's list to have subtasks under their parent
        /// </summary>
        /// <param name="currentSort">the current sort by project row</param>
        /// <returns>sorted list</returns>
        protected static List<Task> ResortTasks(List<Task> currentSort)
        {
            List<Task> newSort = new List<Task>();
            foreach (Task t in currentSort)
            {
                if (t.ParentTask == null)
                    ResortTasksAddRecursively(newSort, t);
            }
            return newSort;
        }

        private List<Task> SortTasks()
        {
            List<Task> sorted = Tasks.OrderBy(x => x.ProjectRow).ToList();
            if (IsValidSort(sorted))
                return sorted;
            return ResortTasks(sorted);
        }

        public void AddTask(Task t)
        {
            if (!tasks.Contains(t))
            {
                AddTaskNoCheck(t);
                if (t.ProjectRow == -1)
                    t.ProjectRow = tasks.Count > 0 ? tasks.Max(x => x.ProjectRow) + 1 : 0;
                OnChild_Change(t);
                NotifyUpdate();
            }
        }

        public void AddTaskNoCheck(Task t)
        {
            sorted = null;
            tasks.Add(t);
        }

        public void RemoveTask(Task t)
        {
            if (tasks.Remove(t))
            {
                sorted = null;
                NotifyUpdate();
            }
        }
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
                    using(var connection = new DBConnection("INSERT INTO UserProject (UserName, ProjectId) VALUES(@username, @projectId);")) {
                        var command = connection.Command;
                        command.Parameters.AddWithValue("@username", worker.Username);
                        command.Parameters.AddWithValue("@projectId", this.Id);
                        command.ExecuteNonQuery();
                    }
                    workers.Add(worker);
                    added = true;
                    NotifyUpdate();
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
                    using(var conn = new DBConnection("DELETE FROM UserProject WHERE ProjectId = @projectId AND UserName = @username);"))
                    {
                        var command = conn.Command;
                        command.Parameters.AddWithValue("@projectId", Id);
                        command.Parameters.AddWithValue("@username", worker.Username);
                        command.ExecuteNonQuery();
                    }
                    workers.Remove(worker);
                    removed = true;
                    NotifyUpdate();
                } catch (SqlException) { }
            }
            return removed;
        }

        #endregion

        #region Database

        /// <summary>
        /// Resorts the task's project rows
        /// </summary>
        private void ResortTasks()
        {
            for (int i = 0; i < sorted.Count; i++)
                sorted[i].ProjectRow = i;
        }

        /// <summary>
        /// Deletes the project from the database
        /// </summary>
        protected override void PerformDelete()
        {
            DBConnection.ExecuteNonQuery("EXEC ProjectDelete " + Id);
        }

        /// <summary>
        /// Inserts the project into the database
        /// </summary>
        /// <throws>Exception if the call was unsuccessful</throws>
        /// <returns>the inserted id on success</returns>
        protected override int PerformInsert()
        {
            string query = "EXEC CreateProject @ProjectName, @StartDate, @EndDate, @Description, @Creator," 
                + "@LikelyDuration, @MinDuration, @MaxDuration, @CreationDate OUT, @Result OUT, @ResultId OUT";
            using (var conn = new DBConnection(query))
            {
                SqlCommand command = conn.Command;
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
                command.Parameters.AddWithValue("@LikelyDuration", LikelyDuration);
                command.Parameters.AddWithValue("@MinDuration", MinDuration);
                command.Parameters.AddWithValue("@MaxDuration", MaxDuration);
                var createDate = command.Parameters.Add("@CreationDate", System.Data.SqlDbType.DateTime);
                createDate.Direction = System.Data.ParameterDirection.Output;
                var insertedId = command.Parameters.Add("@ResultId", System.Data.SqlDbType.Int);
                insertedId.Direction = System.Data.ParameterDirection.Output;
                var result = command.Parameters.Add("@Result", System.Data.SqlDbType.Bit);
                result.Direction = System.Data.ParameterDirection.Output;
                command.ExecuteNonQuery();
                if (!(bool)result.Value)
                    throw new InsertionError("Failed to insert project " + Name + " into database!");
                creationDate = (DateTime)createDate.Value;
                id = (int)insertedId.Value;
                return id;
            }
        }

        /// <summary>
        /// Updates the project properties in the database
        /// </summary>
        protected override void PerformUpdate()
        {
            string query = "UPDATE Project SET Name=@Name, Description=@Description, StartDate=@StartDate, EndDate=@EndDate, MinEstDuration=" 
                + MinDuration + ", MaxEstDuration=" + MaxDuration + ", MostLikelyEstDuration=" + LikelyDuration + " WHERE ProjectId=@Id;";
            using (var conn = new DBConnection(query))
            {
                SqlCommand command = conn.Command;
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@Description", Description);
                command.Parameters.AddWithValue("@StartDate", StartDate);
                if (EndDate == null)
                    command.Parameters.AddWithValue("@EndDate", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@EndDate", EndDate);
                command.Parameters.AddWithValue("@Id", Id);
                command.ExecuteNonQuery();
            }
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
            p.mostLikelyDuration = DBFunctions.IntCast(reader, "MostLikelyEstDuration");
            p.maxDuration = DBFunctions.IntCast(reader, "MaxEstDuration", p.mostLikelyDuration); 
            p.minDuration = DBFunctions.IntCast(reader, "MinEstDuration", p.mostLikelyDuration); 
            return p;
        }

        #endregion
    }
}
