using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPert.Model;

namespace SmartPert.Command
{
    /// <summary>
    /// Command to create project
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    public class CreateProjectCmd : ICmd
    {
        private readonly string name;
        private readonly DateTime start;
        private readonly DateTime? end;
        private readonly string description;
        private Project project;

        public CreateProjectCmd(string name, DateTime start, DateTime? end, string description = "")
        {
            this.name = name;
            this.start = start;
            this.end = end;
            this.description = description;
            project = null;
        }
        protected override bool Execute()
        {
            Project prev = project;
            project = Model.Model.Instance.CreateProject(name, start, end, description);
            if (prev != null)
                CommandStack.Instance.UpdateIds(prev, project);
            return project != null;
        }

        public override bool Undo()
        {
            Model.Model.Instance.DeleteProject(project);
            return true;
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if (old == project)
                project = (Project) newItem;
        }

        public override void OnModelUpdate(Project p)
        {
            UpdateProject(ref project);
        }
    }
}
