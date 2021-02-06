using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pert.Model;
using Pert.View;

namespace Pert.View
{
    class ViewModel : IViewModel
    {
        private IModel model;
        private MainWindow mainWindow;
        public ViewModel(MainWindow window)
        {
            model = new Model.Model(this);
            mainWindow = window;
            if (!model.IsConnected())
            {
                mainWindow.ShowDBConnectionSettings();
            }
        }

        public bool SetConnectionString(string s) => model.SetConnectionString(s);

        public bool IsConnected() => model.IsConnected();

        public void OnModelUpdate(Project p)
        {
            Console.WriteLine("Model was updated");
        }
    }
}
