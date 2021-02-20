﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace PertTest.Model
{
    [TestClass]
    public class TaskTest : IDBItem
    {

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


        [TestMethod]
        public void TestAddUpdateDelete()
        {
            // SETUP
            string name = "Test32312213";
            Project project = DB_GetProject();
            // Ensure task isn't in database
            Task task = DB_ReadTask(name, project);
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
            task.Delete();
            Assert.IsFalse(DB_HasTask(name));
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        protected override int Insert()
        {
            throw new NotImplementedException();
        }

        protected override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
