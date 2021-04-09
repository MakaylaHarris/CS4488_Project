using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartPert.Command;
using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitTests.lib;

namespace UnitTests.Command
{
    [TestClass]
    public class RemoveDependencyCmdTest
    {
        private static Project project;
        private static List<Task> tasks;
 
        [ClassInitialize]
        public static void init(TestContext context)
        {
            project = new InitModel(2).Project;
            tasks = project.SortedTasks.ToList();
            InitModel.Link_Dependencies(tasks);
        }

        [TestMethod]
        public void TestRemoveDepShiftsStartDate()
        {
            Task dep = tasks[1], parent = tasks[0];
            dep.StartDate = parent.StartDate;
            DateTime initial = dep.StartDate;
            Assert.IsTrue(new RemoveDependencyCmd(parent, dep).Run());
            Assert.AreNotEqual(initial, dep.StartDate);
            CommandStack.Instance.Undo();
            Assert.AreEqual(initial, dep.StartDate);
        }
    }
}
