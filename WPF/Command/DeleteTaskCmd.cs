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
        private Model.Task original;
        private List<Task> dependents;
        private List<Task> dependentOn;
        private List<Task> subtasks;
        private List<User> users;
        private Task parentTask;
        private Project project;

        public DeleteTaskCmd(Model.Task task)
        {
            original = task; // store a copy
            dependents = original.Dependencies.ToList();
            dependentOn = original.DependentOn.ToList();
            subtasks = original.SubTasks.ToList();
            users = original.Workers.ToList();
            parentTask = task.ParentTask;
            project = task.Project;
        }

        protected override bool Execute()
        {
            // Turn off notifications while we work
            var model = Model.Model.Instance;
            model.Notify = false;
            Task task = GetUpdatedTask(model, original);
            foreach(Task t in subtasks)
                task.RemoveSubTask(GetUpdatedTask(model, t));
            foreach(Task t in dependents)
                task.RemoveDependency(GetUpdatedTask(model, t));
            foreach(Task t in dependentOn)
                GetUpdatedTask(model, t).RemoveDependency(task);
            foreach (User u in users)
                task.RemoveWorker(u);
            if (parentTask != null)
                GetUpdatedTask(model, parentTask).RemoveSubTask(task);
            model.UpdateProject(ref project);
            project.RemoveTask(task);
            model.Notify = true;
            task.Delete();
            return true;
        }
        
        private Task GetUpdatedTask(Model.Model model, Task t)
        {
            return model.UpdateTask(ref t);
        }

        public override bool Undo()
        {
            var model = Model.Model.Instance;
            model.Notify = false;
            Model.Task newTask = new Model.Task(original, insert: original.IsTracked, original.IsTracked);
            model.UpdateProject(ref project);
            project.AddTask(newTask);
            foreach (Model.Task t in dependents)
                newTask.AddDependency(GetUpdatedTask(model, t));
            foreach (Task t in dependentOn)
                GetUpdatedTask(model, t).AddDependency(newTask);
            foreach (User user in users)
                newTask.AddWorker(user);
            foreach (Task t in subtasks)
                newTask.AddSubTask(GetUpdatedTask(model, t));
            if (parentTask != null)
                GetUpdatedTask(model, parentTask).AddSubTask(newTask);
            model.Notify = true;
            newTask.NotifyUpdate();
            return true;
        }

    }
}
