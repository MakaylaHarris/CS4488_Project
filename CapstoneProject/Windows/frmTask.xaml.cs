using CapstoneProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CapstoneProject.DAL;
using CapstoneProject.Pages;
using System.Globalization;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace CapstoneProject {
    /// <summary>
    /// Interaction logic for frmCreateTask.xaml
    /// </summary>

    //By Levi Delezene
    public partial class frmTask : Window {
        private Chart _chart;
        private Task taskToEdit;

        public frmTask(Chart chart, Task task = null) {
            InitializeComponent();
            DataContext = this;
            Owner = Application.Current.MainWindow;
            _chart = chart;
            taskToEdit = task;
        }
        
        public List<User> Users { get; } = new OUser().Select();
        public List<Status> Statuses { get; } = new List<Status> { Status.Completed, Status.In_Progress, Status.Not_Started};
        public List<int> Priorities { get; } = new List<int> { 1,2,3,4,5};
        public ObservableCollection<Task> Tasks {
            get {
                ObservableCollection<Task> tasks = new OTask().Select(_chart.Project.Id);
                if (taskToEdit != null) {
                    tasks.Remove(taskToEdit);
                }
                return tasks;
            }
        }
       
        private bool validateInput() {
            bool ret = true;

            if (tbxTaskName.Text == null || tbxTaskName.Text == "") {
                tbxTaskName.BorderBrush = Brushes.Red;
                ret = false;
            }
            if (tbxTaskDescription.Text == null || tbxTaskDescription.Text == "") {
                tbxTaskDescription.BorderBrush = Brushes.Red;
                ret = false;
            }
            if (tbxMinDuration.Text == null || tbxMinDuration.Text == "") {
                tbxMinDuration.BorderBrush = Brushes.Red;
                ret = false;
            }
            if (tbxMaxDuration.Text == null || tbxMaxDuration.Text == "") {
                tbxMaxDuration.BorderBrush = Brushes.Red;
                ret = false;
            }
            if (tbxMostLikelyDuration.Text == null || tbxMostLikelyDuration.Text == "")
            {
                tbxMostLikelyDuration.BorderBrush = Brushes.Red;
                ret = false;
            }
            if(dtStartDate.SelectedDate == null)
            {
                dtStartDate.BorderBrush = Brushes.Red;
                ret = false;
            }
            if (cmbOwner.SelectedIndex < 0) {
                cmbOwner.BorderBrush = Brushes.Red;
                ret = false;
            }
            if (cmbStatus.Text == null || cmbStatus.Text == "") {
                cmbStatus.BorderBrush = Brushes.Red;
                ret = false;
            }
            // ensures in progress and not started tasks do not have completed dates
            if (dtCompleteDate.SelectedDate != null && cmbStatus.Text != "Completed")
            {
                MessageBox.Show("Tasks that are not marked as completed cannot have a completed date.");
                dtCompleteDate.SelectedDate = null;              
                ret = false;
            }
            // ensures completed tasks have a completed date
            if (cmbStatus.Text == "Completed" && dtCompleteDate.SelectedDate == null)
            {
                dtCompleteDate.BorderBrush = Brushes.Red;
                ret = false;
            }
            // Ensures max>=most>=min
            if (float.Parse(tbxMinDuration.Text) > float.Parse(tbxMaxDuration.Text))
            {
                tbxMinDuration.BorderBrush = Brushes.Red;
                tbxMaxDuration.BorderBrush = Brushes.Red;
                MessageBox.Show("Minimum Duration must be less than Maximum Duration");
                ret = false;
            }
            else if (float.Parse(tbxMostLikelyDuration.Text) > float.Parse(tbxMaxDuration.Text))
            {
                tbxMostLikelyDuration.BorderBrush = Brushes.Red;
                tbxMaxDuration.BorderBrush = Brushes.Red;
                MessageBox.Show("Most Likely Duration must be equal to or less than Maximum Duration");
                ret = false;
            }
            else if (float.Parse(tbxMinDuration.Text) > float.Parse(tbxMostLikelyDuration.Text))
            {
                tbxMinDuration.BorderBrush = Brushes.Red;
                tbxMostLikelyDuration.BorderBrush = Brushes.Red;
                MessageBox.Show("Minimum Duration must be equal to or less than Most Likely Duration");
                ret = false;
            }

            return ret;
        }

        //By Levi Delezene
        private Task createTask() {
            if (!validateInput()) return null;

            Task task = new Task {
                Name = tbxTaskName.Text,
                ProjectId = _chart.Project.Id,
                Description = tbxTaskDescription.Text,
                MinDuration = float.Parse(tbxMinDuration.Text),
                MaxDuration = float.Parse(tbxMaxDuration.Text),
                MostLikelyDuration = float.Parse(tbxMostLikelyDuration.Text),
                StartedDate = dtStartDate.SelectedDate,
                CompletedDate = dtCompleteDate.SelectedDate,
                Owner = (User)cmbOwner.Items[cmbOwner.SelectedIndex]
            };

            //Add dependent Tasks
            if (cmbTasks.SelectedItems != null)
            {
                task.DependentTasks.Clear();
                foreach (Task _task in cmbTasks.SelectedItems)
                {
                    task.DependentTasks.Add(_task);
                }
            }

            //Maybe find a better way to do this
            switch (cmbStatus.Text) {
                case "Not Started":
                    task.Status = Status.Not_Started;
                    break;
                case "In Progress":
                    task.Status = Status.In_Progress;
                    break;
                case "Completed":
                    task.Status = Status.Completed;
                    break;
            }

            if (task.DependentTasks.Count == 0)
                task.RootNode = true;

            return task.save();
        }

        private Task editTask() {
            if (!validateInput()) return null;
           
            taskToEdit.Name = tbxTaskName.Text;
            taskToEdit.Description = tbxTaskDescription.Text;
            taskToEdit.MinDuration = float.Parse(tbxMinDuration.Text);
            taskToEdit.MaxDuration = float.Parse(tbxMaxDuration.Text);
            taskToEdit.MostLikelyDuration = float.Parse(tbxMostLikelyDuration.Text);
            taskToEdit.StartedDate = dtStartDate.SelectedDate;
            taskToEdit.CompletedDate = dtCompleteDate.SelectedDate;
            taskToEdit.Owner = (User)cmbOwner.SelectedItem;

            //Add dependent Tasks
            taskToEdit.DependentTasks.Clear();
            foreach (object task in cmbTasks.SelectedItems)
            {
                taskToEdit.DependentTasks.Add((Task)task);
            }

            switch (cmbStatus.Text) {
                case "Not Started":
                    taskToEdit.Status = Status.Not_Started;
                    break;
                case "In Progress":
                    taskToEdit.Status = Status.In_Progress;
                    break;
                case "Completed":
                    taskToEdit.Status = Status.Completed;
                    break;
            }

            return taskToEdit.save();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e) {
            //try {
                Task task;
                if (taskToEdit != null) {
                    task = editTask();
                } else {
                    task = createTask();
                }

                if(task != null) {
                    _chart.DrawGraph(_chart.GetTasksAndDependanciesFromDatabase());
                    Close();
                }

            //} catch (Exception excep) {
            //    MessageBox.Show(excep.ToString());
            //}
        }



        //By Levi Delezene
        private void btnSubmitAndClose_Click(object sender, RoutedEventArgs e) {
            try {
                Task task;
                if (taskToEdit != null) {
                    task = editTask();
                } else {
                    task = createTask();
                }

            } catch (Exception excep) {
                MessageBox.Show(excep.ToString());
            } finally {
                Close();
            }
        }

        //By Levi Delezene
        private void btnSubmitAndContinue_Click(object sender, RoutedEventArgs e) {
            try {
                Task task;
                if (taskToEdit != null) {
                    task = editTask();
                } else {
                    task = createTask();
                }
            } catch (Exception excep) {
                MessageBox.Show(excep.ToString());
            } finally {
                //There's probably a better way to do this
                Close();
                new frmTask(_chart).Show();
            }
        }

        //By Levi Delezene
        private void numberValidation(object sender, TextCompositionEventArgs e) {
            MainWindow.numberValidation(sender, e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            tbxTaskName.Focus();
            OUser userDAL = new OUser();
            List<User> userList = userDAL.Select();
            cmbOwner.ItemsSource = userList;

            cmbTasks.ItemsSource = new ObservableCollection<object>(Tasks);
            if (taskToEdit != null)
            {
                tbxTaskName.Text = taskToEdit.Name;
                tbxTaskDescription.Text = taskToEdit.Description;
                tbxMaxDuration.Text = taskToEdit.MaxDuration.ToString();
                tbxMinDuration.Text = taskToEdit.MinDuration.ToString();
                tbxMostLikelyDuration.Text = taskToEdit.MostLikelyDuration.ToString();
                cmbStatus.SelectedIndex = cmbStatus.Items.IndexOf(taskToEdit.Status);
                cmbOwner.SelectedIndex = cmbOwner.Items.IndexOf(taskToEdit.Owner);
                cmbTasks.SelectedItems = new ObservableCollection<object>(taskToEdit.DependentTasks);
                dtStartDate.SelectedDate = taskToEdit.StartedDate;
                dtCompleteDate.SelectedDate = taskToEdit.CompletedDate;
            }
        }


    }
}
