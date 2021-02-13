using SmartPert.Command;
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
            MinDuration.Value = t.MinDuration;
            MostLikelyDuration.Value = t.LikelyDuration;
            MaxDuration.Value = t.MaxDuration;
        }
        #endregion

        private void createTask()
        {
            if (task != null)
                throw new Exception("Task " + task.Name + " already exists in editor, cannot recreate!");
            CreateTaskCmd cmd = new CreateTaskCmd(model, TaskName.Text, (DateTime)StartDate.SelectedDate, EndDate.SelectedDate,
                MostLikelyDuration.Value, MaxDuration.Value, MinDuration.Value, TaskDescription.Text);
            cmd.Run();
            task = cmd.Task;
            CreatedLabel.Content = "Created " + task.CreationDate.ToString() + " by " + task.Creator.Name;
        }

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
            else if (!model.IsValidTaskName(TaskName.Text))
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

        #region Event Handlers

        private void On_Min_Change(object sender, int val)
        {
            // Ensure that min is <= the others
            if (val > MostLikelyDuration.Value)
                MostLikelyDuration.Value = val;
        }

        private void On_Likely_Change(object sender, int val)
        {
            // Ensure it's between min and max
            if (val < MinDuration.Value)
                MinDuration.Value = val;
            if (val > MaxDuration.Value)
                MaxDuration.Value = val;
        }

        private void On_Max_Change(object sender, int val)
        {
            // Ensure Max >= most likely
            if (val < MostLikelyDuration.Value)
                MostLikelyDuration.Value = val;
        }

        private void AssignBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
