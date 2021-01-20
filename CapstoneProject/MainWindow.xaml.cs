using CapstoneProject.Models;
using CapstoneProject.Pages;
using CapstoneProject.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Task = CapstoneProject.Models.Task;

namespace CapstoneProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Project project;
        public Chart chart;

        public MainWindow()
        {
            InitializeComponent();
            IntroPage introPage = new IntroPage(this);
            frameMain.Content = introPage;
        }

        //By Levi Delezene
        private void mi_exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //By Levi Delezene
        private void mi_projectProperties_Click(object sender, RoutedEventArgs e)
        {
            frameMain.Content = new ProjectProperties(ProjectProperties.Mode.UPDATE, project, this);
        }

        //By Brad Tyler
        private void go_main_Click(object sender, RoutedEventArgs e)
        {
            save_project.IsEnabled = false;
            mi_projectProperties.IsEnabled = false;
            IntroPage introPage = new IntroPage(this);
            frameMain.Content = introPage;
            chart = null;
            project = null;

        }
/*
        //By Brad Tyler
        private void delete_project_Click(object sender, RoutedEventArgs e)
        {
            return;
        }
*/

        //By Brad Tyler
        private void save_project_Click(object sender, RoutedEventArgs e)
        {
            project.save();
            //return;
        }

        //By Levi Delezene
        private void mi_openProject_Click(object sender, RoutedEventArgs e)
        {
            save_project.IsEnabled = true;
            frameMain.Content = new Chart(project);
        }

        //By Levi Delezene
        public static void numberValidation(object sender, TextCompositionEventArgs e)
        {
            //This allows for multiple '.' in the input. Could probably find something nicer
            e.Handled = new Regex("[^0-9.]+").IsMatch(e.Text);
        }

/*
        /// <summary>
        /// Shows minimum duration subtasks---By Alankar Pokhrel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mi_showMin_Click(object sender, RoutedEventArgs e)
        {
            
            //Selected duration
            mi_showMin.Header= "[✓]   Show Min. Estimates";
            mi_showMax.Header = "[  ]   Show Max. Estimates";
            mi_showMostLikely.Header = "[  ]   Show Most Likely";
            
            if(project == null)
            {
                return;
            }

            Chart newChart = new Chart(project, "minDuration");
            frameMain.Content = newChart;
        }
        /// <summary>
        /// Shows maximum duration subtasks----By Alankar Pokhrel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mi_showMax_Click(object sender, RoutedEventArgs e)
        {
            //Selected duration
            mi_showMin.Header = "[  ]   Show Min. Estimates";
            mi_showMax.Header = "[✓]   Show Max. Estimates";
            mi_showMostLikely.Header = "[  ]   Show Most Likely";

            if (project == null)
            {
                return;
            }

            Chart newChart = new Chart(project, "maxDuration");
            frameMain.Content = newChart;

        }
        /// <summary>
        /// Shows most likely duration subtasks----By Alankar Pokhrel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mi_showMostLikely_Click(object sender, RoutedEventArgs e)
        {
            //Selected duration
            mi_showMin.Header = "[  ]   Show Min. Estimates";
            mi_showMax.Header = "[  ]   Show Max. Estimates";
            mi_showMostLikely.Header = "[✓]   Show Most Likely";

            if (project == null)
            {
                return;
            }

            Chart newChart = new Chart(project, "mostLikelyDuration");
            frameMain.Content = newChart;
        }

        private void mi_showMax_Click_1(object sender, RoutedEventArgs e)
        {

        }
*/
        private void mi_addUser_Click(object sender, RoutedEventArgs e)
        {
            new frmAddUser().ShowDialog();
        }
    }
}
