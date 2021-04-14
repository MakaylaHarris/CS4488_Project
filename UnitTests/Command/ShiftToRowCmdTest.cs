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
    public class ShiftToRowCmdTest
    {
        private static List<Task> tasks;
        
        [ClassInitialize]
        public static void init(TestContext context)
        {
            tasks = new InitModel(3, 3, 1).Project.SortedTasks.ToList();
            InitModel.Link_Dependencies(new List<Task> { tasks[2], tasks[3] });
        }

        [TestMethod]
        public void TestShiftDependentSubTaskUp()
        {
            int row = tasks[2].ProjectRow;
            new ShiftToRowCmd(tasks[3], row).Run();
            Assert.IsTrue(row == tasks[3].ProjectRow);
            CommandStack.Instance.Undo();
        }
    }
}
