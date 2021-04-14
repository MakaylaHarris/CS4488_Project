using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartPert.Model;

namespace SmartPert.Command
{
    /// <summary>
    /// Command to edit task
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    public class EditTaskCmd : ICmd
    {
        private Task toEdit;
        private readonly string name;
        private readonly DateTime? start;
        private readonly DateTime? end;
        private readonly int likelyDuration;
        private readonly int maxDuration;
        private readonly int minDuration;
        private readonly string description;
        private readonly bool markIncomplete;
        private readonly Task oldTask;

        public EditTaskCmd(Task toEdit, string name=null, DateTime? start=null, DateTime? end = null, int likelyDuration=-1, int maxDuration=-1, int minDuration=-1, string description=null, bool markIncomplete=false)
        {
            this.toEdit = toEdit;
            this.name = name;
            this.start = start;
            if (start == null && end != null && end < toEdit.SetStartDate)
                this.start = end;
            this.end = end;
            this.likelyDuration = likelyDuration;
            this.maxDuration = maxDuration;
            this.minDuration = minDuration;
            this.description = description;
            this.markIncomplete = markIncomplete;
            oldTask = new Task(toEdit);
        }

        protected override bool Execute()
        {
            if(name != null)
                toEdit.Name = name;
            if(start != null)
                toEdit.StartDate = (DateTime)start;
            if(end != null || markIncomplete)
                toEdit.EndDate = end;
            if(maxDuration > 0)
                toEdit.MaxDuration = maxDuration;
            if(likelyDuration > 0)
                toEdit.LikelyDuration = likelyDuration;
            if(minDuration > 0)
                toEdit.MinDuration = minDuration;
            if(description != null)
                toEdit.Description = description;
            return true;
        }

        public override bool Undo()
        {
            if(name != null)
                toEdit.Name = oldTask.Name;
            if(start != null)
                toEdit.StartDate = oldTask.StartDate;
            if(end != null || markIncomplete)
                toEdit.EndDate = oldTask.EndDate;
            if(maxDuration > 0)
                toEdit.MaxDuration = oldTask.MaxDuration;
            if (likelyDuration > 0)
                toEdit.LikelyDuration = oldTask.LikelyDuration;
            if(minDuration > 0)
                toEdit.MinDuration = oldTask.MinDuration;
            if(description != null)
                toEdit.Description = oldTask.Description;
            return true;
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if(old == toEdit)
                toEdit = (Task) newItem;

        }

        public override void OnModelUpdate(Project p)
        {
            UpdateTask(ref toEdit);
        }
    }
}
