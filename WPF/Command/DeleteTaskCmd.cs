using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartPert.Model;

namespace SmartPert.Command
{
    /// <summary>
    /// Command to delete a task
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    public class DeleteTaskCmd : ICmd
    {
        private Model.Task task;

        public DeleteTaskCmd(Model.Task task)
        {
            this.task = task;
        }
        protected override bool Execute()
        {
            Model.Model.Instance.DeleteTask(task);
            return true;
        }
        
        public override bool Undo()
        {
            Model.Task newTask = Model.Model.Instance.CreateTask(task.Name, task.StartDate, task.EndDate, task.Description, task.LikelyDuration, task.MaxDuration, task.MinDuration);
            if (newTask != null)
            {
                List<Model.Task> all = Model.Model.Instance.GetTasks();
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
