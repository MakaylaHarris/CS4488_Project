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
    public abstract class ICmd : IViewModel
    {
        /// <summary>
        /// Runs the command and adds it to the stack if successful
        /// </summary>
        /// <returns>True on success</returns>
        public bool Run(bool isRedo=false)
        {
            if(Execute())
            {
                CommandStack.Instance.PushCommand(this, isRedo);
                return true;
            }
            return false;
        }

        /// <summary>
        ///  Undo the command
        /// </summary>
        /// <returns>true on success</returns>
        public abstract bool Undo();

        /// <summary>
        /// Updates id, used when creating deleted objects (on undo/redo)
        /// </summary>
        /// <param name="old">old object</param>
        /// <param name="newItem">new object</param>
        public abstract void OnIdUpdate(TimedItem old, TimedItem newItem);
        public abstract void OnModelUpdate(Project p);

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
