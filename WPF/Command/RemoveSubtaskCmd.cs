using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartPert.Command
{
    /// <summary>
    /// Remove a sub-task Command
    /// Created 3/8/2021 by Robert Nelson
    /// </summary>
    public class RemoveSubtaskCmd : ICmd
    {
        private Task parent;
        private Task subtask;

        public RemoveSubtaskCmd(Task parent, Task subtask)
        {
            this.parent = parent;
            this.subtask = subtask;
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if (parent == old)
                parent = (Task)newItem;
            else if (subtask == old)
                subtask = (Task)newItem;
        }

        public override void OnModelUpdate(Project p)
        {
            UpdateTask(ref parent);
            UpdateTask(ref subtask);
        }

        public override bool Undo()
        {
            return parent.AddSubTask(subtask);
        }

        protected override bool Execute()
        {
            return parent.RemoveSubTask(subtask);
        }
    }
}
