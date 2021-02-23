using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace PertTest.Model
{
    [TestClass]
    public class ProjectTest : IDBItem
    {
        // some test data
        string name;
        string description;
        DateTime start;
        DateTime end;
        private bool isCreated;
        private Project project;

        #region Constructor
        public ProjectTest()
        {
            name = "Project 1123462123";
            description = "Test project";
            start = DateTime.Now;
            end = start.AddDays(30);
        }
        ~ProjectTest()
        {
            if (isCreated)
            {
                DeleteProjectByName(name);
                isCreated = false;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Use this to create a test project
        /// </summary>
        public Project Create()
        {
            if(!isCreated)
            {
                project = GetProjectByName(name);
                if(project == null)
                    project = new Project(name, start, end, description);
                isCreated = true;
            }
            return project;
        }
        #endregion

        #region Private Methods
        private Project GetProjectByName(string name)
        {
            SqlCommand command = OpenConnection("SELECT * FROM Project WHERE Name=@Name");
            command.Parameters.AddWithValue("@Name", name);
            SqlDataReader reader = command.ExecuteReader();
            project = null;
            while(reader.Read())
            {
                project = Project.Parse(reader, null);
            }
            CloseConnection();
            return project;
        }

        private void DeleteProjectByName(string name)
        {
            SqlCommand command = OpenConnection("DELETE FROM Project WHERE Name=@Name");
            command.Parameters.AddWithValue("@Name", name);
            try
            {
                command.ExecuteNonQuery();
            } catch(SqlException) { }
            CloseConnection();
        }

        /// <summary>
        /// Test create and delete of project
        /// Created 2/20/2021 by Robert Nelson
        /// </summary>
        [TestMethod]
        public void TestCreateDelete()
        {
            // Setup
            DeleteProjectByName(name);
            // Test it creates
            project = new Project(name, start, end, description);
            Assert.IsNotNull(GetProjectByName(name));
            // Test delete
            project.Delete();
            Assert.IsNull(GetProjectByName(name));
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
