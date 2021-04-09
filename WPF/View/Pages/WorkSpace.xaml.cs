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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartPert.ViewModels;
using SmartPert.View.ViewClasses;
using SmartPert.View.Controls;
using SmartPert.Model;
using System.Windows.Threading;

namespace SmartPert.View.Pages
{
    /// <summary>
    /// Interaction logic for WorkSpace.xaml
    /// Implemented by: Makayla Linnastruth
    /// </summary>
    public partial class WorkSpace : Page
    {
        WorkSpaceViewModel viewModel;
        //starts the grid content rows at 2 since the headers take up two rows
        //starts the grid content columns at 1 since the Project/task column is the first
        private const int rowStart = 2;
        private const int colStart = 0;
        //This is the max integer number used to mean span all
        private const int maxInt = 2147483647;
        private List<int> weekendCols = new List<int>();
        private Dictionary<Task, TaskControl> taskControls;

        public WorkSpace()
        {
            InitializeComponent();
            viewModel = new WorkSpaceViewModel(this);
            taskControls = new Dictionary<Task, TaskControl>();
            DataContext = viewModel.RowData;
            mainGrid.SizeChanged += MainGrid_SizeChanged;
            BuildGrid();
        }

        /// <summary>
        /// Update workspace when the model has changed
        /// Added 3/27/2021 by Robert
        /// </summary>
        /// <param name="viewModel">view model</param>
        public void OnWorkspaceModelUpdate(WorkSpaceViewModel viewModel)
        {
            foreach (TaskControl taskControl in taskControls.Values)
                taskControl.DisconnectAllLines();
            taskControls.Clear();

            // Remove any old previews
            List<UIElement> toRemove = new List<UIElement>();
            foreach(UIElement uIElement in MainCanvas.Children)
                if (uIElement.GetType() == typeof(TaskControlPreview))
                    toRemove.Add(uIElement);
            foreach (UIElement uI in toRemove)
                MainCanvas.Children.Remove(uI);

            weekendCols.Clear();
            for (int i = mainGrid.RowDefinitions.Count - 1; i >= rowStart; i--)
            {
                mainGrid.RowDefinitions.RemoveAt(i);
                LeftGrid.RowDefinitions.RemoveAt(i);
            }
            for (int i = mainGrid.ColumnDefinitions.Count - 1; i >= colStart; i--)
                mainGrid.ColumnDefinitions.RemoveAt(i);
            mainGrid.Children.Clear();
            LeftGrid.Children.Clear();
            BuildGrid();
            Dispatcher.Invoke(new Action(() => { AddDependencies(); }), DispatcherPriority.ContextIdle);
        }

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainCanvas.Width = e.NewSize.Width;
            MainCanvas.Height = e.NewSize.Height;
        }

        /// <summary>
        /// Gets the column distance between start and end points
        /// Created 3/6/2021 by Robert Nelson
        /// </summary>
        /// <param name="start">start</param>
        /// <param name="end">end</param>
        /// <returns>number of columns, negative if end is before start</returns>
        public int GetColumnShift(double start, double end)
        {
            int startCol = -1, endCol = -1, currentCol = 0;
            double totalWidth = 0;
            foreach(var columnDef in mainGrid.ColumnDefinitions)
            {
                totalWidth += columnDef.ActualWidth;
                if (startCol == -1 && totalWidth > start)
                {
                    startCol = currentCol;
                    if (endCol > 0)     // Shortcut if we found columns
                        break;
                }
                if (endCol == -1 && totalWidth > end)
                {
                    endCol = currentCol;
                    if (startCol > 0)
                        break;
                }
                ++currentCol;
            }
            if (endCol == -1)
                endCol = currentCol;
            return endCol - startCol;
        }

        /// <summary>
        /// Gets the row distance between start and end points
        /// Created 3/9/2021 by Robert Nelson
        /// </summary>
        /// <param name="start">start</param>
        /// <param name="end">end</param>
        /// <returns>number of rows, negative if end is before start</returns>
        public int GetRowShift(double start, double end)
        {
            int startRow = -1, endRow = -1, currentRow = 0;
            double totalHeight = 0;
            foreach (var rowDef in mainGrid.RowDefinitions)
            {
                totalHeight += rowDef.ActualHeight;
                if (startRow == -1 && totalHeight > start)
                {
                    startRow = currentRow;
                    if (endRow > 0)     // Shortcut if we found columns
                        break;
                }
                if (endRow == -1 && totalHeight > end)
                {
                    endRow = currentRow;
                    if (startRow > 0)
                        break;
                }
                ++currentRow;
            }
            if (endRow == -1)
                endRow = currentRow;
            return endRow - startRow;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AddDependencies();
        }

        // Added 3/8/2021 by Robert Nelson 
        // Adds the task controls dependencies
        private void AddDependencies()
        {
            foreach(KeyValuePair<Task, TaskControl> keyValue in taskControls)
            {
                if(keyValue.Key.Dependencies != null)   
                    foreach (Task t in keyValue.Key.Dependencies)
                        if (taskControls.ContainsKey(t))
                            keyValue.Value.ConnectDependentControl(taskControls[t]);
            }
        }

        private void BuildGrid()
        {
            AddRows();
            AddHeaders();
            AddGridSplitter();
            AddTaskControls();
            AddGridBorders();
            AddTodayBorder();
        }

        /// <summary>
        /// Adds Column headers into grid
        /// </summary>
        private void AddHeaders()
        {
            int colPosition = colStart;
            //outer loop adds the Date (i.e. "Feb 04") column headers
            for (int i = 0; i < viewModel.Headers.Count; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                mainGrid.ColumnDefinitions.Add(colDef);
                colDef.MinWidth = 15;
                TextBlock txt1 = new TextBlock();
                txt1.Text = viewModel.Headers[i];
                txt1.FontSize = 16;
                txt1.Margin = new Thickness(0, 0, 0, 0);
                Grid.SetRow(txt1, 0);
                Grid.SetColumn(txt1, colPosition);
                mainGrid.Children.Add(txt1);

                //inner loop adds the abbreviated day (S,M,T,W,T,F,S) column headers
                for (int j = 0; j < 7; j++)
                {
                    if (j != 0)
                    {
                        ColumnDefinition colDef2 = new ColumnDefinition();
                        mainGrid.ColumnDefinitions.Add(colDef2);
                        colDef2.MinWidth = 15;
                    }
                    TextBlock txt2 = new TextBlock();
                    txt2.Text = viewModel.weekDayAbbrev[j];
                    if (viewModel.weekDayAbbrev[j] == "S")
                    {
                        weekendCols.Add(j + colPosition);
                    }
                    txt2.Margin = new Thickness(0, 0, 0, 0);
                    txt2.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(txt2, 1);
                    Grid.SetColumn(txt2, colPosition + j);
                    mainGrid.Children.Add(txt2);
                }
                //spans outer loop headers to fill the week
                Grid.SetColumnSpan(txt1, 7);
                colPosition += 7;
            }
        }

        /// <summary>
        /// Adds Buttons to Grid for now from start to end date for project and tasks
        /// </summary>
        private void AddTaskControls()
        {
            Control MyControl;
            RowData rowData;
            for (int i = 0; i < viewModel.RowData.Count; i++)
            {
                rowData = viewModel.RowData[i];
                if (rowData.IsProject)
                    MyControl = new Button();
                else
                {
                    TaskControl control = new TaskControl(this, rowData) { Canvas = MainCanvas };
                    taskControls.Add(control.Task, control);
                    MyControl = control;
                }
                MyControl.Margin = new Thickness(0, 4, 0, 4);

                Grid.SetRow(MyControl, i + 2);
                Grid.SetColumn(MyControl, TaskControl.NaturalNum(rowData.StartDateCol));
                Grid.SetColumnSpan(MyControl, TaskControl.NaturalNum(rowData.ColSpan));
                Grid.SetZIndex(MyControl, 100);
                
                                //Tyler K.
                //Don't want task tooltips on the project "button". 
                if(i > 0)
                {
                    MyControl.ToolTip = viewModel.TooltipData[i - 1].OutputToolTip();
                }
                //If we're at index 0, it'll populate the Project "button". Add a project tooltip. 
                if(i == 0)
                {
                    MyControl.ToolTip = viewModel.ProjectTooltip.OutputToolTipProject();
                }
                
                mainGrid.Children.Add(MyControl);

            }
        }

        /// <summary>
        /// Adds Project/task rows to grid with project names and task names
        /// </summary>
        private void AddRows()
        {
            int rowChange = rowStart;
            for (int i = 0; i < viewModel.RowData.Count; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(30, GridUnitType.Pixel);
                LeftGrid.RowDefinitions.Add(rowDef);
                rowDef = new RowDefinition();
                rowDef.Height = new GridLength(30, GridUnitType.Pixel);
                mainGrid.RowDefinitions.Add(rowDef);
                if(!viewModel.RowData[i].IsProject) { 
                    TaskRowLabel label = new TaskRowLabel(viewModel.RowData[i].TimedItem as Task);
                    int subLevel = viewModel.RowData[i].SubTaskLevel;
                    label.Margin = new Thickness(10 * subLevel, 0, 0, 0);
                    label.FontSize = 16 - subLevel >= 8 ? 16 - subLevel : 8;
                    label.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(label, rowChange);
                    Grid.SetColumn(label, 0);
                    LeftGrid.Children.Add(label);
                }
                else
                {
                    TextBlock txt1 = new TextBlock();
                    Binding b1 = new Binding("Name");
                    b1.Source = viewModel.RowData[i];
                    txt1.SetBinding(TextBlock.TextProperty, b1);
                    txt1.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
                    txt1.Margin = new Thickness(5, 0, 0, 0);
                    txt1.FontSize = 16;
                    txt1.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRow(txt1, rowChange);
                    Grid.SetColumn(txt1, 0);
                    LeftGrid.Children.Add(txt1);
                }
                rowChange += 1;
                
            }
        }

        /// <summary>
        /// Adds a Grid Splitter to adjust the size of the task column in case the task names are longer
        /// </summary>
        private void AddGridSplitter()
        {
            //Set brush to Material Theme Primary Hue Mid Brush
            SolidColorBrush MyBrush;
            MyBrush = (SolidColorBrush)Application.Current.Resources["PrimaryHueMidBrush"];

            GridSplitter splitter = new GridSplitter()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Right,
                Background = MyBrush,
                Width = 4,
                Margin = new Thickness(10,0, 0, 0)
            };
            Grid.SetZIndex(splitter, 90);
            Grid.SetColumn(splitter, 0);
            Grid.SetRowSpan(splitter, maxInt);
            workSpace.Children.Add(splitter);

        }

        /// <summary>
        /// Adds a border around the column for today's date
        /// </summary>
        private void AddTodayBorder()
        {
            Border today = new Border();

            // Create a red Brush  
            SolidColorBrush redBrush = new SolidColorBrush();
            redBrush.Color = Colors.Red;

            // Set Line's width and color  
            today.BorderThickness = new Thickness(1);
            today.BorderBrush = redBrush;

            if (viewModel.TodayCol != 0)
            {
                Grid.SetColumn(today, viewModel.TodayCol);
                Grid.SetRowSpan(today, maxInt);
                mainGrid.Children.Add(today);
            }
        }

        /// <summary>
        /// Adds outer borders
        /// </summary>
        private void AddGridOuterBorder()
        {
            SolidColorBrush midbrush = FindResource("PrimaryHueMidBrush") as SolidColorBrush;
            // Border around header
            Border border = CreateBorder(mainGrid, midbrush, 2, 1);
            Grid.SetColumnSpan(border, mainGrid.ColumnDefinitions.Count);
            Grid.SetRowSpan(border, 2);
            border = CreateBorder(LeftGrid, midbrush, 2, 1);
            Grid.SetRowSpan(border, 2);

            // Border around entire grid
            border = CreateBorder(workSpace, midbrush, 1, 1);
            Grid.SetColumnSpan(border, workSpace.ColumnDefinitions.Count);
        }

        // Helper function to create borders
        private Border CreateBorder(Grid grid, SolidColorBrush brush, double thickness=0.3, double opacity=0.5)
        {
            Border border = new Border();
            border.BorderThickness = new Thickness(thickness);
            border.Opacity = opacity;
            border.BorderBrush = brush;
            grid.Children.Add(border);
            return border;
        }

        /// <summary>
        /// Adds primary light hue grid borders to main workspace
        /// </summary>
        private void AddGridBorders()
        {
            Border border;
            SolidColorBrush lightbrush = (SolidColorBrush)Application.Current.Resources["PrimaryHueLightBrush"];
            AddGridOuterBorder();
            for (int i = rowStart; i < mainGrid.RowDefinitions.Count; i++)
            {
                // Create a primary hue light Brush  
                border = CreateBorder(mainGrid, lightbrush);
                Grid.SetRow(border, i);
                Grid.SetColumnSpan(border, maxInt);
                border = CreateBorder(LeftGrid, lightbrush);
                Grid.SetRow(border, i);
            }

            for (int i = colStart; i < mainGrid.ColumnDefinitions.Count; i++)
            {
                // Create a primary hue light Brush  
                if (weekendCols.Contains(i))
                {
                    border = CreateBorder(mainGrid, lightbrush, opacity: 0.25);
                    border.Background = (SolidColorBrush)Application.Current.Resources["PrimaryHueDarkBrush"];
                } else
                    border = CreateBorder(mainGrid, lightbrush);

                Grid.SetZIndex(border, 0);
                Grid.SetColumn(border, i);
                Grid.SetRow(border, 1);
                Grid.SetRowSpan(border, maxInt);
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if(LeftView.VerticalOffset != e.VerticalOffset)
                LeftView.ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void LeftView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (ScrollViewer.VerticalOffset != e.VerticalOffset)
                ScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void BestCaseRadio_Click(object sender, RoutedEventArgs e)
        {
            Model.Task.CalculateDependentsMaxEstimate = false;
        }

        private void WorstCaseRadio_Click(object sender, RoutedEventArgs e)
        {
            Model.Task.CalculateDependentsMaxEstimate = true;
        }
    }
}
