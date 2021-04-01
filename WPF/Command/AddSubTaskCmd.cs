using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartPert.Command
{
    /// <summary>
    /// Command that adds a sub-task
    /// Created 3/8/2021 by Robert Nelson
    /// </summary>
    public class AddSubTaskCmd : ICmd
    {
        private Task parent;
        private Task subtask;
        private bool detach;
        private Task oldParent;

        public Task Parent => parent;
        public Task Subtask => subtask;
        public AddSubTaskCmd(Task parent, Task subtask)
        {
            this.parent = parent;
            this.subtask = subtask;
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if (old == parent)
                parent = (Task)newItem;
            else if (old == subtask)
                subtask = (Task)newItem;
            else if (old == oldParent)
                oldParent = (Task)newItem;
        }

        public override void OnModelUpdate(Project p)
        {
            UpdateTask(ref parent);
            UpdateTask(ref subtask);
        }

        public override bool Undo()
        {
            bool result = parent.RemoveSubTask(subtask);
            if(result && detach)
            {
                detach = false;
                result = oldParent.AddSubTask(subtask);
            }
            return result;
        }

        protected override bool Execute()
        {
            if(subtask.ParentTask != null)      // Then remove
            {
                detach = true;
                oldParent = subtask.ParentTask;
                oldParent.RemoveSubTask(subtask);
            }
            return parent.AddSubTask(subtask);
        }
    }
}
