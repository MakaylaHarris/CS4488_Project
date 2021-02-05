using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF.View
{
    public class MenuItemViewModel : MenuItem
    {
        private string name;
        private ICommand cmd;

        public MenuItemViewModel(string name, ICommand cmd)
        {
            this.name = name;
            this.cmd = cmd;
        }

        public string Name { get; }
        public ICommand Command { get; }
        
    }
}
