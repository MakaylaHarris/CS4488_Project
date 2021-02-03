using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PERT.Model
{
    /// <summary>
    /// A Task is a single item on the Pert chart that has a most likely duration, minimum duration, and maximum duration.
    /// Dependencies are Tasks that cannot be started until this one is finished.
    /// Created 1/29/2021 by Robert Nelson
    /// </summary>
    public class Task : TimedItem
    {
        private uint mostLikelyDuration;
        private uint maxDuration;
        private uint minDuration;
        private List<Task> dependencies;

        public Task(string name, DateTime start, DateTime? end, uint duration, uint maxDuration = 0, uint minDuration = 0, string description = "", int id = -1) : base(name, start, end, description, id)
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
                (uint)reader["MostLikelyEstDuration"],
                (uint)reader["MaxEstDuration"],
                (uint)reader["MinEstDuration"],
                DBFunctions.StringCast(reader, "Description"),
                (int)reader["TaskId"]);
        }
        #endregion
    }
}
