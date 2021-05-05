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
    public class AddDependencyCmdTest
    {
        private static List<Task> tasks;
        private static Project project;

        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            project = new InitModel(2).Project;
            tasks = project.SortedTasks.ToList();
        }


        public void TestAddDependencyShiftsStartDate()
        {
            Task t = tasks[1];
            t.StartDate = tasks[0].StartDate;
            DateTime initial = t.StartDate;
            Assert.IsTrue(new AddDependencyCmd(tasks[0], t).Run());
            Assert.AreNotEqual(initial, t.StartDate);
            CommandStack.Instance.Undo();
            Assert.AreEqual(initial, t.StartDate);
        }

    }
}
