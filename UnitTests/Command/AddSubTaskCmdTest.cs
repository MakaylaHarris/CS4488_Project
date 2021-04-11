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
    public class AddSubTaskCmdTest
    {
        private static int subtasksPerTask;
        private static int numTasks;
        private static int maxSubTaskLevel;
        private static Project project;
        private static List<Task> tasks;


        class AddSubTaskExposed : AddSubTaskCmd
        {
            public AddSubTaskExposed(Task parent, Task subtask) : base(parent, subtask){}

            public List<Task> Reorder(Task toMove, Task parent) => ReorderRows(toMove, parent);
            public bool CanAddSubTaskRow() => CanAddSubtaskAtRow();
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            // Setup project with tasks
            numTasks = 3;
            subtasksPerTask = 2;
            maxSubTaskLevel = 3;
            project = new InitModel(numTasks, subtasksPerTask, maxSubTaskLevel).Project;
            tasks = project.SortedTasks.ToList();
            Assert.AreEqual(45, project.Tasks.Count);
            for (int i = 0; i < tasks.Count; i++)
                Assert.AreEqual(i, tasks[i].ProjectRow);
        }

        [TestMethod]
        public void TestReorderRowsWithinParent()
        {
            Task parent = tasks[0], subtask = tasks[2];
            var cmd = new AddSubTaskExposed(parent, subtask);
            List<Task> reordered = cmd.Reorder(subtask, parent);
            // subtask should be 3 from the end of the subtask group
            Task sub = reordered[parent.CountAllSubtasks() - 2];
            Assert.AreEqual(subtask, sub);
        }

        [TestMethod]
        public void TestReorderRowsAboveParent()
        {
            Task parent = null, subtask = tasks[0];
            for (int i = tasks.Count - 1; i >= 0; i--)
                if (tasks[i].ParentTask == null)
                {
                    parent = tasks[i];
                    break;
                }
            var reordered = new AddSubTaskExposed(parent, subtask).Reorder(subtask, parent);
            Assert.AreEqual(subtask, reordered[tasks.Count - subtask.CountAllSubtasks() - 1]);
        }

        [TestMethod]
        public void TestReorderRowsBelowParent()
        {
            Task parent = tasks[0], subtask = tasks[33];
            var reordered = new AddSubTaskExposed(parent, subtask).Reorder(subtask, parent);
            Assert.AreEqual(subtask, reordered[parent.CountAllSubtasks() + 1]);
        }


        [TestMethod]
        public void TestCanAddSubtaskWithinGroupAtEnd()
        {
            Task parent = tasks[0], subtask = tasks[5];
            var cmd = new AddSubTaskExposed(parent, subtask);
            Assert.IsTrue(cmd.CanAddSubTaskRow());
        }

        [TestMethod]
        public void TestCanAddSubtaskOneAfterGroup()
        {
            Task parent = tasks[0], subtask = tasks[15];
            Assert.IsTrue(new AddSubTaskExposed(parent, subtask).CanAddSubTaskRow());
        }

        [TestMethod]
        public void TestCannotAddInGroupMiddle()
        {
            Task parent = tasks[0], subtask = tasks[3];
            Assert.IsFalse(new AddSubTaskExposed(parent, subtask).CanAddSubTaskRow());
        }

        [TestMethod]
        public void TestCannotAddAfterOrBefore()
        {
            Assert.IsFalse(new AddSubTaskExposed(tasks[0], tasks[24]).CanAddSubTaskRow());
            Assert.IsFalse(new AddSubTaskExposed(tasks.FindLast(x => x.ParentTask == null), tasks[13]).CanAddSubTaskRow());
        }

        [TestMethod]
        public void TestExecuteWithReorder()
        {
            Task parent = tasks[0];
            Task subtask = InitModel.Init_Tasks(project, baseName: "Fox")[0];
            List<Task> currentOrder = project.SortedTasks;
            int prevRow = subtask.ProjectRow;
            new AddSubTaskCmd(parent, subtask).Run();
            Assert.IsTrue(subtask.ParentTask == parent);
            Assert.IsTrue(subtask.ProjectRow <= parent.CountAllSubtasks());

            // Undo
            CommandStack.Instance.Undo();
            Assert.IsTrue(subtask.ProjectRow == prevRow);
            Assert.IsTrue(currentOrder.SequenceEqual(parent.Project.SortedTasks));
        }

        [TestMethod]
        public void TestExecuteNoReorder()
        {
            Task parent = tasks[0], subtask = tasks[42];
            int prevRow = subtask.ProjectRow;
            new AddSubTaskCmd(parent, subtask).Run();
            Assert.IsTrue(subtask.ParentTask == parent);
            Assert.IsTrue(subtask.ProjectRow <= parent.CountAllSubtasks());

            // undo
            CommandStack.Instance.Undo();
            Assert.IsTrue(subtask.ProjectRow == prevRow);
        }
    }
}
