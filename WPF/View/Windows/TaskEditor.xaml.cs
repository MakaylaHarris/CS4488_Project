﻿using SmartPert.Command;
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


        #region Constructor
        /// <summary>
        /// Constructor for Task Editor, task is required for editing
        /// </summary>
        /// <param name="model">The underlying model</param>
        /// <param name="task">The underlying task</param>
        public TaskEditor(IModel model, Task task = null)
        {
            InitializeComponent();
            DataContext = this;
            if (model == null)
                throw new ArgumentNullException("task", "Task Editor requires model!");
            this.model = model;
            Owner = Application.Current.MainWindow;
            if(task != null)
            {
                this.task = task;
                LoadTaskData(task);
            } else      // Set a default start that makes sense (now)
            {
                DateTime projectStart = model.GetProject().StartDate;
                StartDate.SelectedDate = DateTime.Now > projectStart ? DateTime.Now : projectStart;
            }
        }

        private void LoadTaskData(Task t)
        {
            TaskName.Text = t.Name;
            TaskDescription.Text = t.Description;
            EndDate.SelectedDate = t.EndDate;
            StartDate.SelectedDate = t.StartDate;
            MaxDuration.Value = t.MaxDuration;
            MostLikelyDuration.Value = t.LikelyDuration;
            MinDuration.Value = t.MinDuration;
            CreatedLabel.Content = "Created " + t.CreationDate.ToString("d");
            if(t.Creator != null)
                CreatedLabel.Content += " by " + t.Creator.Name;
            Complete.IsChecked = t.IsComplete;
        }
        #endregion

        #region Commands
        private void createTask()
        {
            CreateTaskCmd cmd = new CreateTaskCmd(model, TaskName.Text, (DateTime)StartDate.SelectedDate, EndDate.SelectedDate,
                MostLikelyDuration.Value, MaxDuration.Value, MinDuration.Value, TaskDescription.Text);
            cmd.Run();
            task = cmd.Task;
            CreatedLabel.Content = "Created " + task.CreationDate.ToString("d") + " by " + task.Creator.Name;
        }

        private void runTaskEdit()
        {
            new EditTaskCmd(task, TaskName.Text, (DateTime) StartDate.SelectedDate, EndDate.SelectedDate,
                MostLikelyDuration.Value, MaxDuration.Value, MinDuration.Value, TaskDescription.Text).Run();
        }
        #endregion

        #region Validators
        private bool isValidInput()
        {
            if(isValidTaskName() && isValidDates())
            {
                ValidateLabel.Content = "";
                return true;
            }
            return false;
        }

        private bool isValidTaskName()
        {
            bool result = false;
            if (TaskName.Text == "")
                ValidateLabel.Content = "Task name is required!";
            else if (TaskName.Text != task.Name && !model.IsValidTaskName(TaskName.Text))
                ValidateLabel.Content = "Task name " + TaskName.Text + " is not valid";
            else
                result = true;
            return result;
        }

        private bool isValidDates()
        {
            // Start < End
            bool result = true;
            if(StartDate.SelectedDate > EndDate.SelectedDate)
            {
                ValidateLabel.Content = "Start date must be before end date";
                return false;
            }

            // Check if it's within project time frame
            if (StartDate.SelectedDate < model.GetProject().StartDate)
            {
                ValidateLabel.Content = "Task start date must be after project start date.";
                result = false;
            }
            return result;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Call this every time an update occurs to a task field
        /// </summary>
        private void On_Update()
        {
            if (isValidInput())
            {
                if (task == null)
                    createTask();
                else
                    runTaskEdit();
            }
        }


        private void Complete_Checked(object sender, RoutedEventArgs e)
        {
            bool complete = (bool)Complete.IsChecked;
            if (task != null && complete != task.IsComplete)
            {
                if (complete)
                    EndDate.SelectedDate = DateTime.Now;
                else
                    EndDate.SelectedDate = null;
            }
        }

        private void On_Min_Change(object sender, int val)
        {
            // Ensure that min is <= the others
            if (val > MostLikelyDuration.Value)
                MostLikelyDuration.Value = val;
            else if (val != task.MinDuration)
                On_Update();
        }

        private void On_Likely_Change(object sender, int val)
        {
            // Ensure it's between min and max
            if (val < MinDuration.Value)
                MinDuration.Value = val;
            else if (val > MaxDuration.Value)
                MaxDuration.Value = val;
            else if (val != task.LikelyDuration)
                On_Update();
        }

        private void On_Max_Change(object sender, int val)
        {
            // Ensure Max >= most likely
            if (val < MostLikelyDuration.Value)
                MostLikelyDuration.Value = val;
            else if (val != task.MaxDuration)
                On_Update();
        }
        private void EndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (task == null || EndDate.SelectedDate != task.EndDate)
                On_Update();
        }

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (task == null || StartDate.SelectedDate != task.StartDate)
                On_Update();
        }

        private void TaskDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            if (task == null || TaskDescription.Text != task.Description)
                On_Update();
        }

        private void TaskName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (task == null || TaskName.Text != task.Name)
                On_Update();
        }

        private void AssignBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
