using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pert.Model;

namespace Pert.View
{
    class OpenProjectCommand : ICommand
    {
        private readonly IModel model;
        private readonly Project project;

        public OpenProjectCommand(IModel model, Project project)
        {
            this.model = model;
            this.project = project;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            model.SetProject(project);
        }
    }
}
