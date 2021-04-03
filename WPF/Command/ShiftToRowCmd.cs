using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartPert.Command
{
    /// <summary>
    /// Command to shift a task to a row
    /// Created 3/29/2021 by Robert Nelson
    /// </summary>
    public class ShiftToRowCmd : ICmd
    {
        private int targetRow;
        private int insertIndex;
        private Task task;
        private List<Task> subtaskGroup;
        private List<Task> tasks;
        private List<Task> prevSorted;
        private Task above;
        private bool attached;
        private bool isValid;
        private Task detachedFrom;
        private Tuple<int, int> groupRange;

        private Tuple<int, int> GroupRange
        {
            get
            {
                if (groupRange == null)
                    groupRange = GetSubTaskGroup(task.Project.SortedTasks, task);
                return groupRange;
            }
        }

        private Task Above { get {
                if (above == null)
                    above = GetTaskAbove();
                return above;
            } }

        public ShiftToRowCmd(Task t, int targetRow)
        {
            task = t;
            this.targetRow = targetRow;
            isValid = true;
            insertIndex = -1;
        }

        #region Private Methods
        /// <summary>
        /// Determines if task can be shifted to the row
        /// </summary>
        private bool CanShiftToRow()
        {
            // the task can not be a subtask of itself
            return isValid;
        }


        private void BeforeExecute()
        {
            // Store some old things
            prevSorted = task.Project.SortedTasks;

            // Divide tasks into those to be moved and those that will be moved around
            // Also determine where to insert the moving task at
            subtaskGroup = new List<Task>();
            tasks = new List<Task>();
            var sorted = prevSorted;
            for(int i = 0; i < sorted.Count; i++)
            {
                if (i < GroupRange.Item1)
                {
                    if (insertIndex == -1 && sorted[i].ProjectRow >= targetRow)
                        insertIndex = tasks.Count;
                    tasks.Add(sorted[i]);
                }
                // i >= GroupRange.Item1
                else if (i <= GroupRange.Item2)
                {
                    if (insertIndex == -1 && sorted[i].ProjectRow >= targetRow)
                        isValid = false;
                    subtaskGroup.Add(sorted[i]);
                }
                else // i > GroupRange.Item2 
                {
                    if (insertIndex == -1 && sorted[i].ProjectRow >= targetRow)
                        insertIndex = tasks.Count;
                    tasks.Add(sorted[i]);
                }
            }
        }

        /// <summary>
        /// Gets the list of all possible tasks that could be parent once shifted to the target row
        /// </summary>
        /// <returns>list of tasks</returns>
        private List<Task> GetTasksThatCouldBeParent()
        {
            var ret = new List<Task>();
            bool canBeOutermost = true;
            // When inserting into the middle of a group, the task must have a subtask level >= to the tasks below it
            // Start at the task 1 above and move outwards, finding how far down their subtasks go
            if(insertIndex - 1 >= 0)
                for(Task t = tasks[insertIndex - 1]; t != null; t = t.ParentTask)
                {
                    ret.Add(t);
                    if (insertIndex < tasks.Count && tasks[insertIndex].TaskIsAncestor(t))
                    {
                        canBeOutermost = false;
                        break;  // Stop if a subtask group that extends below the insertion point is found
                    }
                }
            if (canBeOutermost)
                ret.Add(null);
            return ret;
        }

        /// <summary>
        /// Figures out which is the ideal parent task that can be added as the new parent
        /// </summary>
        /// <param name="t">Task</param>
        /// <returns>New Parent task</returns>
        private Task GetIdealParent(Task t)
        {
            List<Task> possible = GetTasksThatCouldBeParent();
            if (possible.Contains(t.ParentTask))
                return t.ParentTask;
            // Otherwise, Gives the outermost level possible
            return possible[possible.Count - 1];
        }

        /// <summary>
        /// Gets the task immediately above the row
        /// </summary>
        /// <returns>task</returns>
        private Task GetTaskAbove()
        {
            Task lastAbove = null;
            foreach(Task t in tasks)
            {
                if (t.ProjectRow < targetRow)
                    lastAbove = t;
                else
                    break;
            }
            return lastAbove;
        }

        /// <summary>
        /// Gets the group of subtasks under a parent task
        /// </summary>
        /// <param name="sorted">list of all tasks, ordered</param>
        /// <param name="parent">the parent of the group</param>
        /// <returns>integer range [min,max]</returns>
        private static Tuple<int, int> GetSubTaskGroup(List<Task> sorted, Task parent)
        {
            int start = -1, end = -1;
            for (int i = 0; i < sorted.Count; i++)
            {
                if (sorted[i] == parent)
                    start = end = i;
                else if (start > -1)
                {
                    if (sorted[i].TaskIsAncestor(parent))
                        end = i;
                    else
                        break;
                }

            }
            return new Tuple<int, int>(start, end);
        }


        private List<Task> reorderTasks()
        {
            List<Task> ordered = new List<Task>();
            for(int i = 0; i < tasks.Count; i++)
            {
                if (i == insertIndex)
                    ordered.AddRange(subtaskGroup);
                ordered.Add(tasks[i]);
            }
            if (insertIndex < 0 || insertIndex >= tasks.Count) // Then just add at the end
                ordered.AddRange(subtaskGroup);
            return ordered;
        }
        #endregion

        #region Public Methods
        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if (old.GetType() == typeof(Task) && task == (Task)old)
                task = (Task)newItem;
        }

        public override void OnModelUpdate(Project p)
        {
        }

        public override bool Undo()
        {
            // Work our way backwards
            if(attached)
            {
                task.ParentTask.RemoveSubTask(task);
                attached = false;
            }
            // Resort
            task.Project.SortedTasks = prevSorted;
            if(detachedFrom != null)
            {
                detachedFrom.AddSubTask(task);
            }
            return true;
        }

        protected override bool Execute()
        {
            BeforeExecute();
            if (!CanShiftToRow())
                return false;
            Task newParent = GetIdealParent(task);
            // Detach
            if (newParent != task.ParentTask && task.ParentTask != null) {
                detachedFrom = task.ParentTask;
                task.ParentTask.RemoveSubTask(task);
            }
            // Resort
            task.Project.SortedTasks = reorderTasks();
            // Add as subtask
            if (newParent != null && newParent != task.ParentTask) {
                newParent.AddSubTask(task);
                attached = true;
            }
            return true;
        }
        #endregion
    }
}
