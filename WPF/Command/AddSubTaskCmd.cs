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
        public const int InsertAfterGroup = 0;
        public const int InsertAfterOutmost = 1;

        private Task parent;
        private Task subtask;
        private List<Task> oldSorted;
        private bool detach;
        private Task oldParent;

        public Task Parent => parent;
        public Task Subtask => subtask;
        public AddSubTaskCmd(Task parent, Task subtask)
        {
            this.parent = parent;
            this.subtask = subtask;
        }

        // Task can only be added within subtask group, and must not break up subgroups
        protected bool CanAddSubtaskAtRow()
        {
            var parentGroup = ShiftToRowCmd.GetSubTaskGroup(parent);
            var subGroup = ShiftToRowCmd.GetSubTaskGroup(subtask);
            List<Task> sorted = parent.Project.SortedTasks;
            // Immediately After parent group or at end of parent group
            if (subGroup.Item1 == parentGroup.Item2 + 1 || subGroup.Item2 == parentGroup.Item2)
                return true;
            // Not in parent range
            if (subGroup.Item1 < parentGroup.Item1 || subGroup.Item1 > parentGroup.Item2)
                return false;
            return sorted[subGroup.Item2 + 1].ParentTask == parent;
        }

        public static List<Task> ReorderRows(Task toMove, Task parent, int insertOp=0)
        {
            List<Task> oldSorted = toMove.Project.SortedTasks;
            var subgroup = ShiftToRowCmd.GetSubTaskGroup(toMove);
            var parentgroup = ShiftToRowCmd.GetSubTaskGroup(parent);
            int insertAt = parentgroup.Item2 + 1;
            // Shift insertion point to the next non-subtask
            if (insertOp == InsertAfterOutmost)
            {
                while(insertAt < oldSorted.Count)
                {
                    if (oldSorted[insertAt].ParentTask == null)
                        break;
                    insertAt++;
                }
            }

            List<Task> reordered = new List<Task>();
            // Add initial tasks
            for (int i = 0; i < insertAt; i++)
            {
                if (i < subgroup.Item1 || i > subgroup.Item2)
                    reordered.Add(oldSorted[i]);
            }
            // Add the moving group
            reordered.AddRange(oldSorted.GetRange(subgroup.Item1, subgroup.Item2 + 1 - subgroup.Item1));
            // Add remaining
            for (int i = insertAt; i < oldSorted.Count; i++)
                if (i < subgroup.Item1 || i > subgroup.Item2)
                    reordered.Add(oldSorted[i]);
            return reordered;
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
            if(result && oldSorted != null)
                subtask.Project.SortedTasks = oldSorted;
            if(result && detach)
            {
                detach = false;
                result = oldParent.AddSubTask(subtask);
            }
            return result;
        }


        protected override bool Execute()
        {
            if (!parent.CanAddSubTask(subtask))
                return false;
            if(subtask.ParentTask != null)      // Then remove
            {
                detach = true;
                oldParent = subtask.ParentTask;
                oldParent.RemoveSubTask(subtask);
            }
            if (!CanAddSubtaskAtRow())
            {
                oldSorted = subtask.Project.SortedTasks;
                subtask.Project.SortedTasks = ReorderRows(subtask, parent);
            }
            else
                oldSorted = null;
            return parent.AddSubTask(subtask);
        }
    }
}
