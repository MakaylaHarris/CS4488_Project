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

        #region Project
        public Project(string name, DateTime start, DateTime? end, string description = "", 
            User creator = null, DateTime? creationTime = null, int id = -1) 
            : base(name, start, end, description, creator, creationTime, id)
        {
            tasks = new List<Task>();
            if (id == -1)
                id = Insert();
        }
        public Project(Project project, int id = -1) : base(project, id)
        {
            tasks = new List<Task>();
            foreach (Task t in project.tasks)
                tasks.Add(t);
        }
        #endregion

        #region Task Methods
        public void AddTask(Task t)
        {
            if(!tasks.Contains(t))
                tasks.Add(t);
        }

        public void RemoveTask(Task t) => tasks.Remove(t);
        #endregion

        #region Workers
        /// <summary>
        /// Adds the worker to the project
        /// Implemented 2/9/2021 by Robert Nelson
        /// </summary>
        /// <param name="worker">User on project to add</param>
        /// <param name="updateDB">Also adds the user to the database</param>
        public override void AddWorker(User worker, bool updateDB=true)
        {
            if(!workers.Contains(worker))
            {
                workers.Add(worker);
                if(updateDB)
                {
                    SqlCommand command = OpenConnection("INSERT INTO UserProject (UserName, ProjectId) VALUES(@username, @projectId);");
                    command.Parameters.AddWithValue("@username", worker.Username);
                    command.Parameters.AddWithValue("@projectId", this.Id);
                    command.ExecuteNonQuery();      // Todo: If error is thrown and it's because the primary key exists then ignore (stored procedure?)
                    CloseConnection();
                }
            }
        }

        /// <summary>
        /// Removes the worker from the project
        /// Implemented 2/9/2021 by Robert Nelson
        /// </summary>
        /// <param name="worker">User to remove</param>
        /// <param name="updateDB">Update the database</param>
        public override void RemoveWorker(User worker, bool updateDB=true)
        {
            if(workers.Remove(worker))
            {
                if(updateDB)
                {
                    SqlCommand command = OpenConnection("DELETE FROM UserProject WHERE ProjectId = @projectId AND UserName = @username);");
                    command.Parameters.AddWithValue("@projectId", Id);
                    command.Parameters.AddWithValue("@username", worker.Username);
                    command.ExecuteNonQuery();      // Todo: ignore if the user is already removed
                    CloseConnection();
                }
            }
        }

        #endregion

        #region Database
        public override void Delete()
        {
            ExecuteSql("Delete from Project Where ProjectId= " + Id + ";");
        }

        protected override int Insert()
        {
            // todo: fix this hot mess
            int id = ExecuteSql("insert into Project(Name, Description, StartDate, WorkingHours, ProjectOwner) output INSERTED.ProjectId values('"
                   + Name + "', '" + Description + "', '" + StartDate + "', '" + EndDate + "');");
            return id;
        }

        protected override void Update()
        {
            // todo: fix this hot mess
            ExecuteSql("update Project set Name = '" + Name
                + "', Description = '" + Description +
                "', StartDate='" + StartDate +
                "', EndDate='" + EndDate +
                "'Where ProjectId=" + Id + ";");
            Model.Instance.OnModelUpdate(this);
        }

        static public Project Parse(SqlDataReader reader, List<User> users)
        {
            string creator = DBFunctions.StringCast(reader, "Creator");
            User user = users != null && creator != "" ? users.Find(x => x.Username == creator) : null;
            return new Project(
                (string)reader["Name"],
                (DateTime)reader["StartDate"],
                DBFunctions.DateCast(reader, "EndDate"),
                DBFunctions.StringCast(reader, "Description"),
                user,
                DBFunctions.DateCast(reader, "CreationDate"),
                (int)reader["ProjectId"]); ;
        }

        #endregion
    }
}
