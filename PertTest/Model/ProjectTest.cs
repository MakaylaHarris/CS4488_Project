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
            Project p = null;
            while(reader.Read())
            {
                p = Project.Parse(reader, null);
            }
            CloseConnection();
            return p;
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
            Project p = new Project(name, start, end, description);
            Assert.IsNotNull(GetProjectByName(name));
            // Test delete
            p.Delete();
            Assert.IsNull(GetProjectByName(name));
        }
        #endregion

        #region Interface Methods
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
        #endregion
    }
}
