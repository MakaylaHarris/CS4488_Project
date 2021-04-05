using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartPert.Model;
using System;

namespace UnitTests.Model
{
    /// <summary>
    /// Task Unit Tests
    /// Created 4/4/2021 by Robert Nelson
    /// </summary>
    [TestClass]
    public class CanAddDependencyTest
    {
        [TestMethod]
        public void TestCannotAddSubtask()
        {
            Task task = new Task("Foo", DateTime.Now, null, 5, insert: false, track: false);
            Task parent = new Task("Bar", DateTime.Now, null, 5, insert: false, track: false);
            parent.AddSubTask(task);
            Assert.IsFalse(parent.CanAddDependency(task));
            Assert.IsFalse(task.CanAddDependency(parent));
        }

        [TestMethod]
        public void TestCannotCreateCircularDependents()
        {
            Task task = new Task("Foo", DateTime.Now, null, 5, insert: false, track: false);
            Task parent = new Task("Bar", DateTime.Now, null, 5, insert: false, track: false);
            Task t = new Task("Foxo", DateTime.Now, null, 5, insert: false, track: false);
            parent.AddDependency(task);
            task.AddDependency(t);

            Assert.IsFalse(t.CanAddDependency(parent));
            Assert.IsFalse(parent.CanAddDependency(t));
        }

        [TestMethod]
        public void TestCanAddNew()
        {
            Task task = new Task("Foo", DateTime.Now, null, 5, insert: false, track: false);
            Task parent = new Task("Bar", DateTime.Now, null, 5, insert: false, track: false);
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
            Task parent = new Task("Parent", DateTime.Now, null, 5, insert: false, track: false);
            Task task = new Task("Foo", DateTime.Now, null, 5, insert: false, track: false);
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
