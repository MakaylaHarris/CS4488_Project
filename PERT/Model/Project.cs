using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERT.Model
{
    public class Project : TimedItem
    {
        private List<Task> tasks;

        public Project(string name, DateTime start, DateTime end, string description = "", int id = -1) : base(name, start, end, description, id) {
            tasks = new List<Task>();
        }

        public void AddTask(Task t)
        {
            tasks.Add(t);
        }

        public void RemoveTask(Task t)
        {
            tasks.Remove(t);
        }

        protected override void Delete()
        {
            ExecuteSql("Delete from Project Where ProjectId= " + this.Id + ";");
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
                (DateTime)reader["EndDate"],
                (string)reader["Description"],
                (int)reader["ProjectId"]);
        }
    }
}
