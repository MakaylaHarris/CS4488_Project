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
        private List<Task> prevOrdered;

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
            if(prevOrdered != null)
            {
                subtask.Project.SortedTasks = prevOrdered;
                prevOrdered = null;
            }
            return parent.AddSubTask(subtask);
        }

        private bool IsAtGroupBottom(Task task)
        {
            List<Task> tasks = task.Project.SortedTasks;
            int i = tasks.IndexOf(task) + 1;
            return i >= tasks.Count || tasks[i].ParentTask == null;
        }

        protected override bool Execute()
        {
            if (!IsAtGroupBottom(subtask))
            {
                prevOrdered = subtask.Project.SortedTasks;
                subtask.Project.SortedTasks = AddSubTaskCmd.ReorderRows(subtask, subtask.ParentTask, AddSubTaskCmd.InsertAfterOutmost);
            }
            else
                prevOrdered = null;
            return parent.RemoveSubTask(subtask);
        }
    }
}
