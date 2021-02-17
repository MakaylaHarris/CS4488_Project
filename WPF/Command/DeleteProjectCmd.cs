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
    public class DeleteProjectCmd : ICmd
    {
        private readonly IModel model;
        private Project project;

        public DeleteProjectCmd(IModel model, Project project)
        {
            this.model = model;
            this.project = project;
        }

        protected override bool Execute()
        {
            this.model.DeleteProject(project);
            return true;
        }

        public override bool Undo()
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
                CommandStack.Instance.UpdateIds(project, newProject);
                project = newProject;
                return true;
            }
            return false;
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if (old == project)
                project = (Project)newItem;
        }

        public override void OnModelUpdate(Project p)
        {
            UpdateProject(ref project);
        }
    }
}
