using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Pert.Model
{
    /// <summary>
    /// A Task is a single item on the Pert chart that has a most likely duration, minimum duration, and maximum duration.
    /// Dependencies are Tasks that cannot be started until this one is finished.
    /// Created 1/29/2021 by Robert Nelson
    /// </summary>
    public class Task : TimedItem
    {
        private int mostLikelyDuration;
        private int maxDuration;
        private int minDuration;
        private List<Task> dependencies;

        public int LikelyDuration
        {
            get => mostLikelyDuration;
            set
            {
                mostLikelyDuration = value;
                Update();
            }
        }
        public int MaxDuration { get => maxDuration; 
            set {
                maxDuration = value;
                Update();
            } 
        }

        public int MinDuration
        {
            get => minDuration; set
            {
                minDuration = value;
                Update();
            }
        }
        public List<Task> Dependencies { get => dependencies; }

        public Task(string name, DateTime start, DateTime? end, int duration, int maxDuration = 0, int minDuration = 0, string description = "", int id = -1) : base(name, start, end, description, id)
        {
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
        }



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
        protected override void Update()
        {
            throw new NotImplementedException();
        }

        protected override int Insert()
        {
            throw new NotImplementedException();
        }

        protected override void Delete()
        {
            throw new NotImplementedException();
        }

        static public Task Parse(SqlDataReader reader)
        {
            return new Task(
                (string)reader["Name"],
                (DateTime)reader["StartDate"],
                DBFunctions.DateCast(reader, "EndDate"),
                (int)reader["MostLikelyEstDuration"],
                (int)reader["MaxEstDuration"],
                (int)reader["MinEstDuration"],
                DBFunctions.StringCast(reader, "Description"),
                (int)reader["TaskId"]);
        }
        #endregion
    }
}
