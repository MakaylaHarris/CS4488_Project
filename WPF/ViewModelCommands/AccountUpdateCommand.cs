using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmartPert.ViewModels;

namespace SmartPert.ViewModelCommands
{
    /// <summary>
    /// Command for updating the user information from the account page
    /// @author: Makayla Linnastruth
    /// @date: 03/14/2021
    /// </summary>
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
            return _viewModel.CanUpdate;
        }

        public void Execute(object parameter)
        {
            _viewModel.SaveAccountInfo();
        }
        #endregion
    }
}
