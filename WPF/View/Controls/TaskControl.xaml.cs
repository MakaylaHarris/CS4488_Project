using System.Windows;
using System.Windows.Controls;

using SmartPert.Model;
using SmartPert.View.Pages;
using SmartPert.View.Windows;

namespace SmartPert.View.Controls
{
    /// <summary>
    /// Interaction logic for TaskControl.xaml
    /// </summary>
    public partial class TaskControl : UserControl
    {
        Chart _chart;

        public TaskControl(Task task, Chart chart)
        {
            InitializeComponent();
            DataContext = task;
            this._chart = chart;

        }

        private void mi_editTask_Click(object sender, RoutedEventArgs e)
        {
            Task task = (Task)((MenuItem)sender).DataContext;
            new TaskEditor(null, task).ShowDialog();
            _chart.RefreshGraph();
            //_chart.DrawGraph(_chart.GetTasksAndDependanciesFromDatabase());
        }

        private void mi_deleteTask_Click(object sender, RoutedEventArgs e)
        {
            Task task = (Task)((MenuItem)sender).DataContext;
            _chart.DeleteTask(task);
            _chart.DeleteTask(task);
            _chart.RefreshGraph();

        }
    }
}
