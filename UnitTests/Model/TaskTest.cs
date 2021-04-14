using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartPert.Model;
using System;
using System.Collections.Generic;
using UnitTests.lib;

namespace UnitTests.Model
{
    /// <summary>
    /// Task Unit Tests
    /// Created 4/4/2021 by Robert Nelson
    /// </summary>
    /// 

    [TestClass]
    public class CanAddSubTaskTest
    {
        private static List<Task> tasks;

        [ClassInitialize]
        public static void init(TestContext c)
        {
            tasks = new InitModel(2, 2, 2).Project.SortedTasks;
            InitModel.Link_Dependencies(new List<Task> { tasks[1], tasks[4] });
        }

        [TestMethod]
        public void TestCannotAddAncestor()
        {
            // Setup
            Task ancestor = tasks[0];
            Task task = tasks[2];

            Assert.IsFalse(task.CanAddSubTask(ancestor));
        }

        [TestMethod]
        public void TestCannotAddDependentSubTask()
        {
            Task parent = tasks[1];
            Task task = tasks[4];

            Assert.IsFalse(parent.CanAddSubTask(task));
        }

        [TestMethod]
        public void TestCanAddSubTask()
        {
            Task parent = tasks[2];
            Task task = tasks[3];
            Assert.IsTrue(parent.CanAddSubTask(task));
        }
    }
}
