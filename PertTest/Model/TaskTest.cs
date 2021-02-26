using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace PertTest.Model
{
    /// <summary>
    /// Tests for tasks
    /// Created 2/19/2021 by Robert Nelson
    /// </summary>
    [TestClass]
    public class TaskTest : IDBItem
    {
        private string name;
        private DateTime start;
        private int estimatedDuration;
        private Task task;
        private bool isCreated;
        private Project project;
        private ProjectTest projectTest;

        #region Constructor
        public TaskTest()
        {
            name = "TASK_3361234";
            start = DateTime.Now;
            estimatedDuration = 5;
            isCreated = false;
            projectTest = new ProjectTest();
            project = null;
        }

        ~TaskTest()
        {
            if(isCreated)
            {
                task.Delete();
            }
        }
        #endregion

        #region Private Methods
        private bool DB_HasTask(string taskname)
        {
            SqlCommand command = OpenConnection("SELECT COUNT(*) FROM dbo.[Task] WHERE Name=@Name");
            command.Parameters.AddWithValue("@Name", taskname);
            int result = (int) command.ExecuteScalar();
            CloseConnection();
            return result > 0;
        }

        private Task DB_ReadTask(string taskName, Project p)
        {
            SqlCommand command = OpenConnection("SELECT * FROM dbo.[Task] WHERE Name=@Name");
            command.Parameters.AddWithValue("@Name", taskName);
            SqlDataReader reader = command.ExecuteReader();
            Task ret = null;
            Dictionary<int, Project> keyValuePairs = new Dictionary<int, Project>();
            keyValuePairs.Add(p.Id, p);
            if (reader.Read())
                ret = Task.Parse(reader, null, keyValuePairs);
            CloseConnection();
            return ret;
        }

        private Project DB_GetProject()
        {
            string query = "SELECT TOP 1 * FROM dbo.Project";
            SqlCommand command = OpenConnection(query);
            SqlDataReader reader = command.ExecuteReader();
            Assert.IsTrue(reader.Read());
            Project ret = Project.Parse(reader, null);
            return ret;
        }
        #endregion

        #region Public Methods
        public Task Create(Project project = null)
        {
            if (project == null)
                project = projectTest.Create();
            this.project = project;
            if (!isCreated)
            {
                task = DB_ReadTask(name, project);
                if(task == null)
                    task = new Task(name, DateTime.Now, null, estimatedDuration, project: project);
                isCreated = true;
                this.project = project;
            }
            else // Better be on the same project
                Assert.IsTrue(project == task.Proj);
            return task;
        }

        [TestMethod]
        public void TestAddRemoveWorker()
        {
            // SETUP
            Create();
            UserTest test = new UserTest();
            User worker = test.Create();

            // TEST
            task.AddWorker(worker);
            Assert.IsTrue(task.Workers.Contains(worker));
            task.RemoveWorker(worker);
            Assert.IsFalse(task.Workers.Contains(worker));
        }

        [TestMethod]
        public void TestAddUpdateDelete()
        {
            // SETUP
            string name = "Test32312213";
            Project project = projectTest.Create();
            // Ensure task isn't in database
            task = DB_ReadTask(name, project);
            if (task != null)
                task.Delete();

            // Insert the task into database
            task = new Task(name, DateTime.Now, null, 5, project: project);
            Assert.IsTrue(DB_HasTask(name));

            // Update the task
            string description = "Something interesting";
            task.Description = description;
            Task updated = DB_ReadTask(task.Name, project);
            Assert.IsTrue(task.Description == updated.Description);

            // Delete Task
            updated.Delete();
            Assert.IsFalse(DB_HasTask(name));
        }
        #endregion

        #region Interface Methods
        protected override void PerformDelete()
        {
            throw new NotImplementedException();
        }

        protected override int PerformInsert()
        {
            throw new NotImplementedException();
        }

        protected override void PerformUpdate()
        {
            throw new NotImplementedException();
        }

        public override bool PerformParse(SqlDataReader reader)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
