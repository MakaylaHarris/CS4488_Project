using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartPert.View.Windows
{
    /// <summary>
    /// Task Editor
    /// Created 2/11/2021 by Robert Nelson
    /// </summary>
    public partial class TaskEditor : Window
    {
        private Task task;
        private IModel model;

        public TaskEditor(IModel model=null, Task task = null)
        {
            InitializeComponent();
            DataContext = this;
            if (model == null && task == null)
                throw new ArgumentNullException("task", "Task Editor requires model or task!");
            this.model = model;
            Owner = Application.Current.MainWindow;
            this.task = task;
            LoadTaskData();
        }

        private void LoadTaskData()
        {
            Task t = this.task;
            if (t != null)
            {
                TaskName.Text = t.Name;
                TaskDescription.Text = t.Description;
                EndDate.SelectedDate = t.EndDate;
                StartDate.SelectedDate = t.StartDate;

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

    }
}
