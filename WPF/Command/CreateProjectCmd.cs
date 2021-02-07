using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pert.Model;

namespace Pert.Command
{
    /// <summary>
    /// Command to create project
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    class CreateProjectCmd : ICmd
    {
        private IModel model;
        private readonly string name;
        private readonly DateTime start;
        private readonly DateTime end;
        private readonly string description;
        private Project project;

        public CreateProjectCmd(IModel model, string name, DateTime start, DateTime end, string description = "")
        {
            this.model = model;
            this.name = name;
            this.start = start;
            this.end = end;
            this.description = description;
        }
        public bool Execute()
        {
            project = model.CreateProject(name, start, end, description);
            return project != null;
        }

        public bool Undo()
        {
            model.DeleteProject(project);
            return true;
        }
    }
}
