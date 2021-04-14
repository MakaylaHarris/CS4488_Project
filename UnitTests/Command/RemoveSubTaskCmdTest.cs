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
    public class RemoveSubTaskCmdTest
    {
        private static Project project;
        private static List<Task> tasks;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            project = new InitModel(subtasksPerTask: 2, maxSubTaskLevel: 2).Project;
            tasks = project.SortedTasks.ToList();
        }

        [TestMethod]
        public void TestRemoveWithinGroup()
        {
            Task parent = tasks[1], subtask = tasks[2];
            new RemoveSubtaskCmd(parent, subtask).Run();
            Assert.IsNull(subtask.ParentTask);
            Assert.AreEqual(tasks.Count - 1, subtask.ProjectRow);
            CommandStack.Instance.Undo();
            Assert.AreEqual(2, subtask.ProjectRow);
            Assert.AreEqual(subtask.ParentTask, parent);
        }

        [TestMethod]
        public void TestRemoveLastInGroup()
        {
            Task parent = tasks[0];
            Task subtask = parent.Tasks.Last();
            new RemoveSubtaskCmd(parent, subtask).Run();
            int projectRow = subtask.ProjectRow;
            Assert.IsNull(subtask.ParentTask);
            CommandStack.Instance.Undo();
            Assert.IsTrue(projectRow == subtask.ProjectRow);
        }
    }
}
