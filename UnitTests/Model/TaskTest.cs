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
