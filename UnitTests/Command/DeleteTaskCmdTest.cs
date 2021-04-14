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
    public class DeleteTaskCmdTest
    {
        private static Project project;
        private static List<Task> tasks;

        [ClassInitialize]
        public static void init(TestContext context)
        {
            project = new InitModel(2, 2, 1).Project;
            tasks = project.SortedTasks.ToList();
        }

        [TestMethod]
        public void TestUndoRecreatesSubtasks()
        {
            // Create a task
            Task parent = InitModel.Init_Tasks(project, baseName: "FooWithSubtasks", initTaskCallback: new InitModel.UniformSubtaskCallback(2, 2).Callback)[0];
            Task task = parent.Tasks.First();
            // Run delete and undo
            var cmd = new DeleteTaskCmd(task);
            Assert.IsTrue(cmd.Run());
            CommandStack.Instance.Undo();
            SmartPert.Model.Model.Instance.UpdateTask(ref task);

            // Test
            Assert.IsTrue(task.SubTasks.Count == 2);
            Assert.IsNotNull(task.ParentTask);
        }

        [TestMethod]
        public void TestUndoRecreatesDependencies()
        {
            var tasklist = InitModel.Init_Tasks(project, 3, baseName: "FooDependencies ");
            InitModel.Link_Dependencies(tasklist);
            var t = tasklist[1];

            Assert.IsTrue(new DeleteTaskCmd(t).Run());
            CommandStack.Instance.Undo();
            SmartPert.Model.Model.Instance.UpdateTask(ref t);

            // Test 
            Assert.IsTrue(t.Dependencies.Contains(tasklist[2]));
            Assert.IsTrue(t.DependentOn.Contains(tasklist[0]));
        }
    }
}
