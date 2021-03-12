using SmartPert.Command;
using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class TaskEditor : Window, IItemObserver
    {
        private Task task;
        private bool isLoading;
        private static DateTime defaultStartDate = DateTime.Now;

        #region Properties
        private Task Task { 
            get => task;
            set {
                task = value;
                task.Subscribe(this);
                LoadTaskData(task);
            }
        }
        /// <summary>
        /// Users assigned to task
        /// </summary>
        public ObservableCollection<User> Assignees { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for Task Editor, task is required for editing
        /// </summary>
        /// <param name="task">The underlying task</param>
        public TaskEditor(Task task = null)
        {
            InitializeComponent();
            DataContext = this;
            isLoading = false;
            Owner = Application.Current.MainWindow;
            Assignees = new ObservableCollection<User>();
            if (task != null)
            {
                this.Task = task;
                LoadTaskData(task);
            } else      // Set a default start that makes sense (now)
            {
                StartDate.SelectedDate = defaultStartDate;
            }
        }

        private void LoadTaskData(Task t)
        {
            isLoading = true;
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
            isLoading = false;
            UpdatePopup();
        }
        #endregion

        #region Commands
        private void createTask()
        {
            CreateTaskCmd cmd = new CreateTaskCmd(TaskName.Text, (DateTime)StartDate.SelectedDate, EndDate.SelectedDate,
                MostLikelyDuration.Value, MaxDuration.Value, MinDuration.Value, TaskDescription.Text);
            if(cmd.Run())
            {
                Task = cmd.Task;
                LoadTaskData(task);
            }
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
            else if ((task == null || TaskName.Text != task.Name) && !Model.Model.Instance.IsValidTaskName(TaskName.Text))
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
            if (StartDate.SelectedDate < Model.Model.Instance.GetProject().StartDate)
            {
                ValidateLabel.Content = "Task start date must be after project start date.";
                result = false;
            }
            return result;
        }
        #endregion

        #region Event Handlers
        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (/*!AssigneePopup.IsFocused && */!cb_assign.IsMouseOver && !IsMouseOver)
            {
                // Todo: Known issue when creating task and pressing tab this throws null exception
                try
                {
                    Close();
                }
                catch (InvalidOperationException) { 
                }
            }
        }


        /// <summary>
        /// Call this every time an update occurs to a task field
        /// </summary>
        private void On_Input()
        {
            if (!isLoading && isValidInput())
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
            else
                On_Input();
        }

        private void On_Likely_Change(object sender, int val)
        {
            // Ensure it's between min and max
            if (val < MinDuration.Value)
                MinDuration.Value = val;
            else if (val > MaxDuration.Value)
                MaxDuration.Value = val;
            else
                On_Input();
        }

        private void On_Max_Change(object sender, int val)
        {
            // Ensure Max >= most likely
            Console.WriteLine(val);
            if (val < MostLikelyDuration.Value)
                MostLikelyDuration.Value = val;
            else
                On_Input();
        }
        private void EndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (task == null || EndDate.SelectedDate != task.EndDate)
                On_Input();
        }

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (task == null || StartDate.SelectedDate != task.StartDate)
            {
                defaultStartDate = (DateTime)StartDate.SelectedDate;  // update default start
                On_Input();
            }
        }

        private void TaskDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            if (task == null || TaskDescription.Text != task.Description)
                On_Input();
        }

        private void TaskName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (task == null || TaskName.Text != task.Name)
                On_Input();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (task != null)
                task.UnSubscribe(this);
        }

        private void UpdatePopup()
        {
            Assignees.Clear();
            foreach(User user in task.Workers)
                Assignees.Add(user);
            ObservableCollection<object> items = cb_assign.Items;
            items.Clear();
            foreach (object o in task.Proj.Workers)
                items.Add(o);
        }

        //private void AssignBtn_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    if (task != null)
        //    {
        //        AssigneePopup.IsOpen = true;
        //    }
        //}
        //private void AssigneePopup_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (!cb_assign.IsFocused && !AssigneePopup.IsFocused && !AssignBtn.IsMouseOver && !AssigneePopup.IsMouseOver)
        //        AssigneePopup.IsOpen = false;
        //}

        private void RM_Assignee(object sender, RoutedEventArgs e)
        {
            // Find user and remove from task assignees
            new RemoveWorkerCmd(Task, (User)((Button)sender).DataContext).Run();
        }
        #endregion

        #region Public methods
        public void Assign_to(object selected, string text)
        {
            User u = null;
            if (selected != null)
                u = (User)selected;
            else if (text != "")
                u = Model.Model.Instance.CreateOrGetUser(text);
            if (u != null)
            {
                if (!(new AddWorkerCmd(task, u).Run()))
                {
                    ValidateLabel.Content = "User " + u + " is assigned";
                }
            }
        }

        /// <summary>
        /// When the task has been updated
        /// </summary>
        /// <param name="item">task</param>
        public void OnUpdate(IDBItem item)
        {
            LoadTaskData((Task)item);
        }

        /// <summary>
        /// IItemObserver interface: Close the window if task has been deleted
        /// </summary>
        /// <param name="item"></param>
        public void OnDeleted(IDBItem item)
        {
            Close();
        }
        #endregion

    }
}
