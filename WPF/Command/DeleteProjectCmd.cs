using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPert.Model;

namespace SmartPert.Command
{
    /// <summary>
    /// Delete Project command
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    class DeleteProjectCmd : ICmd
    {
        private readonly IModel model;
        private Project project;

        public DeleteProjectCmd(IModel model, Project project)
        {
            this.model = model;
            this.project = project;
        }

        public bool Execute()
        {
            this.model.DeleteProject(project);
            return true;
        }

        public bool Undo()
        {
            Project newProject = this.model.CreateProject(project.Name, project.StartDate, project.EndDate, project.Description);
            if(newProject != null) {
                List<Model.Task> tasks = project.Tasks; 
                // Re-add tasks
                foreach (Model.Task task in tasks)
                {
                    Model.Task newTask = model.CreateTask(task.Name, task.StartDate, task.EndDate, task.Description,
                        task.LikelyDuration, task.MaxDuration, task.MinDuration);
                    foreach(User worker in task.Workers)
                    {
                        newTask.AddWorker(worker);
                    }
                }
                // And Dependencies
                foreach(Model.Task task in tasks)
                {
                    foreach(Model.Task dependent in task.Dependencies)
                    {
                        Model.Task match = newProject.Tasks.Find(x => x.Id == dependent.Id);
                        task.AddDependency(match);
                    }
                }
                // Re-add workers
                foreach(User worker in project.Workers)
                {
                    newProject.AddWorker(worker);
                }
                project = newProject;
                return true;
            }
            return false;
        }
    }
}
