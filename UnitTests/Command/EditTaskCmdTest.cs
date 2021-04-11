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
            project = new InitModel(3, 1, 1).Project;
            tasks = project.SortedTasks.ToList();
            InitModel.Link_Dependencies(new List<Task> { tasks[0], tasks[2] });
            tasks[0].LikelyDuration = 5;
            tasks[0].MaxDuration = 8;
        }
        
        [TestMethod]
        public void TestMarkCompleteEarlyEndChangesStartDate()
        {
            Task toEdit = tasks[4];
            new EditTaskCmd(toEdit, end: toEdit.StartDate.AddDays(-3)).Run();
            Assert.IsTrue(toEdit.EndDate >= toEdit.StartDate);
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
            Task task = tasks[2];
            DateTime startdate = task.StartDate;
            new EditTaskCmd(task, task.Name, startdate.AddDays(5), null, task.LikelyDuration, task.MaxDuration, task.MinDuration, task.Description).Run();
            Assert.AreEqual(5, (task.StartDate - startdate).Days);
            CommandStack.Instance.Undo();
            Assert.AreEqual(startdate, task.StartDate);
        }

        [TestMethod]
        public void TestEditStartDateOfParentTask()
        {
            Task parent = tasks[0], subtask = tasks[1];
            subtask.StartDate = parent.StartDate;
            DateTime start = subtask.StartDate;
            new EditTaskCmd(parent, parent.Name, start.AddDays(5), null, parent.LikelyDuration, parent.MaxDuration, parent.MinDuration, parent.Description).Run();
            Assert.AreEqual(5, (subtask.StartDate - start).Days);
            CommandStack.Instance.Undo();
            Assert.AreEqual(start, subtask.StartDate);
        }
    }
}
