using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace SmartPert.View.Pages
{
    /// <summary>
    /// Interaction logic for WorkSpace.xaml
    /// Implemented by: Makayla Linnastruth
    /// </summary>
    public partial class WorkSpace : Page
    {
        WorkSpaceViewModel viewModel;
        private List<string> test = new List<string>();
        //starts the grid content rows at 2 since the headers take up two rows
        //starts the grid content columns at 1 since the Project/task column is the first
        private const int rowStart = 2;
        private const int colStart = 1;
        //This is the max integer number used to mean span all
        private const int maxInt = 2147483647;
        private List<int> weekendCols = new List<int>();

        public WorkSpace()
        {
            InitializeComponent();
            viewModel = new WorkSpaceViewModel();
            DataContext = viewModel.RowData;
            BuildGrid();
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
            for (int i = 0; i < viewModel.RowData.Count; i++)
            {
                //TODO: add different time case scenarios
                Button MyControl = new Button();
                MyControl.Margin = new Thickness(0, 10, 0, 10);
                MyControl.MinWidth = 0;

                Grid.SetRow(MyControl, i + rowStart);
                Grid.SetColumn(MyControl, viewModel.RowData[i].StartDateCol);
                if (viewModel.RowData[i].ColSpan != 1)
                {
                    Grid.SetColumnSpan(MyControl, viewModel.RowData[i].ColSpan);
                    MyControl.ToolTip = viewModel.TooltipData[i].OutputToolTip();
                }
                //Grid.SetZIndex(MyControl, 100);
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
                mainGrid.RowDefinitions.Add(rowDef);
                TextBlock txt1 = new TextBlock();
                Binding b1 = new Binding("Name");
                b1.Source = viewModel.RowData[i];
                txt1.SetBinding(TextBlock.TextProperty, b1);
                if (viewModel.RowData[i].IsProject)
                {
                    txt1.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
                }
                txt1.Margin = new Thickness(10,0,0,0);
                txt1.FontSize = 16;
                txt1.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRow(txt1, rowChange);
                Grid.SetColumn(txt1, 0);
                mainGrid.Children.Add(txt1);
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
            mainGrid.Children.Add(splitter);

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
        /// Adds primary light hue grid borders to main workspace
        /// </summary>
        private void AddGridBorders()
        {
            for (int i = rowStart; i < mainGrid.RowDefinitions.Count; i++)
            {
                Border border = new Border();

                // Create a primary hue light Brush  
                SolidColorBrush primaryLight = (SolidColorBrush)Application.Current.Resources["PrimaryHueLightBrush"];
                border.BorderThickness = new Thickness(0.3);
                border.Opacity = 0.5;
                border.BorderBrush = primaryLight;

                Grid.SetRow(border, i);
                Grid.SetColumnSpan(border, maxInt);
                mainGrid.Children.Add(border);
            }

            for (int i = colStart; i < mainGrid.ColumnDefinitions.Count; i++)
            {
                Border border = new Border();

                // Create a primary hue light Brush  
                SolidColorBrush primaryLight = (SolidColorBrush)Application.Current.Resources["PrimaryHueLightBrush"];
                border.BorderThickness = new Thickness(0.3);
                border.Opacity = 0.5;
                if (weekendCols.Contains(i))
                {
                    border.Background = (SolidColorBrush)Application.Current.Resources["PrimaryHueDarkBrush"];
                    border.Opacity = 0.25;
                }

                border.BorderBrush = primaryLight;
                Grid.SetZIndex(border, 0);
                Grid.SetColumn(border, i);
                Grid.SetRow(border, 1);
                Grid.SetRowSpan(border, maxInt);
                mainGrid.Children.Add(border);
            }
        }
    }
}
