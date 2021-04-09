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
    public class EditTaskCmdTest
    {
        private static Project project;
        private static List<Task> tasks;

        [ClassInitialize]
        public static void init(TestContext context)
        {
            project = new InitModel(3).Project;
            tasks = project.SortedTasks.ToList();
            InitModel.Link_Dependencies(tasks);
            tasks[0].LikelyDuration = 5;
            tasks[0].MaxDuration = 8;
        }
        
        [TestMethod]
        public void TestEditStartDate()
        {
            DateTime startdate = tasks[0].StartDate;
            new EditTaskCmd(tasks[0], tasks[0].Name, startdate.AddDays(5), null, tasks[0].LikelyDuration, tasks[0].MaxDuration, tasks[0].MinDuration, tasks[0].Description).Run();
            Assert.AreEqual(5, (tasks[0].StartDate - startdate).Days);
            CommandStack.Instance.Undo();
            Assert.AreEqual(startdate, tasks[0].StartDate);
        }

        [TestMethod]
        public void TestEditStartDateOfDependent()
        {
            Task task = tasks[1];
            DateTime startdate = task.StartDate;
            new EditTaskCmd(task, task.Name, startdate.AddDays(5), null, task.LikelyDuration, task.MaxDuration, task.MinDuration, task.Description).Run();
            Assert.AreEqual(5, (task.StartDate - startdate).Days);
            CommandStack.Instance.Undo();
            Assert.AreEqual(startdate, task.StartDate);
        }
    }
}
