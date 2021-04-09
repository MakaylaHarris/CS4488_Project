using SmartPert.Command;
using SmartPert.Model;
using SmartPert.View.Windows;
using SmartPert.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartPert.View.Controls
{
    /// <summary>
    /// Interaction logic for TaskRowLabel.xaml
    /// Created 3/30/2021 by Robert Nelson
    /// </summary>
    public partial class TaskRowLabel : UserControl
    {
        private readonly Task task;
        private readonly TaskRowLabelViewModel viewModel;

        public TaskRowLabel(Task task)
        {
            InitializeComponent();
            this.task = task;
            viewModel = new TaskRowLabelViewModel(task);
            DataContext = this.task;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (viewModel.CanShiftLeft())
                LeftButton.Visibility = Visibility.Visible;
            if (viewModel.CanShiftRight())
                RightButton.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            LeftButton.Visibility = Visibility.Hidden;
            RightButton.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Returns true if the parent has subtasks below the given row
        /// </summary>
        /// <param name="parent">parent task</param>
        /// <param name="row">project row</param>
        /// <returns></returns>
        private bool HasSubtasksBelow(Task parent, int row)
        {
            foreach (Task t in parent.Tasks)
                if (t.ProjectRow > row)
                    return true;
            return false;
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShiftLeft();
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShiftRight();
        }

        private void mi_editTask_Click(object sender, RoutedEventArgs e)
        {
            StateSwitcher.Instance.OnTaskCreateOrEdit(task);
        }

        private void mi_deleteTask_Click(object sender, RoutedEventArgs e)
        {

            new DeleteTaskCmd(task).Run();
        }
        private void Add_New_Click(object sender, RoutedEventArgs e)
        {
            new TaskEditor().ShowDialog();
        }


    }
}
