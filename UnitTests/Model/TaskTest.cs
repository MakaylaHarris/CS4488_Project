using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartPert.Model;
using System;

namespace UnitTests.Model
{
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
