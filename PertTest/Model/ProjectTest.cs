using Microsoft.VisualStudio.TestTools.UnitTesting;
using PertTest.DAL;
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
        private Project project;

        #region Constructor
        public ProjectTest() : base()
        {
            name = "Project 1123462123";
            description = "Test project";
            start = DateTime.Now;
            end = start.AddDays(30);
        }

        [ClassInitialize]
        public static void init(TestContext context)
        {
            new TestDB(new List<string> { "project_foo.sql" });
        }
        #endregion

        #region Private Methods
        private Project GetProjectByName(string name)
        {
            using (var conn = new DBConnection("SELECT * FROM Project WHERE Name=@Name"))
            {
                SqlCommand command = conn.Command;
                command.Parameters.AddWithValue("@Name", name);
                SqlDataReader reader = command.ExecuteReader();
                project = null;
                while (reader.Read())
                {
                    project = Project.Parse(reader, null);
                }
            }
            return project;
        }
        #endregion

        #region Test Methods
        /// <summary>
        /// Test create and delete of project
        /// Created 2/20/2021 by Robert Nelson
        /// </summary>
        [TestMethod]
        public void Test_CreateDelete()
        {
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
