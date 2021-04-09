using SmartPert.Command;
using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartPert.ViewModels
{
    public class TaskRowLabelViewModel
    {
        private readonly Task task;
        private List<Task> possibleParents;

        public TaskRowLabelViewModel(Task task)
        {
            this.task = task;
        }

        /// <summary>
        /// Gets a list of possible parents it could have at the current row
        /// </summary>
        /// <returns>list of tasks</returns>
        private static List<Task> GetPossibleRightShiftParents(Task task)
        {
            List<Task> tasks = new List<Task>();
            Task parent = task.ParentTask;
            var sorted = task.Project.SortedTasks;
            int i = sorted.IndexOf(task);
            if (i == 0)
                return tasks;

            for (Task t = sorted[i - 1]; t != parent; t = t.ParentTask)
            {
                if (t == null)
                    break;
                if (t.CanAddSubTask(task))
                    tasks.Add(t);
            }
            return tasks;
        }

        private static List<Task> GetPossibleLeftShiftParents(Task task)
        {
            List<Task> tasks = new List<Task>();
            if (task.ParentTask == null)
                return tasks;
            else if(task.ParentTask.ParentTask == null)
            {
                tasks.Add(null);
                return tasks;
            }
            for(Task t = task.ParentTask; t != null; t = t.ParentTask)
            {
                if (t.CanAddSubTask(task))
                    tasks.Add(t);
            }
            tasks.Add(null);
            return tasks;
        }


        public bool CanShiftRight()
        {
            possibleParents = GetPossibleRightShiftParents(task);
            return possibleParents.Count > 0;
        }

        public bool CanShiftLeft()
        {
            possibleParents = GetPossibleLeftShiftParents(task);
            return possibleParents.Count > 0;
        }


        public void ShiftRight()
        {
            if (CanShiftRight())
                new AddSubTaskCmd(possibleParents.Last(), task).Run();
        }

        public void ShiftLeft()
        {
            if (CanShiftLeft())
            {
                if (possibleParents[0] != null)
                    new AddSubTaskCmd(possibleParents[0], task).Run();
                else
                    new RemoveSubtaskCmd(task.ParentTask, task).Run();
            }
        }
    }
}
