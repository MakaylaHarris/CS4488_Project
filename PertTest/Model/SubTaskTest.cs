using Microsoft.VisualStudio.TestTools.UnitTesting;
using PertTest.DAL;
using SmartPert.Command;
using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PertTest.Model
{
    /// <summary>
    /// Summary description for SubTaskTest
    /// Created 3/28/2021 by Robert Nelson
    /// </summary>
    [TestClass]
    public class SubTaskTest
    {
        private Project project;
        public SubTaskTest()
        {
            new TestDB(new List<string> { "project_boat.sql" });
            SmartPert.Model.Model model = SmartPert.Model.Model.Instance;
            model.SetProject(model.GetProjectList()[0]);
            project = model.GetProject();
        }

        #region Test Methods
        [TestMethod]
        public void Test_GetSubtasks()
        {
            Task task = project.SortedTasks.Find(x => x.Name == "Build Frame");
            Assert.IsTrue(task.Tasks.Count > 0);
            foreach (Task t in task.Tasks)
                Assert.AreEqual(t.ParentTask, task);
        }

        [TestMethod]
        public void Test_AddSubtaskShiftsParentStartDate()
        {
            Task task = project.SortedTasks.Find(x => x.Name == "Water Test");
            Task sub = project.SortedTasks.Find(x => x.Name == "Acquire Plans");
            DateTime endEstimated = task.MaxEstDate;
            task.AddSubTask(sub);

            // Tests
            Assert.IsTrue(task.SubTasks.Contains(sub));
            Assert.IsTrue(task.StartDate <= sub.StartDate);
            Assert.IsTrue(endEstimated <= task.MaxEstDate);
        }

        [TestMethod]
        public void Test_AddSubtaskShiftsParentsEstimates()
        {
            Task task = project.GetTask("Build Frame");
            Task sub = project.GetTask("Seal with Glue");
            task.AddSubTask(sub);

            Assert.IsTrue(task.MaxEstDate == sub.MaxEstDate);
            Assert.IsTrue(task.LikelyDate == sub.LikelyDate);
        }

        [TestMethod]
        public void Test_UndoAddSubtaskShiftsParentBack()
        {
            Task task = project.GetTask("Build Frame");
            DateTime prevLikely = task.LikelyDate;
            Task sub = project.GetTask("Seal with Glue");
            ICmd cmd = new AddSubTaskCmd(task, sub);
            cmd.Run();
            CommandStack.Instance.Undo();
            Assert.AreEqual(prevLikely, task.LikelyDate);
        }

        [TestMethod]
        public void Test_RemoveSubtask()
        {
            Task task = project.GetTask("Build Frame");
            Task sub = task.SubTasks[0];
            task.RemoveSubTask(sub);

            Assert.IsNull(sub.ParentTask);
            Assert.IsFalse(task.SubTasks.Contains(sub));
        }
        #endregion
    }
}
