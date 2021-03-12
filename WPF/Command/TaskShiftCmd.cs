using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartPert.Command
{
    /// <summary>
    /// Shifts the task dates
    /// </summary>
    public class TaskShiftCmd : ICmd
    {
        private Task task;

        public TaskShiftCmd(Task task, int shift)
        {
            Task = task;
            Shift = shift;
        }

        public Task Task { get => task; set => task = value; }
        public int Shift { get; }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if (old == Task)
                Task = newItem as Task;
        }

        public override void OnModelUpdate(Project p)
        {
            UpdateTask(ref task);
        }

        public override bool Undo()
        {
            task.Shift(-1 * Shift);
            return true;
        }

        protected override bool Execute()
        {
            task.Shift(Shift);
            return true;
        }
    }
}
