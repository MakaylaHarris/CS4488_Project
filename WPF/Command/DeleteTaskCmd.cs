using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPert.Model;

namespace SmartPert.Command
{
    /// <summary>
    /// Command to delete a task
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    public class DeleteTaskCmd : ICmd
    {
        private readonly IModel model;
        private Model.Task task;

        public DeleteTaskCmd(IModel model, Model.Task task)
        {
            this.model = model;
            this.task = task;
        }
        protected override bool Execute()
        {
            this.model.DeleteTask(task);
            return true;
        }

        public override bool Undo()
        {
            Model.Task newTask = model.CreateTask(task.Name, task.StartDate, task.EndDate, task.Description, task.LikelyDuration, task.MaxDuration, task.MinDuration);
            if (newTask != null)
            {
                List<Model.Task> all = model.GetTasks();
                foreach (Model.Task t in task.Dependencies)
                    newTask.AddDependency(all.Find(x => x.Id == t.Id));
                foreach (User user in task.Workers)
                    newTask.AddWorker(user);
                task = newTask;
                return true;
            }
            return false;
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if (task == old)
                task = (Model.Task)newItem;
        }

        public override void OnModelUpdate(Project p)
        {
            UpdateTask(ref task);
        }
    }
}
