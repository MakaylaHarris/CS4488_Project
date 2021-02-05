using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Model;

namespace WPF.View
{
    class ViewModel : IViewModel
    {
        private IModel model;
        private MainWindow mainWindow;

        public IModel Model { get; }

        public ViewModel(MainWindow window)
        {
            model = new Model.Model(this);
            mainWindow = window;
            if (!model.IsConnected())
            {
                mainWindow.ShowDBConnectionSettings();
            }
        }


        #region Public Methods
        public bool SetConnectionString(string s) => model.SetConnectionString(s);

        public bool IsConnected() => model.IsConnected();

        public void OnModelUpdate(Project p)
        {
            Console.WriteLine("Model was updated");
        }

        public void Refresh() => model.Refresh();
        #endregion
    }
}
