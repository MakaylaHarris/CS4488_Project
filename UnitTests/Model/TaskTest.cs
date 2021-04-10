using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartPert.Model;
using System;
using System.Collections.Generic;

namespace UnitTests.Model
{
    /// <summary>
    /// Task Unit Tests
    /// Created 4/4/2021 by Robert Nelson
    /// </summary>
    /// 
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

    [TestClass]
    public class CanAddDependencyTest
    {
        [TestMethod]
        public void TestCannotAddSubtask()
        {
            Task task = new Task("Foo", DateTime.Now, null, 5, insert: false, track: false, id: -5);
            Task parent = new Task("Bar", DateTime.Now, null, 5, insert: false, track: false, id: -4);
            parent.AddSubTask(task);
            Assert.IsFalse(parent.CanAddDependency(task));
            Assert.IsFalse(task.CanAddDependency(parent));
        }

        [TestMethod]
        public void TestCannotCreateCircularDependents()
        {
            Task task = new Task("Foo", DateTime.Now, null, 5, insert: false, track: false, id: -5);
            Task parent = new Task("Bar", DateTime.Now, null, 5, insert: false, track: false, id: -4);
            Task t = new Task("Foxo", DateTime.Now, null, 5, insert: false, track: false, id: -3);
            parent.AddDependency(task);
            task.AddDependency(t);

            Assert.IsFalse(t.CanAddDependency(parent));
            Assert.IsFalse(parent.CanAddDependency(t));
        }

        [TestMethod]
        public void TestCanAddNew()
        {
            Task task = new Task("Foo", DateTime.Now, null, 5, insert: false, track: false, id: -5);
            Task parent = new Task("Bar", DateTime.Now, null, 5, insert: false, track: false, id: -4);
            Assert.IsTrue(parent.CanAddDependency(task));
        }
    }

    [TestClass]
    public class CanAddSubTaskTest
    {
        [TestMethod]
        public void TestCannotAddAncestor()
        {
            // Setup
            Task ancestor = new Task("Ancestor", DateTime.Now, null, 5, insert: false, track: false);
            Task parent = new Task("Parent", DateTime.Now, null, 5, insert: false, track: false);
            ancestor.AddSubTask(parent);
            Task task = new Task("Foo", DateTime.Now, null, 5, insert: false, track: false);
            parent.AddSubTask(task);

            Assert.IsFalse(task.CanAddSubTask(ancestor));
        }

        [TestMethod]
        public void TestCannotAddDependentSubTask()
        {
            Task parent = new Task("Parent", DateTime.Now, null, 5, insert: false, track: false, id: -5);
            Task task = new Task("Foo", DateTime.Now, null, 5, insert: false, track: false, id: -4);
            parent.AddDependency(task);

            Assert.IsFalse(parent.CanAddSubTask(task));
        }

        [TestMethod]
        public void TestCanAddSubTask()
        {
            Task parent = new Task("Parent", DateTime.Now, null, 5, insert: false, track: false);
            Task task = new Task("Foo", DateTime.Now, null, 5, insert: false, track: false);
            Assert.IsTrue(parent.CanAddSubTask(task));
        }
    }
}
