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
    /// Command for updating the user password from the password page
    /// @author: Makayla Linnastruth
    /// @date: 03/14/2021
    /// </summary>
    class PasswordUpdateCommand : ICommand
    {
        private AccountViewModel _viewModel;

        /// <summary>
        /// Initializes an instance of the Password Update Command
        /// </summary>
        public PasswordUpdateCommand(AccountViewModel viewModel)
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
            return _viewModel.CanUpdatePw;
        }

        public void Execute(object parameter)
        {
            _viewModel.SavePasswordInfo();
        }
        #endregion
    }
}
