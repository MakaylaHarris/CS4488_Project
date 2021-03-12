using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmartPert.ViewModels;

namespace SmartPert.ViewModelCommands
{
    class AccountUpdateCommand : ICommand
    {
        private AccountViewModel _viewModel;

        /// <summary>
        /// Initializes an instance of the Account Update Command
        /// </summary>
        public AccountUpdateCommand(AccountViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        #region ICommand Members
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
