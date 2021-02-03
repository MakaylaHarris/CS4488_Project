using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPF.Model;

namespace WPF.Command
{
    /// <summary>
    /// Command to edit task
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    class EditTaskCmd : ICmd
    {
        private readonly Task toEdit;
        private readonly string name;
        private readonly DateTime start;
        private readonly DateTime? end;
        private readonly int likelyDuration;
        private readonly int maxDuration;
        private readonly int minDuration;
        private readonly string description;
        private readonly Task oldTask;

        public EditTaskCmd(Task toEdit, string name, DateTime start, DateTime? end, int likelyDuration, int maxDuration, int minDuration, string description)
        {
            this.toEdit = toEdit;
            this.name = name;
            this.start = start;
            this.end = end;
            this.likelyDuration = likelyDuration;
            this.maxDuration = maxDuration;
            this.minDuration = minDuration;
            this.description = description;
            oldTask = new Task(toEdit.Name, toEdit.StartDate, toEdit.EndDate, toEdit.LikelyDuration, toEdit.MaxDuration,
                toEdit.MinDuration, toEdit.Description, toEdit.Id);
        }

        public bool Execute()
        {
            toEdit.Name = name;
            toEdit.StartDate = start;
            toEdit.EndDate = end;
            toEdit.LikelyDuration = likelyDuration;
            toEdit.MaxDuration = maxDuration;
            toEdit.MinDuration = minDuration;
            toEdit.Description = description;
            return true;
        }

        public bool Undo()
        {
            toEdit.Name = oldTask.Name;
            toEdit.StartDate = oldTask.StartDate;
            toEdit.EndDate = oldTask.EndDate;
            toEdit.LikelyDuration = oldTask.LikelyDuration;
            toEdit.MaxDuration = oldTask.MaxDuration;
            toEdit.MinDuration = oldTask.MinDuration;
            toEdit.Description = oldTask.Description;
            return true;
        }
    }
}
