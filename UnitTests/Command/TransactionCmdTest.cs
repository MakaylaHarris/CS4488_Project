using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartPert.Command;
using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.Command
{
    /// <summary>
    /// TransactionCmd Tests
    /// Created 4/4/2021 by Robert Nelson
    /// </summary>
    [TestClass]
    public class TransactionCmdTest
    {
        public class NestedCmd : ICmd
        {
            private readonly ModelClass model;

            public NestedCmd(ModelClass model)
            {
                this.model = model;
            }

            public override bool Undo()
            {
                return true;
            }

            protected override bool Execute()
            {
                new AddOne(model).Run();
                new AddOne(model).Run();
                return true;
             }
        }

        public TransactionCmdTest()
        {

        }

        public void TestSeriesTransactionSucceeds()
        {
            ModelClass model = new ModelClass(5);
            ICmd.BeginTransaction();
            new AddOne(model).Run();
            new AddOne(model).Run();
            ICmd.PostTransaction();
            Assert.AreEqual(7, model.x);
            CommandStack.Instance.Undo();
            Assert.AreEqual(5, model.x);
        }

        [TestMethod]
        public void TestNestedCommandCreatesTransaction()
        {
            // Create a nested command
            ModelClass model = new ModelClass(5);
            NestedCmd cmd = new NestedCmd(model);
            cmd.Run();

            // Ensure Transaction command on stack
            Assert.IsTrue(CommandStack.Instance.Cmds.Peek().GetType() == typeof(TransactionCmd));
            Assert.IsTrue(CommandStack.Instance.Cmds.Count == 1);
        }

        [TestMethod]
        public void TestNestedCommandUndoRedo()
        {
            // Create a nested command
            ModelClass model = new ModelClass(5);
            NestedCmd cmd = new NestedCmd(model);
            cmd.Run();
            Assert.AreEqual(7, model.x);
            CommandStack.Instance.Undo();
            Assert.AreEqual(5, model.x);
            CommandStack.Instance.Redo();
            Assert.AreEqual(7, model.x);
        }

        [TestMethod]
        public void TestTransactionSucceeds()
        {
            ModelClass model = new ModelClass(5);
            ICmd.BeginTransaction(new AddOne(model));
            new AddOne(model);
            new AddOne(model);
            ICmd.PostTransaction();
            Assert.AreEqual(8, model.x);
            CommandStack.Instance.Undo();
            Assert.AreEqual(5, model.x);
            CommandStack.Instance.Redo();
            Assert.AreEqual(8, model.x);
        }
    }
}
