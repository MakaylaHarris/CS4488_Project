using System;
using System.Collections.Generic;
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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            drawCalendar(14, DateTime.Now);
        }

        private void drawCalItems(int days, DateTime start)
        {
            CalItems.ColumnDefinitions.Clear();
            CalItems.RowDefinitions.Clear();
            for(int i = 0; i < days; i++)
                CalItems.ColumnDefinitions.Add(new ColumnDefinition());
            CalItems.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < 5; i++)
            {
                // Arbitrarily using Label
                var x = new Label();
                x.Content = i.ToString();
                CalItems.Children.Add(x);
                x.Margin = new Thickness(15);
                x.Background = Brushes.BlueViolet;
                x.Opacity = 0.3;
                // Some arbitrary dates
                DateTime startDate = DateTime.Now.AddDays(i);
                DateTime endDate = DateTime.Now.AddDays(i + 5-i);
                // calculate span
                Grid.SetColumnSpan(x, GetDaySpan(startDate, endDate));   // span multiple dates
                Grid.SetColumn(x, GetDaySpan(DateTime.Now, startDate));       // Set the date column
                // Put each item on a new row
                CalItems.RowDefinitions.Add(new RowDefinition());
                Grid.SetRow(x, i + 1);
            }

        }

        private int GetDaySpan(DateTime begin, DateTime end)
        {
            return (int)(end - begin).TotalDays;
        }

        private void drawCalendar(int days, DateTime start)
        {
            Cal.ColumnDefinitions.Clear();
            for (int i = 0; i < days; i++, start = start.AddDays(1))
            {
                Cal.ColumnDefinitions.Add(new ColumnDefinition());
                CalendarDay day = new CalendarDay(start);
                Cal.Children.Add(day);
                Grid.SetColumn(day, i);
            }
            drawCalItems(days, start);
        }
    }
}
