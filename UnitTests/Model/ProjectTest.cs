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
    public class ProjectTest : Project
    {
        private static Project project;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            project = new InitModel(3, 2, 2).Project;
        }


        public ProjectTest() : base("Foo", DateTime.Now, null, insert: false, track: false)
        {

        }

        [TestMethod]
        public void TestIsValidSort()
        {
            Assert.IsTrue(Project.IsValidSort(project.SortedTasks));
        }

        [TestMethod]
        public void TestIsInvalidSort()
        {
            List<Task> sorted = project.SortedTasks.ToList();
            Task temp = sorted[2];
            sorted[2] = sorted[4];
            sorted[4] = temp;
            Assert.IsFalse(Project.IsValidSort(sorted));
        }

        [TestMethod]
        public void TestReorderRows()
        {
            List<Task> sorted = project.SortedTasks.ToList();
            Task temp = sorted[2];
            sorted[2] = sorted[4];
            sorted[4] = temp;
            Assert.IsTrue(Project.IsValidSort(Project.ResortTasks(sorted)));
        }

    }
}
