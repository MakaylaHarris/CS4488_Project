using SmartPert.Model;
using SmartPert.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPert.Command
{
    /// <summary>
    /// Command pattern interface, any command that can be undone should implement
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    public abstract class ICmd
    {
        private static TransactionCmd transaction;
        private static bool transactionIsInternal;
        private static ICmd current;  // Wether a command is currently running

        public ICmd()
        {
            if (transaction != null)
                transaction.Add(this);
        }

        public static void BeginTransaction(ICmd cmd=null, bool hasRun=false, bool isInternal=false)
        {
            transaction = new TransactionCmd();
            transactionIsInternal = isInternal;
            if (cmd != null)
                transaction.Add(cmd, hasRun);
        }

        public static bool PostTransaction()
        {
            bool result = transaction.Run();
            transaction = null;
            return result;
        }

        /// <summary>
        /// Runs the command and adds it to the stack if successful
        /// </summary>
        /// <returns>True on success</returns>
        public bool Run(bool isRedo=false, bool pushStack=true)
        {
            // Are we attempting to run another command mid-way through?
            if (current != null)
            {
                if (transaction == null)
                {
                    BeginTransaction(current, true, true);
                    transaction.Add(this);
                }
            }
            else
            {
                current = this;
                if (current.GetType() == typeof(TransactionCmd))
                    transaction = (TransactionCmd) this;
            }

            if (transaction != null && transaction != this && pushStack)
                return transaction.Run(this);
            try
            {
                bool result = Execute();
                if (result)
                {
                    if (transaction == null || this == transaction)
                        CommandStack.Instance.PushCommand(this, isRedo);
                }
                // Reset current
                if (current == this)
                {
                    if (transaction != null && current != transaction && transactionIsInternal)
                        result = result && PostTransaction();
                    current = null;
                }
                return result;
            }
            catch (Exception) {
                current = null;
                transaction = null;
                throw;
            }
        }

        /// <summary>
        ///  Undo the command
        /// </summary>
        /// <returns>true on success</returns>
        public abstract bool Undo();

        /// <summary>
        /// Get updates for new objects, used when creating deleted objects (on undo/redo)
        /// </summary>
        /// <param name="old">old object</param>
        /// <param name="newItem">new object</param>
        public virtual void OnIdUpdate(TimedItem old, TimedItem newItem) { }
        public virtual void OnModelUpdate(Project p) { }

        #region Protected Methods
        /// <summary>
        /// Called by run to execute command
        /// </summary>
        /// <returns>true on success</returns>
        protected abstract bool Execute();

        protected void UpdateTask(ref Model.Task t) => Model.Model.Instance.UpdateTask(ref t, false);

        protected void UpdateProject(ref Project p) => Model.Model.Instance.UpdateProject(ref p, false);

        protected void UpdateUser(ref User u) => Model.Model.Instance.UpdateUser(ref u);

        protected void UpdateTimedItem(ref TimedItem item)
        {
            if (item.GetType() == typeof(Model.Task))
            {
                Model.Task t = (Model.Task)item;
                UpdateTask(ref t);
                item = t;
            }
            else if (item.GetType() == typeof(Model.Project))
            {
                Project p = (Project)item;
                UpdateProject(ref p);
                item = p;
            }

        }
        #endregion

    }

}
