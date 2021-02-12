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
        private readonly IModel model;
        private readonly string name;
        private readonly DateTime start;
        private readonly DateTime? end;
        private readonly int duration;
        private readonly int maxDuration;
        private readonly int minDuration;
        private readonly string description;
        private Model.Task task;

        public CreateTaskCmd(IModel model, string name, DateTime start, DateTime? end, int duration, int maxDuration = 0, int minDuration = 0, string description = "")
        {
            this.model = model;
            this.name = name;
            this.start = start;
            this.end = end;
            this.duration = duration;
            this.maxDuration = maxDuration;
            this.minDuration = minDuration;
            this.description = description;
        }
        protected override bool Execute()
        {
            task = model.CreateTask(name, start, end, description, duration, maxDuration, minDuration);
            return task != null;
        }

        public override bool Undo()
        {
            model.DeleteTask(task);
            return true;
        }
    }
}
