using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SmartPert.Model;
using SmartPert.View;
using PertTest.DAL;

namespace PertTest.Model
{
    [TestClass]
    public class ModelTest : IViewModel
    {
        private SmartPert.Model.IModel model;
        public ModelTest()
        {
            model = SmartPert.Model.Model.GetInstance(this);
        }

        [ClassInitialize]
        public static void init(TestContext context)
        {
            new TestDB(new List<string> { "project_foo.sql" });
            
        }

        [TestMethod]
        public void Test_GetFiveTasks()
        {
            // Setup
            model.SetProject(model.GetProjectList()[0]);
            Assert.AreEqual(model.GetTasks().Count, 5);
        }

        [TestMethod]
        public void Test_GetFooProject()
        {
            List<Project> projects = model.GetProjectList();
            Assert.IsTrue(projects.Count == 1);
            Assert.IsTrue(projects[0].Name == "Foo");
        }

        [TestMethod]
        public void Test_GetUserJoe()
        {
            List<User> users = model.GetUsers();
            Assert.IsNotNull(users.Find(x => x.Username == "Joe"));
        }

        #region Interface Methods

        public void OnDisconnect()
        {
            throw new NotImplementedException();
        }

        public void OnModelUpdate(Project p)
        {
            Console.WriteLine("Update received");
        }


        public bool SetConnectionString(string s)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
