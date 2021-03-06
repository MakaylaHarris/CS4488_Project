using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SmartPert.Command;
using SmartPert.Model;
using SmartPert.View.Pages;
using SmartPert.View.Windows;

namespace SmartPert.View.Controls
{
    /// <summary>
    /// Interaction logic for TaskControl.xaml
    /// Redone 3/5/2021 by Robert Nelson
    /// </summary>
    public partial class TaskControl : UserControl, IItemObserver
    {
        private Task task;
        private Color completedColor;
        private Color incompleteColor;
        private Color color;
        private Brush minColor;
        private Brush maxColor;
        private Brush likelyColor;

        #region Properties
        /// <summary>
        /// The Brush if complete
        /// </summary>
        public Color CompletedColor { get => completedColor; set { completedColor = value; LoadBrush(task); } }
        /// <summary>
        /// The Brush if incomplete
        /// </summary>
        public Color IncompleteColor { get => incompleteColor; set { incompleteColor = value; LoadBrush(task); } }

        private Color Color
        {
            get => color;
            set
            {
                color = value;
                color.A = 80;
                minColor = new SolidColorBrush(color);
                color.A = 40;
                maxColor = new SolidColorBrush(Color);
                color.A = 180;
                likelyColor = new SolidColorBrush(Color);
            }
        }

        /// <summary>
        /// Min color
        /// </summary>
        public Brush MinColor { get => minColor; set => minColor = value; }

        /// <summary>
        /// Max Color
        /// </summary>
        public Brush MaxColor { get => maxColor; set => maxColor = value; }

        /// <summary>
        /// Likely Color
        /// </summary>
        public Brush LikelyColor { get => likelyColor; set => likelyColor = value; }

        /// <summary>
        /// The task associated
        /// </summary>
        public Task Task
        {
            get => task;
            set
            {
                if (task != null)
                    task.UnSubscribe(this);
                task = value;
                if(task != null)
                {
                    task.Subscribe(this);
                    LoadTask(task);
                }
            }
        }
        #endregion

        #region Constructor
        public TaskControl()
        {
            InitializeComponent();
            DataContext = this;
            CompletedColor = Brushes.Green.Color;
            IncompleteColor = Brushes.BlueViolet.Color;
        }

        ~TaskControl()
        {
            if (Task != null)
                Task.UnSubscribe(this);
        }
        #endregion

        #region Private Methods
        private static int NaturalNum(int n) => n > 0 ? n : 1;
        private void SetColSpans(int likely, int min, int max)
        {
            Grid.SetColumnSpan(MinRect, NaturalNum(min));
            Grid.SetColumnSpan(LikelyRect, NaturalNum(likely - min));
            Grid.SetColumn(LikelyRect, min);
            Grid.SetColumnSpan(MaxRect, NaturalNum(max - likely));
            Grid.SetColumn(MaxRect, likely);
        }

        private void LoadBrush(Task task)
        {
            if(task != null && task.IsComplete)
            {
                Color = CompletedColor;
                CompletionRect.Fill = LikelyColor;
                CompletionRect.Visibility = Visibility.Visible;
                Grid.SetColumnSpan(CompletionRect, ((DateTime)task.EndDate - task.StartDate).Days);
            } else
            {
                Color = IncompleteColor;
                CompletionRect.Visibility = Visibility.Hidden;
            }
        }

        private void LoadTask(Task task)
        {
            // Create the grid
            int days = (task.CalculateLastTaskDate() - task.StartDate).Days;
            MyGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < days; i++)
                MyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            SetColSpans(task.LikelyDuration, task.MinDuration, task.MaxDuration);
            LoadBrush(task);
        }


        private void mi_editTask_Click(object sender, RoutedEventArgs e)
        {
            StateSwitcher.Instance.OnTaskCreateOrEdit(Task);
        }

        private void mi_deleteTask_Click(object sender, RoutedEventArgs e)
        {
            new DeleteTaskCmd(Task).Run();
        }
        #endregion

        #region Public Methods
        public void OnUpdate(IDBItem item)
        {
            LoadTask(task);
        }

        public void OnDeleted(IDBItem item)
        {
        }
        #endregion

    }
}
