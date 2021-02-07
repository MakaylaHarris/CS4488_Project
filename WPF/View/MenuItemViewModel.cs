using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Pert
{
    public class MenuItemViewModel
    {
        private readonly string name;
        private readonly ICommand _command;

        public MenuItemViewModel(string name, ICommand command)
        {
            this.name = name;
            _command = command;

        }

        public string Header { get => name; }

        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

        public ICommand Command
        {
            get
            {
                return _command;
            }
        }

    }

}
