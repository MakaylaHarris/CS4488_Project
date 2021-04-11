using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitTests.lib;

namespace UnitTests.Model
{
    [TestClass]
    public class CanAddDependencyTest
    {
        private static List<Task> tasks;

        [ClassInitialize]
        public static void init(TestContext context)
        {
            tasks = new InitModel(3, 2, 1).Project.SortedTasks;
            InitModel.Link_Dependencies(new List<Task> { tasks[0], tasks[3] });
            InitModel.Link_Dependencies(new List<Task> { tasks[4], tasks[6] });
        }

        [TestMethod]
        public void TestSubtaskCanAddDependentSubtask()
        {
            Assert.IsTrue(tasks[1].CanAddDependency(tasks[2]));
        }

        [TestMethod]
        // When a task is dependent on a subtask, it should be able to add a dependency to a different subtask in that group
        public void TestCanAddDependencyToDifferentSubtask()
        {
            Assert.IsTrue(tasks[6].CanAddDependency(tasks[5]));
        }

        [TestMethod]
        public void TestCannotAddCircularDependencyToSubtask()
        {
            Task dependent = tasks[3], subtask = tasks[1];
            Assert.IsFalse(dependent.CanAddDependency(subtask));
        }

        [TestMethod]
        public void TestCannotAddDependencyToSubtask()
        {
            Task parent = tasks[0], task = tasks[1];
            Assert.IsFalse(parent.CanAddDependency(task));
            Assert.IsFalse(task.CanAddDependency(parent));
        }

        [TestMethod]
        public void TestCannotCreateCircularDependents()
        {
            Task parent = tasks[0], t = tasks[3];
            Assert.IsFalse(t.CanAddDependency(parent));
            Assert.IsFalse(parent.CanAddDependency(t));
        }

        [TestMethod]
        public void TestCanAddNew()
        {
            Task parent = tasks[0], task = tasks[6];
            Assert.IsTrue(parent.CanAddDependency(task));
        }
    }

    [TestClass]
    public class DependencyDateTest
    {
        private Task task;
        private Task parent;

        [TestInitialize]
        public void Initialize()
        {
            task = new Task("Foo", DateTime.Now, null, 5, insert: false, track: false);
            parent = new Task("Bar", DateTime.Now, null, 5, 8, insert: false, track: false);
            parent.AddDependency(task);
        }

        [TestMethod]
        public void TestDependentStartDate()
        {
            int expected = Task.CalculateDependentsMaxEstimate ? 8 : 5;
            Assert.AreEqual(expected, (task.StartDate - parent.StartDate).Days);
        }

        [TestMethod]
        public void TestEstimateChangeShiftsDependencyForward()
        {
            parent.LikelyDuration = parent.LikelyDuration + 5;
            Assert.AreEqual(10, (task.StartDate - parent.StartDate).Days);
        }

        [TestMethod]
        public void TestMovingStartDateForward()
        {
            DateTime prev = task.StartDate;
            parent.StartDate = parent.StartDate.AddDays(5);
            Assert.AreEqual(5, (task.StartDate - prev).Days);
        }

        [TestMethod]
        public void TestSwitchingToMaxEstimation()
        {
            Task.CalculateDependentsMaxEstimate = true;
            Assert.AreEqual(8, (task.StartDate - parent.StartDate).Days);
        }

        [TestMethod]
        public void TestSettingEndDateMovesDependentEarlier()
        {
            parent.IsComplete = true;
            Assert.AreEqual(0, (task.StartDate - (DateTime)parent.EndDate).Days);
        }
    }


}
