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

        public Project(string name, DateTime start, DateTime? end, string description = "", int id = -1) : base(name, start, end, description, id)
        {
            tasks = new List<Task>();
        }

        #region Task Methods
        public void AddTask(Task t)
        {
            tasks.Add(t);
        }

        public void RemoveTask(Task t)
        {
            tasks.Remove(t);
        }
        #endregion

        #region Workers
        public override void AddWorker(User worker)
        {
            throw new NotImplementedException();
        }

        public override void RemoveWorker(User worker)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Database
        public override void Delete()
        {
            ExecuteSql("Delete from Project Where ProjectId= " + Id + ";");
        }

        protected override int Insert()
        {
            return ExecuteSql("insert into Project(Name, Description, StartDate, WorkingHours, ProjectOwner) output INSERTED.ProjectId values('"
                   + Name + "', '" + Description + "', '" + StartDate + "', '" + EndDate + "');");
        }

        protected override void Update()
        {
            ExecuteSql("update Project set Name = '" + Name
                + "', Description = '" + Description +
                "', StartDate='" + StartDate +
                "', EndDate='" + EndDate +
                "Where ProjectId=" + Id + ";");
        }

        static public Project Parse(SqlDataReader reader)
        {
            return new Project(
                (string)reader["Name"],
                (DateTime)reader["StartDate"],
                DBFunctions.DateCast(reader, "EndDate"),
                DBFunctions.StringCast(reader, "Description"),
                (int)reader["ProjectId"]);
        }

        #endregion
    }
}
