using SmartPert.Command;
using SmartPert.Model;
using SmartPert.View.Windows;
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

        public TaskRowLabel(Task task)
        {
            InitializeComponent();
            this.task = task;
            DataContext = this.task;
        }

        private bool CanShiftLeft()
        {
            return task.ParentTask != null;
        }

        private Task GetTaskAbove()
        {
            int index = task.Project.SortedTasks.IndexOf(task);
            if (index > 0)
                return task.Project.SortedTasks[index - 1];
            return null;
        }


        private bool CanShiftRight()
        {
            // if it's at the same level...
            Task above = GetTaskAbove();
            return above != null && above.CanAddSubTask(task);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (CanShiftLeft())
            {
                LeftButton.Visibility = Visibility.Visible;
            }
            if (CanShiftRight())
            {
                RightButton.Visibility = Visibility.Visible;
            }
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
            if(CanShiftLeft())
            {
                // Test if we can keep at this row, if subtasks are below then we must move outside shifting rows
                if (HasSubtasksBelow(task.ParentTask, task.ProjectRow)) 
                    new ShiftToRowCmd(task, task.ParentTask.ProjectRow).Run();
                // Adding as subtask to grandparent?
                else if (task.ParentTask.ParentTask != null) 
                    new AddSubTaskCmd(task.ParentTask.ParentTask, task).Run();
                // Outer layer
                else
                    new RemoveSubtaskCmd(task.ParentTask, task).Run();
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if(CanShiftRight())
            {
                Task above = GetTaskAbove();
                Task myParent = task.ParentTask;
                while (above.ParentTask != myParent && above != null)
                    above = above.ParentTask;
                new AddSubTaskCmd(above, task).Run();
            }
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
