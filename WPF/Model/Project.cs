using System;
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
                if(sorted == null)
                    sorted = Tasks.OrderBy(x => x.ProjectRow).ToList();
                return sorted;
            }
        }

        #region Project
        public Project(string name, DateTime start, DateTime? end, string description = "", int id = -1, bool insert = true, bool track=true, IItemObserver observer=null) 
            : base(name, start, end, description, id, observer)
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

        #endregion

        #region Task Methods
        public void AddTask(Task t)
        {
            if (!tasks.Contains(t))
            {
                sorted = null;
                tasks.Add(t);
                t.Subscribe(this);
                NotifyUpdate();
            }
        }

        public void RemoveTask(Task t)
        {
            if (tasks.Remove(t))
            {
                sorted = null;
                t.UnSubscribe(this);
                NotifyUpdate();
            }
        }

        public override void OnUpdate(IDBItem item)
        {
            sorted = null;      // make it resort
            base.OnUpdate(item);
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
                    SqlCommand command = OpenConnection("INSERT INTO UserProject (UserName, ProjectId) VALUES(@username, @projectId);");
                    command.Parameters.AddWithValue("@username", worker.Username);
                    command.Parameters.AddWithValue("@projectId", this.Id);
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
                    NotifyUpdate();
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
            string query = "EXEC CreateProject @ProjectName, @StartDate, @EndDate, @Description, @Creator," 
                + "@LikelyDuration, @MinDuration, @MaxDuration, @CreationDate OUT, @Result OUT, @ResultId OUT";
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
            creationDate = (DateTime) createDate.Value;
            id = (int) insertedId.Value;
            return id;
        }

        /// <summary>
        /// Updates the project properties in the database
        /// </summary>
        protected override void PerformUpdate()
        {
            string query = "UPDATE Project SET Name=@Name, Description=@Description, StartDate=@StartDate, EndDate=@EndDate, MinEstDuration=" 
                + MinDuration + ", MaxEstDuration=" + MaxDuration + ", MostLikelyEstDuration=" + LikelyDuration + " WHERE ProjectId=@Id;";
            SqlCommand command = OpenConnection(query);
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
            p.mostLikelyDuration = DBFunctions.IntCast(reader, "MostLikelyEstDuration");
            p.maxDuration = DBFunctions.IntCast(reader, "MaxEstDuration", p.mostLikelyDuration); 
            p.minDuration = DBFunctions.IntCast(reader, "MinEstDuration", p.mostLikelyDuration); 
            return p;
        }

        #endregion
    }
}
