using Microsoft.VisualStudio.TestTools.UnitTesting;
using PertTest.DAL;
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
    public class TaskTest
    {
        private Task task;
        private SmartPert.Model.Model model;
        private Project project;

        #region Constructor
        public TaskTest() : base()
        {
            new TestDB(new List<string> { "project_foo.sql" });
            model = SmartPert.Model.Model.Instance;
            project = model.GetProjectList().Find(x => x.Name == "Foo");
            model.SetProject(project);
            if (project.SortedTasks.Count <= 0)
                Console.WriteLine("No Tasks!");
            task = project.SortedTasks[0];
        }

        #endregion

        #region Test Methods

        [TestMethod]
        public void TestAddRemoveWorker()
        {
            User worker = model.GetUsers()[0];
            // TEST
            task.AddWorker(worker);
            DBReader.Instance.OnDBUpdate();
            Assert.IsTrue(task.Workers.Contains(worker));
            task.RemoveWorker(worker);
            DBReader.Instance.OnDBUpdate();
            Assert.IsFalse(task.Workers.Contains(worker));
        }

        [TestMethod]
        public void TestInsert()
        {
            // SETUP
            string name = "Test32312213";

            // Insert the task into database
            Task newTask = new Task(name, DateTime.Now, null, 5, project: project);
            DBReader.Instance.OnDBUpdate();
            Assert.IsNotNull(model.GetTaskById(newTask.Id));
        }

        [TestMethod]
        public void TestDelete()
        {
            Task t = model.GetTasks()[1];
            // Delete Task
            t.Delete();
            DBReader.Instance.OnDBUpdate();
            Assert.IsNull(model.GetTaskById(t.Id));

        }

        [TestMethod]
        public void TestUpdate()
        {
            // Update the task
            string description = "Something interesting";
            task.Description = description;
            DBReader.Instance.OnDBUpdate();
            Assert.IsTrue(description == task.Description);
        }

        [TestMethod]
        public void TestAddSubTask()
        {
            Task sub = model.GetTasks()[1];
            // Add sub
            task.AddSubTask(sub);
            Assert.IsTrue(task.Tasks.Contains(sub));
            DBReader.Instance.OnDBUpdate();
            Assert.IsTrue(task.Tasks.Contains(sub));
            // Test that task delete removes sub
            sub.Delete();
            Assert.IsFalse(task.Tasks.Contains(sub));
            DBReader.Instance.OnDBUpdate();
            Assert.IsFalse(task.Tasks.Contains(sub));
        }
        #endregion

    }
}
