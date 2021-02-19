using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPert.Model;

namespace SmartPert.Command
{
    public class CreateTaskCmd : ICmd
    {
        private readonly string name;
        private readonly DateTime start;
        private readonly DateTime? end;
        private readonly int duration;
        private readonly int maxDuration;
        private readonly int minDuration;
        private readonly string description;
        private Model.Task task;

        /// <summary>
        /// Gets the created task
        /// </summary>
        public Model.Task Task { get => task; }

        public CreateTaskCmd(string name, DateTime start, DateTime? end, int duration, int maxDuration = 0, int minDuration = 0, string description = "")
        {
            this.name = name;
            this.start = start;
            this.end = end;
            this.duration = duration;
            this.maxDuration = maxDuration;
            this.minDuration = minDuration;
            this.description = description;
            task = null;
        }
        protected override bool Execute()
        {
            Model.Task prev = task;
            task = Model.Model.Instance.CreateTask(name, start, end, description, duration, maxDuration, minDuration);
            if (prev != null)
                CommandStack.Instance.UpdateIds(prev, task);
            return task != null;
        }

        public override bool Undo()
        {
            Model.Model.Instance.DeleteTask(task);
            return true;
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if (old == task)
                task = (Model.Task) newItem;
        }

        public override void OnModelUpdate(Project p)
        {
            UpdateTask(ref task);
        }
    }
}
