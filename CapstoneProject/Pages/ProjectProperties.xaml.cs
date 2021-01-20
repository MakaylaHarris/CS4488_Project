using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
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
using CapstoneProject.DAL;
using CapstoneProject.Models;
namespace CapstoneProject.Pages {
    /// <summary>
    /// Interaction logic for ProjectProperties.xaml
    /// </summary>
    
    //By Levi Delezene
    public partial class ProjectProperties : Page
    {
        Project project;
        Mode _mode;
        OProject projectDAL = new OProject();
        WindowState prevWindowState = new WindowState();
        MainWindow parent;

        public enum Mode { INSERT = 0, UPDATE = 1 }

        public string TextProp { get; set; }


        public ProjectProperties(Mode mode, Project project, MainWindow mainWindow) {
            _mode = mode;
            this.project = project;
            parent = mainWindow;
            InitializeComponent();
            this.DataContext = this;
        }

        public void numberValidation(object sender, TextCompositionEventArgs e) {
            MainWindow.numberValidation(sender, e);
        }

        private void Save() {
            try
            {
                project = createProject();
                if (_mode == Mode.INSERT)
                    NavigationService.Navigate(new Chart(project));
                else
                {
                    Window.GetWindow(this).WindowState = WindowState.Maximized;
                    Window.GetWindow(this).KeyDown -= Page_KeyDown;
                    NavigationService.GoBack();
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.ToString());
            }

        }

        private Project createProject() {

            project.Name = tbxName.Text;
            project.Description = tbxDescription.Text;
            project.WorkingHours = int.Parse(tbxWorkingHours.Text);
            project.StartDate = (DateTime)dpStartDate.SelectedDate;
            project.ProjectOwner = (User)cboOwner.Items[cboOwner.SelectedIndex];

            if (_mode == Mode.INSERT)
                project = projectDAL.Insert(project);

            if (_mode == Mode.UPDATE)
                projectDAL.Update(project);

            return project;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            prevWindowState = Window.GetWindow(this).WindowState;
            Window.GetWindow(this).WindowState = WindowState.Normal;


            OUser userDAL = new OUser();
            List<User> userList = userDAL.Select();
            foreach (User user in userList)
            {
                User userValue = new User(user.Id, user.FirstName,user.LastName);
                cboOwner.Items.Add(userValue);
            }

            if (_mode == Mode.UPDATE)
            {
                tbxName.Text = project.Name;
                tbxDescription.Text = project.Description;
                tbxWorkingHours.Text = project.WorkingHours.ToString();
                dpStartDate.SelectedDate = project.StartDate.Date;
                cboOwner.SelectedIndex = cboOwner.Items.IndexOf(project.ProjectOwner);
                //Disable startdate for being edited
                dpStartDate.IsEnabled = false;
            }

            Window.GetWindow(this).KeyDown += Page_KeyDown;
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                NavigationService.GoBack();
                Window.GetWindow(this).WindowState = prevWindowState;
                Window.GetWindow(this).KeyDown -= Page_KeyDown;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void TbxName_LostFocus(object sender, RoutedEventArgs e)
        {
            tbxName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void TbxDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            tbxDescription.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void DpStartDate_LostFocus(object sender, RoutedEventArgs e)
        {
            dpStartDate.GetBindingExpression(DatePicker.SelectedDateProperty).UpdateSource();
        }

        private void tbxWorkingHours_LostFocus(object sender, RoutedEventArgs e)
        {
            tbxWorkingHours.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void SaveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void SaveCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsValid(sender as DependencyObject);
        }

        private bool IsValid(DependencyObject obj)
        {
            // The dependency object is valid if it has no errors and all
            // of its children (that are dependency objects) are error-free.
            return !Validation.GetHasError(obj) &&
            LogicalTreeHelper.GetChildren(obj)
            .OfType<DependencyObject>()
            .All(IsValid);
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            tbxName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxDescription.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            dpStartDate.GetBindingExpression(DatePicker.SelectedDateProperty).UpdateSource();
            tbxWorkingHours.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            cboOwner.GetBindingExpression(ComboBox.TextProperty).UpdateSource();

            parent.project = this.project;
        }

        private void cboOwner_LostFocus(object sender, RoutedEventArgs e)
        {
            cboOwner.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
        }

        // Brad Tyler - 11/20/2020
        private void btnAddOwner_Click(object sender, RoutedEventArgs e)
        {
            // Brings up new user form
            new Windows.frmAddUser().ShowDialog();
            // Clears previous combo box contents
            cboOwner.Items.Clear();
            // Fills combo box with all users
            OUser userDAL = new OUser();
            List<User> userList = userDAL.Select();
            foreach (User user in userList)
            {
                User userValue = new User(user.Id, user.FirstName, user.LastName);
                cboOwner.Items.Add(userValue);
            }
        }
    }
}