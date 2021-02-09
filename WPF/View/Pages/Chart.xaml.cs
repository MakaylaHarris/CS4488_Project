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
using System.Collections;
using System.Collections.ObjectModel;
using SmartPert.Model;
using SmartPert.View.Controls;

namespace SmartPert.View.Pages
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// Converted to work with SmartPert, 2/9/2021 by Robert Nelson
    /// </summary>

    //By Levi Delezene
    public partial class Chart : Page, IViewModel
    {

        List<Task> taskList;
        private IModel model;
        private Point savedMousePosition;
        private TranslateTransform move;
        private ScaleTransform zoom;
        private Project _project;

        private double dayWidth = 75;
        int buttonSpacing = 50;
        int buttonHeight = 25;

        private TaskControl adjustingTask;
        private Point previous;
        private int leftDateChange = -1;
        private int rightDateChange = -1;
        private int dayThreshold = 50;

        private Brush taskBrush;
        LinearGradientBrush taskGradient = new LinearGradientBrush();

        private Dictionary<string, int> dayMonths = new Dictionary<string, int>(); //Dictionary to add months and their respective days
        private string duration = "";
        WindowState prevWindowState = new WindowState();

        public Chart(IModel model)
        {
            InitializeComponent();

            this.model = model;
            model.Subscribe(this);
            _project = model.GetProject();
            this.PreviewMouseWheel += ZoomCanvas;
            this.MouseMove += DragCanvas;
            this.MouseUp += ReleaseMouseDrag;

            addItemsHashTable();
            // addItemsCombo();
        }

        //public Chart(Project project, string _duration)
        //{
        //    InitializeComponent();

        //    Project = project;
        //    duration = _duration;
        //    this.PreviewMouseWheel += ZoomCanvas;

        //    addItemsHashTable();
        //    // addItemsCombo();
        //}

        // Created by Sandro Pawlidis (10/15/2019)
        // Modified By: Brad Tyler
        // Date: 9/8/2020
        public int DrawGraph(List<Task> mainLevel)
        {
            mainCanvas.Children.Clear();
            int top = 50;

            //Only try to draw the tasks if there are some to draw;
            if (mainLevel.Count > 0)
            {
                for (int i = 0; i < mainLevel.Count; i++)
                {
                    int spaceUsed = DrawSubTasks(mainLevel[i], top);
                    top += (spaceUsed + 1) * (buttonHeight + buttonSpacing) + 50;
                }


                int screenHeight = top;
                mainCanvas.Height = (screenHeight > System.Windows.SystemParameters.PrimaryScreenHeight) ? screenHeight : System.Windows.SystemParameters.PrimaryScreenHeight;
                DrawCalendar(2);
            }
            else
            {
                DrawCalendar(2);
            }
            return top;
        }

        public void RefreshGraph()
        {
            DrawGraph(Project.Tasks);
        }

        // Edit Brad Tyler (10/28/2020)
        // Modified to make default max duration
        // Created by Sandro Pawlidis (10/15/2019)
        private int DrawSubTasks(Task parent, double topMargin)
        {
            double tempDuration = 0;
            // Playing with some ideas of how to implement the colors based on day length
            double maxDuration = parent.MaxDuration;
            double minDuration = parent.MinDuration;
            double mostDuration = parent.LikelyDuration;

            // Colors fromRgb
            Color maxColor = Color.FromRgb(249, 192, 192);
            Color minColor = Color.FromRgb(204, 246, 200);
            Color mostColor = Color.FromRgb(250, 252, 194);

            double newTopMargin = topMargin;
            double minWidth = int.MaxValue;

            // Day Width Calculations
            int subtaskCount = 0;
            for (int i = 0; i < parent.Dependencies.Count; i++)
            {
                subtaskCount += DrawSubTasks(parent.Dependencies[i], newTopMargin);
                Point start = new Point(((DateTime)parent.StartDate - _project.StartDate).TotalDays * dayWidth + tempDuration * dayWidth - dayWidth / 4, topMargin + buttonHeight / 2);
                Point end = new Point(((DateTime)parent.Dependencies[i].StartDate - _project.StartDate).TotalDays * dayWidth, newTopMargin + buttonHeight / 2);

                double width = start.X + (end.X - start.X) / 2;
                minWidth = (width < minWidth) ? width : minWidth;

                Point midTop = new Point(minWidth, start.Y);
                Point midBot = new Point(minWidth, end.Y);

                List<Point> points = new List<Point>();
                points.Add(start);
                points.Add(midTop);
                points.Add(midBot);
                points.Add(end);

                Path p = getPath(points);

                KeyValuePair<Task, Task> pair = new KeyValuePair<Task, Task>(parent, parent.Dependencies[i]);
                p.DataContext = pair;

                Button button = new Button();

                Canvas.SetZIndex(p, 99);
                mainCanvas.Children.Add(p);

                newTopMargin = topMargin + (i + 1) * (buttonHeight + buttonSpacing);
                newTopMargin += subtaskCount * (buttonHeight + buttonSpacing);
            }

            // Brad Tyler 11-11-2020
            // Math for offsets - Percentage of decrease from maxDuration
            double minOffset = 1 - (Math.Abs(maxDuration - minDuration) / maxDuration);
            double mostOffset = 1 - (Math.Abs(maxDuration - mostDuration) / maxDuration);

            // Making and adding gradient stopps for min/most/max
            // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.lineargradientbrush?view=net-5.0
            GradientStop minStop = new GradientStop(minColor, minOffset * .95);
            GradientStop mostStart = new GradientStop(mostColor, minOffset * 1.05);
            GradientStop mostStop = new GradientStop(mostColor, mostOffset * .95);
            GradientStop maxStart = new GradientStop(maxColor, mostOffset * 1.05);

            taskGradient = new LinearGradientBrush(minColor, maxColor, 0);

            taskGradient.GradientStops.Add(minStop);
            taskGradient.GradientStops.Add(mostStart);
            taskGradient.GradientStops.Add(mostStop);
            taskGradient.GradientStops.Add(maxStart);

            taskBrush = taskGradient;
            tempDuration = maxDuration + 1;

            //taskcontrol
            TaskControl t = new TaskControl(parent, this);
            t.taskBorder.Background = taskBrush;
            t.ToolTip = createToolTip(parent);
            t.MouseDown += resizeTask;
            Canvas.SetLeft(t, ((DateTime)parent.StartDate - _project.StartDate).TotalDays * dayWidth);
            Canvas.SetTop(t, topMargin);


            //Min/Max/mostlikely view----By Alankar Pokhrel
            // Updated by Andrew Christiansen 11/6/2020
            if (parent.EndDate == null)
            {
                t.Width = tempDuration * dayWidth - dayWidth;
            }
            else
            {
                t.Width = ((DateTime)parent.EndDate - (DateTime)parent.StartDate).TotalDays * dayWidth;

                if (((DateTime)parent.EndDate - (DateTime)parent.StartDate).TotalDays < parent.MinDuration)
                {
                    t.taskBorder.Background = new SolidColorBrush(minColor);
                }
                else if (((DateTime)parent.EndDate - (DateTime)parent.StartDate).TotalDays < parent.MinDuration + parent.LikelyDuration)
                {
                    t.taskBorder.Background = new SolidColorBrush(mostColor);
                }
                else
                {
                    t.taskBorder.Background = new SolidColorBrush(maxColor);
                }
            }


            Canvas.SetZIndex(t, 100);
            mainCanvas.Children.Add(t);

            subtaskCount += (parent.Dependencies.Count > 1) ? parent.Dependencies.Count - 1 : 0;

            return subtaskCount;
        }

        // Created by Sandro Pawlidis (11/3/2019)
        private void progressResizeTask()
        {
            Task task = (Task)adjustingTask.DataContext;
            // TODO: Check if date is decreased past parents start dated/after childs start date
            // TODO: ?? Maybe increase started date of all child tasks if parent is increased past child start date.
            // TODO: Update Tasks in database.
            double mouseX = Mouse.GetPosition(mainCanvas).X;
            double taskX = Canvas.GetLeft(adjustingTask);

            if (rightDateChange != -1)
            {
                if (Math.Abs(mouseX - previous.X) > dayThreshold)
                {
                    int direction = Math.Sign(mouseX - previous.X);
                    previous = Mouse.GetPosition(mainCanvas);

                    task.MinDuration += direction;
                    DrawGraph(taskList);
                }
            }
            else if (leftDateChange != -1)
            {
                if (Math.Abs(mouseX - previous.X) > dayThreshold)
                {
                    int direction = Math.Sign(mouseX - previous.X);
                    previous = Mouse.GetPosition(mainCanvas);

                    task.StartDate = ((DateTime)task.StartDate).AddDays(direction);
                    task.MinDuration += -direction;
                    DrawGraph(taskList);
                }
            }
        }

        // Created by Sandro Pawlidis (11/3/2019)
        private void resizeTask(object sender, RoutedEventArgs e)
        {
            TaskControl t = (TaskControl)sender;
            Task task = (Task)t.DataContext;

            double mouseX = Mouse.GetPosition(mainCanvas).X;
            double taskX = Canvas.GetLeft(t);

            double mouseOnTask = mouseX - taskX;
            previous = Mouse.GetPosition(mainCanvas);

            if (mouseOnTask < t.Width * 0.05)
            {
                leftDateChange = 0;
                adjustingTask = t;
            }

            else if (mouseOnTask > t.Width * 0.95)
            {
                rightDateChange = 0;
                adjustingTask = t;
            }

        }

        /// <summary>
        /// Written By: Chris Neeser
        /// Date: 10/22/2019
        /// Create the tool tip for a task
        /// </summary>
        /// <param name="task">The task to create the tooltip for</param>
        /// <returns>The tooltip to assign to your controls ToolTip property</returns>
        private TaskToolTip createToolTip(Task task)
        {
            TaskToolTip ttp = new TaskToolTip(task);
            ttp.Style = (Style)FindResource("AppToolTip");
            return ttp;
        }

        // Created by Sandro Pawlidis (10/15/2019)
        private Path getPath(List<Point> points)
        {
            Path path = new Path();
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 2;

            PathSegmentCollection segments = new PathSegmentCollection();
            for (int i = 1; i < points.Count; i++)
                segments.Add(new LineSegment(points[i], true));

            path.Data = new PathGeometry()
            {
                Figures = new PathFigureCollection() {
                    new PathFigure() {
                        StartPoint = points[0],
                        Segments = segments
                    }
                }
            };

            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Remove dependency";
            menuItem.Click += mi_deleteDependency_Click;
            menuItem.DataContext = path;

            ContextMenu menu = new ContextMenu();
            menu.Items.Add(menuItem);
            path.ContextMenu = menu;

            return path;
        }


        public Project Project { get => _project; set => _project = value; }

        /// <summary>
        /// Deletes a dependency
        /// Written by Levi Delezene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mi_deleteDependency_Click(object sender, RoutedEventArgs e)
        {
            Path path = (Path)((MenuItem)sender).DataContext;
            KeyValuePair<Task, Task> pair = (KeyValuePair<Task, Task>)path.DataContext;
            //Look at all tasks and remove this task^ from the one it's dependent on???
            Task parent = pair.Key;
            Task child = pair.Value;
            parent.RemoveDependency(child);
            mainCanvas.Children.Remove(path);
            DrawGraph(Project.Tasks);
        }

        private void mi_addTask_Click(object sender, RoutedEventArgs e)
        {
            //new frmTask(this).ShowDialog();
            DrawGraph(Project.Tasks);
        }

        private Line CreateLine(int xMultiplier)
        {
            Line line = new Line();
            line.Stroke = System.Windows.Media.Brushes.Cyan;
            line.StrokeThickness = 2;

            line.X1 = xMultiplier * dayWidth;
            line.X2 = line.X1;

            line.Y1 = 0;
            line.Y2 = mainCanvas.Height; //Changed .Height to .ActualHeight to make use of auto width and height - Chase

            return line;
        }

        private Label CreateLabel(string content)
        {
            Label label = new Label();
            label.Content = content;
            return label;
        }

        // Created by Sandro Pawlidis (9/25/2019)
        private void DrawCalendar(int years)
        {
            int startDay = Project.StartDate.Day;
            int startMonth = Project.StartDate.Month - 1;

            Line initialLine = CreateLine(0);

            mainCanvas.Children.Add(initialLine);

            Label initialLabel = new Label();

            initialLabel.Content = dayMonths.Keys.ElementAt(startMonth) + "\r\n" + startDay;

            Canvas.SetLeft(initialLabel, initialLine.X1);

            mainCanvas.Children.Add(initialLabel);

            startDay++;

            for (int i = 1; i < 365 * years; i++)
            {
                Line nextLine = CreateLine(i);
                mainCanvas.Children.Add(nextLine);
                Label label;

                if (startDay > dayMonths.Values.ElementAt(startMonth))
                {
                    startDay = 1;
                    startMonth++;
                }
                if (startMonth > 11)
                {
                    startMonth = 0;
                    startDay = 1;
                }
                if (startDay == 1)
                {
                    label = CreateLabel(dayMonths.Keys.ElementAt(startMonth) + "\r\n" + startDay.ToString());
                }
                else
                {
                    label = CreateLabel("\r\n" + startDay.ToString());
                }
                startDay++;
                Canvas.SetLeft(label, nextLine.X1);
                mainCanvas.Children.Add(label);
            }
        }

        private Rect dateToChartCoordinate(DateTime chartStartTime, Nullable<DateTime> taskStartTime, double duration)
        {
            Rect rect = new Rect();
            double days = ((DateTime)taskStartTime - chartStartTime).TotalDays;
            rect.X = days * dayWidth;
            rect.Width = duration * dayWidth;

            return rect;
        }

        public void DeleteTask(Task task)
        {
            model.DeleteTask(task);
        }

        public void AddTask(Task task)
        {
            MessageBox.Show(task.ToString());
            if (task == null) return;
            Rect rectVal = dateToChartCoordinate(Project.StartDate, task.StartDate, task.MaxDuration);

            Grid taskGrid = new Grid();
            Rectangle taskRect = new Rectangle();
            TextBlock taskTextBlock = new TextBlock();

            taskTextBlock.Text = task.Name;

            taskRect.Width = rectVal.Width;
            taskRect.Height = 50;
            taskRect.StrokeThickness = 2;
            taskRect.Stroke = new SolidColorBrush(Colors.Red);

            taskGrid.Children.Add(taskRect);
            taskGrid.Children.Add(taskTextBlock);

            TaskControl taskControl = new TaskControl(task, this);
            taskControl.Width = rectVal.Width;
            taskControl.Height = 50;

            Canvas.SetTop(taskControl, 30);
            Canvas.SetLeft(taskControl, rectVal.X);

            mainCanvas.Children.Add(taskControl);

        }

        // Created by Sandro Pawlidis (9/25/2019)
        private void SetupCanvas()
        {
            //Set the height so that we have a vertical scrollbar
            mainCanvas.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            mainCanvas.Width = 365 * 2 * dayWidth; //TO-DO: Necessary for MouseEvents to fire. Discuss. 

            move = new TranslateTransform();
            zoom = new ScaleTransform();

            TransformGroup group = new TransformGroup();
            group.Children.Add(move);
            group.Children.Add(zoom);

            mainCanvas.LayoutTransform = group;
            mainCanvas.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        // Created by Sandro Pawlidis (9/25/2019)
        private void ZoomCanvas(object sender, MouseWheelEventArgs e)
        {
            double zoomSpeed = 0.05;

            if (e.Delta > 0)
            {
                zoom.ScaleX += zoomSpeed;
                zoom.ScaleY += zoomSpeed;
            }
            else if (e.Delta < 0)
            {
                zoom.ScaleX -= zoomSpeed;
                zoom.ScaleY -= zoomSpeed;
            }
        }

        // Created by Sandro Pawlidis (9/25/2019)
        private void DragCanvas(object sender, RoutedEventArgs e)
        {
            if (leftDateChange != -1 || rightDateChange != -1)
            {
                progressResizeTask();
                return;
            }
        }

        // Created by Sandro Pawlidis (9/25/2019)
        private void SetMouseDrag(object sender, RoutedEventArgs e)
        {
            savedMousePosition = Mouse.GetPosition(mainCanvas);
        }

        // Created by Sandro Pawlidis (9/25/2019)
        private void ReleaseMouseDrag(object sender, RoutedEventArgs e)
        {
            //translating = false;

            if (leftDateChange != -1 || rightDateChange != -1)
                leftDateChange = rightDateChange = -1;
        }

        // Created by Chris Neeser (10/1/2019)
        Point scrollMousePoint = new Point();
        double hOff = 1;

        private void scrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            scrollMousePoint = e.GetPosition(scrollViewer);
            hOff = scrollViewer.HorizontalOffset;
            scrollViewer.CaptureMouse();
        }

        private void scrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (scrollViewer.IsMouseCaptured)
            {
                scrollViewer.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(scrollViewer).X));
            }
        }

        private void scrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            scrollViewer.ReleaseMouseCapture();
        }

        private void scrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }


        // Adds the months and days to the dictionary - Chase Torres (9/26/2019)
        private void addItemsHashTable()
        {
            dayMonths.Add("January", 31);
            dayMonths.Add("February", 28);
            dayMonths.Add("March", 31);
            dayMonths.Add("April", 30);
            dayMonths.Add("May", 31);
            dayMonths.Add("June", 30);
            dayMonths.Add("July", 31);
            dayMonths.Add("August", 31);
            dayMonths.Add("September", 30);
            dayMonths.Add("October", 31);
            dayMonths.Add("November", 30);
            dayMonths.Add("December", 31);
        }

        //Going to try to use a groupbox as the container to add tasks to the canvas
        private void addGroupBoxCanvas()
        {
            GroupBox taskGroup = new GroupBox();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            prevWindowState = Window.GetWindow(this).WindowState;
            Window.GetWindow(this).WindowState = WindowState.Maximized;

            SetupCanvas();

            if (Project != null)
            {
                taskList = Project.Tasks;
                int screenHeight = 0;

                screenHeight = DrawGraph(taskList);
            }
            Window.GetWindow(this).KeyDown += Page_KeyDown;
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Escape)
            {
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                    Window.GetWindow(this).WindowState = prevWindowState;
                    Window.GetWindow(this).KeyDown -= Page_KeyDown;
                }
            }
        }

        public void OnModelUpdate(Project p)
        {
            if (p != null)
            {
                this._project = p;
                this.DrawGraph(p.Tasks);
            }
        }
    }
}
