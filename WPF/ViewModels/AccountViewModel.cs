using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmartPert.Model;
using SmartPert.ViewModelCommands;

namespace SmartPert.ViewModels
{
    class AccountViewModel
    {
        private Project _project;

        public AccountViewModel()
        {
            _project = Model.Model.Instance.GetProject();
            AccountUpdateCommand = new AccountUpdateCommand(this);
        }

        /// <summary>
        /// Gets the Project Instance
        /// </summary>
        public Project Project
        {
            get { return _project; }
        }

        /// <summary>
        /// Gets the AccountUpdateCommand for the view model.
        /// </summary>
        public ICommand AccountUpdateCommand
        {
            get;
            private set;
        }
    }
}
