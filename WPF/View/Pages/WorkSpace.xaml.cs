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
    /// </summary>
    public partial class WorkSpace : Page
    {
        WorkSpaceViewModel viewModel;
        private List<string> test = new List<string>();

        public WorkSpace()
        {
            InitializeComponent();
            viewModel = new WorkSpaceViewModel();
            DataContext = viewModel.RowData;
            BuildGrid();
        }
        private void BuildGrid()
        {
            int rowStart = 2;
            int colStart = 1;
            AddRows(rowStart);
            AddHeaders(colStart, rowStart);
            addTaskControls();
        }

        private void AddHeaders(int colStart, int rowStart)
        {
            int colPosition = colStart;
            List<int> weekendCols = new List<int>();
            for (int i = 0; i < viewModel.Headers.Count; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                mainGrid.ColumnDefinitions.Add(colDef);
                TextBlock txt1 = new TextBlock();
                txt1.Text = viewModel.Headers[i];
                txt1.Margin = new Thickness(0, 0, 0, 0);
                Grid.SetRow(txt1, 0);
                Grid.SetColumn(txt1, colPosition);
                mainGrid.Children.Add(txt1);

                for (int j = 0; j < 7; j++)
                {
                    ColumnDefinition colDef2 = new ColumnDefinition();
                    mainGrid.ColumnDefinitions.Add(colDef2);
                    TextBlock txt2 = new TextBlock();
                    txt2.Text = viewModel.weekDayAbbrev[j];
                    if (viewModel.weekDayAbbrev[j] == "S")
                    {

                        weekendCols.Add(j + colStart);
                    }
                    txt2.Margin = new Thickness(0, 0, 0, 0);
                    Grid.SetRow(txt2, 1);
                    Grid.SetColumn(txt2, colPosition + j);
                    mainGrid.Children.Add(txt2);
                }

                Grid.SetColumnSpan(txt1, 7);
                colPosition += 7;
            }
        }

        private void colorGrid(List<int> weekendCols)
        {
            for (int i = 0; i < mainGrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < mainGrid.RowDefinitions.Count; j++)
                {
                    //TODO: add panels inside cells to display background for weekends, add border for today's date
                }
            }
        }

        private void addTaskControls()
        {
            for (int i = 0; i < viewModel.RowData.Count; i++)
            {
                //TODO: add task buttons
                Button MyControl = new Button();
                MyControl.Margin = new Thickness(0, 2, 0, 2);

                Grid.SetRow(MyControl, i + 2);
                Grid.SetColumn(MyControl, viewModel.RowData[i].StartDateCol);
                Grid.SetColumnSpan(MyControl, viewModel.RowData[i].ColSpan);
                mainGrid.Children.Add(MyControl);
            }
        }

        private void AddRows(int rowStart)
        {
            //Set brush to Material Theme Primary Hue Mid Brush
            SolidColorBrush MyBrush;
            MyBrush = (SolidColorBrush)Application.Current.Resources["PrimaryHueMidBrush"];

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
                Grid.SetRow(txt1, rowStart);
                Grid.SetColumn(txt1, 0);
                mainGrid.Children.Add(txt1);
                rowStart += 1;
                
            }
            GridSplitter splitter = new GridSplitter()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Right,
                Background = MyBrush,
                Width = 4,
                Margin = new Thickness(10, 0, 10, 0)
            };

            mainGrid.Children.Add(splitter);
            Grid.SetColumn(splitter, 0);
            Grid.SetRowSpan(splitter, 2147483647);
        }

        private void AddTodayBorder()
        {
            //if (DateTime.Today.Date - )
            //Border b1 = new Border();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //Set brush to Material Theme Primary Hue Mid Brush
            SolidColorBrush MyBrush;
            MyBrush = (SolidColorBrush) Application.Current.Resources["PrimaryHueMidBrush"];
            
        }



    }
}
