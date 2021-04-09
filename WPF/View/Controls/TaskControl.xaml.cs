using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using SmartPert.Command;
using SmartPert.Model;
using SmartPert.View.Pages;
using SmartPert.View.ViewClasses;
using SmartPert.View.Windows;

namespace SmartPert.View.Controls
{
    /// <summary>
    /// Shifters control what to shift on the task when user clicks and drags
    /// </summary>
    public enum Shifter
    {
        MinEstShifter = 1,
        LikelyEstShifter = 2,
        MaxEstShifter = 4, 
        StartDateShifter = 8,
        EndDateShifter = 16,
        StartDateExtender = 32  // This one is to freeze the estimation dates and only move the start date
    }
    /// <summary>
    /// Interaction logic for TaskControl.xaml
    /// Created 3/5/2021 by Robert Nelson
    /// </summary>
    public partial class TaskControl : Connectable, IItemObserver
    {
        private RowData rowData;
        private Color completedColor;
        private Color incompleteColor;
        private Color color;
        private Brush minColor;
        private Brush maxColor;
        private Brush likelyColor;
        private Point dragStartPoint;
        private Shifter shifter;
        private TaskControlPreview preview;
        private Task task;

        #region Properties
        /// <summary>
        /// The Brush if complete
        /// </summary>
        public Color CompletedColor { get => completedColor; set { completedColor = value; LoadBrush(rowData); } }
        /// <summary>
        /// The Brush if incomplete
        /// </summary>
        public Color IncompleteColor { get => incompleteColor; set { incompleteColor = value; LoadBrush(rowData); } }

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
        public WorkSpace WorkSpace { get; }

        /// <summary>
        /// The task associated
        /// </summary>
        public RowData RowData
        {
            get => rowData;
            set
            {
                rowData = value;
                if(rowData != null)
                {
                    LoadData(rowData);
                }
            }
        }

        public Task Task { get => task; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public TaskControl(WorkSpace workSpace, RowData rowData = null)
        {
            InitializeComponent();
            DataContext = this;
            WorkSpace = workSpace;
            task = (Task)rowData.TimedItem;
            this.RowData = rowData;
            CompletedColor = ((SolidColorBrush)FindResource("SecondaryHueMidBrush")).Color;
            IncompleteColor = ((SolidColorBrush) FindResource("PrimaryHueMidBrush")).Color;
            AddAnchor(LeftAnchor);
            AddAnchor(RightAnchor);
        }

        #endregion

        #region Private Methods
        public static int NaturalNum(int n) => n > 0 ? n : 1;
        private void SetColSpans(int likely, int min, int max)
        {
            Grid.SetColumnSpan(MinRect, NaturalNum(min));
            Grid.SetColumnSpan(LikelyRect, NaturalNum(likely));
            Grid.SetColumn(LikelyRect, min);
            Grid.SetColumnSpan(MaxRect, NaturalNum(max));
            Grid.SetColumn(MaxRect, likely + min);
        }

        private void LoadBrush(RowData rowData)
        {
            if (rowData != null && Task.IsComplete)
            {
                Color = CompletedColor;
                CompletionRect.Fill = new SolidColorBrush(CompletedColor);
                CompletionRect.Visibility = Visibility.Visible;
                Grid.SetColumnSpan(CompletionRect, NaturalNum(rowData.EndDateSpan));
            } else
            {
                Color = IncompleteColor;
                CompletionRect.Visibility = Visibility.Hidden;
            }
        }

        private void LoadData(RowData rowData)
        {
            // Create the grid
            MyGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < NaturalNum(rowData.ColSpan); i++)
                MyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            SetColSpans(rowData.LikelyEstSpan, rowData.MinEstSpan, rowData.MaxEstSpan);
            LoadBrush(rowData);
            OnMove();
        }


        private void mi_editTask_Click(object sender, RoutedEventArgs e)
        {
            StateSwitcher.Instance.OnTaskCreateOrEdit(Task);
        }

        private void mi_deleteTask_Click(object sender, RoutedEventArgs e)
        {
            
            new DeleteTaskCmd(Task).Run();
        }

        private bool InRange(double clickedX, double hitRange, double elementX)
        {
            return clickedX > elementX - hitRange && clickedX < elementX + hitRange;
        }

        // Determine what to shift based on the clicked position on grid
        private Shifter DetermineShifter(Point clickedPosition)
        {
            Shifter shifter = 0;
            double x = clickedPosition.X;
            double colWidth = MyGrid.ColumnDefinitions[0].ActualWidth;
            double hitRange = colWidth / 4;
            double completedHit = rowData.EndDateSpan * colWidth;
            if (x < hitRange)
                shifter = Shifter.StartDateExtender;
            else if(Task.IsComplete && clickedPosition.Y < MyGrid.RowDefinitions[0].ActualHeight 
                && InRange(x, hitRange, completedHit))    // Near End Date Rect?
            {
                shifter = Shifter.EndDateShifter;
            } else
            {
                if (InRange(x, hitRange, Task.MinDuration * colWidth))
                    shifter |= Shifter.MinEstShifter;
                if (InRange(x, hitRange, Task.LikelyDuration * colWidth))
                    shifter |= Shifter.LikelyEstShifter;
                if (InRange(x, hitRange, Task.MaxDuration * colWidth))
                    shifter |= Shifter.MaxEstShifter;
                if (shifter == 0) // default: shift start date
                {
                    shifter = Shifter.StartDateShifter;
                }
            }
            return shifter;
        }

        private void PerformHorizontalShift(Shifter shifter, int horizontalShift)
        {
            if(horizontalShift != 0)
            {
                // Horizontal Shift
                DateTime start;
                DateTime? end;
                int likely, min, max;
                if ((shifter & Shifter.StartDateExtender) > 0)
                {
                    start = Task.StartDate.AddDays(horizontalShift);
                    end = Task.EndDate;
                    likely = NaturalNum(Task.LikelyDuration - horizontalShift);
                    min = NaturalNum(Task.MinDuration - horizontalShift);
                    max = NaturalNum(Task.MaxDuration - horizontalShift);
                }
                else
                {
                    start = (shifter & Shifter.StartDateShifter) > 0 ? Task.StartDate.AddDays(horizontalShift) : Task.StartDate;
                    end = (shifter & Shifter.EndDateShifter) > 0 ? ((DateTime)Task.EndDate).AddDays(horizontalShift) : Task.EndDate;
                    likely = (shifter & Shifter.LikelyEstShifter) > 0 ? Task.LikelyDuration + horizontalShift : Task.LikelyDuration;
                    min = (shifter & Shifter.MinEstShifter) > 0 ? Task.MinDuration + horizontalShift : Task.MinDuration;
                    max = (shifter & Shifter.MaxEstShifter) > 0 ? Task.MaxDuration + horizontalShift : Task.MaxDuration;
                }
                if (end != null && end < start)
                    return;
                new EditTaskCmd(Task, Task.Name, start, end, likely, max, min, Task.Description).Run();
            }
        }

        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(WorkSpace.mainGrid);
            // Determine what we're dragging
            shifter = DetermineShifter(e.GetPosition(this.MyGrid));
            if (shifter == Shifter.StartDateShifter)
            {
                if (preview != null)
                    EndPreview();
                preview = new TaskControlPreview(this, Canvas, e.GetPosition(WorkSpace.MainCanvas));
            } 
            else
            {
                EndPreview();
            }
            CaptureMouse();
        }

        private void UserControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (preview != null)
                preview.Location = e.GetPosition(WorkSpace.MainCanvas);
        }


        private void UserControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point endPoint = e.GetPosition(WorkSpace.mainGrid);
            if(endPoint != dragStartPoint)
            {
                int verticalShift = WorkSpace.GetRowShift(dragStartPoint.Y, endPoint.Y);
                if(verticalShift != 0)
                {
                    // Test if we should add as subtask or just shift
                    new ShiftToRowCmd(Task, task.ProjectRow + verticalShift).Run();
                } else
                {
                    int horizontalShift = WorkSpace.GetColumnShift(dragStartPoint.X, endPoint.X);
                    PerformHorizontalShift(shifter, horizontalShift);
                }
            }
            EndPreview();
            ReleaseMouseCapture();
        }

        private void EndPreview()
        {
            Canvas.Children.Remove(preview);
            preview = null;
        }

        private void UserControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new TaskEditor(Task).ShowDialog();
        }

        private void Add_New_Click(object sender, RoutedEventArgs e)
        {
            new TaskEditor().ShowDialog();
        }

        #endregion

        #region Public Methods
        public override bool CanConnect(Connectable target, Anchor targetAnchor, bool isReceiver)
        {
            return base.CanConnect(target, targetAnchor, isReceiver) && (isReceiver || (!isReceiver && task.CanAddDependency(((TaskControl)target).Task)));
        }
        /// <summary>
        /// Connect dependencies
        /// </summary>
        /// <param name="control">the task control that is a dependent</param>
        public void ConnectDependentControl(TaskControl control)
        {
                RightAnchor.Connect(control.LeftAnchor);
        }

        public override void OnConnect(Anchor sender, Connectable target, bool isReceiver)
        {
            if (!isReceiver)
                new AddDependencyCmd(Task, ((TaskControl)target).Task).Run();
                
        }

        public override void OnDisconnect(Anchor sender, Connectable target, bool isReceiver)
        {
            if (!isReceiver)
                new RemoveDependencyCmd(Task, ((TaskControl)target).Task).Run();
        }

        public void OnUpdate(IDBItem item)
        {
            //LoadData(RowData);
        }

        public void OnDeleted(IDBItem item)
        {
        }

        #endregion

    }
}
