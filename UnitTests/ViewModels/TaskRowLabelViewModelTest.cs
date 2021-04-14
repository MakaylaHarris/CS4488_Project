using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartPert.Model;
using SmartPert.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitTests.lib;

namespace UnitTests.ViewModels
{
    [TestClass]
    public class TaskRowLabelViewModelTest
    {
        private static List<Task> tasks;
        private static Project project;
        private TaskRowLabelViewModel viewModel;

        [ClassInitialize]
        public static void init(TestContext context) {
            project = new InitModel(2, 2, 2).Project;
            tasks = project.SortedTasks.ToList();
            InitModel.Link_Dependencies(new List<Task> { tasks[2], tasks[3] });
            InitModel.Link_Dependencies(new List<Task> { tasks[1], tasks[4] });
        }

        [TestMethod]
        public void TestCanShiftRight()
        {
            viewModel = new TaskRowLabelViewModel(tasks.Last());
            Assert.IsTrue(viewModel.CanShiftRight());
        }

        [TestMethod]
        public void TestCanShiftLeft()
        {
            viewModel = new TaskRowLabelViewModel(tasks[1]);
            Assert.IsTrue(viewModel.CanShiftLeft());
        }

        [TestMethod]
        public void TestCannotShiftRightDependent()
        {
            viewModel = new TaskRowLabelViewModel(tasks[4]);
            Assert.IsFalse(viewModel.CanShiftRight());
        }


        [TestMethod]
        public void TestCannotShiftLeftWithNoParent()
        {
            Assert.IsFalse(new TaskRowLabelViewModel(tasks[0]).CanShiftLeft());
        }

    }
}
