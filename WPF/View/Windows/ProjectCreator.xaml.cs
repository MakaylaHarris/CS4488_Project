using SmartPert.Command;
using SmartPert.Model;
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
using System.Windows.Shapes;

namespace SmartPert.View.Windows
{
    /// <summary>
    /// Interaction logic for ProjectCreator.xaml
    /// Created 2/19/2021 by Robert Nelson
    /// </summary>
    public partial class ProjectCreator : Window, IItemObserver
    {
        private Project project;
        private bool isEditMode;
        private bool isLoading;

        #region Properties
        /// <summary>
        /// The project associated (if any)
        /// </summary>
        public Project Project { get => project; 
            set { 
                project = value;
                if(project != null)
                    Load_Project(project);
                IsEditMode = project != null;
            }
        }

        private bool IsEditMode
        {
            get => isEditMode;
            set
            {
                isEditMode = value;
                if(isEditMode)
                {
                    Title = "Project Editor";
                    SubmitBtn.Visibility = Visibility.Hidden;
                    CancelBtn.Visibility = Visibility.Hidden;
                    project.Subscribe(this);
                }
                else
                {
                    Title = "Project Creator";
                    SubmitBtn.Visibility = Visibility.Visible;
                    CancelBtn.Visibility = Visibility.Visible;
                    if(project != null)
                        project.UnSubscribe(this);
                }

            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ProjectCreator()
        {
            InitializeComponent();
            StartDatePicker.SelectedDate = DateTime.Now;
        }


        /// <summary>
        /// Destructor
        /// </summary>
        ~ProjectCreator()
        {
            if (isEditMode)
                project.UnSubscribe(this);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// When the project is updated load data
        /// </summary>
        /// <param name="item">project</param>
        public void OnUpdate(IDBItem item)
        {
            Load_Project((Project)item);
        }

        /// <summary>
        /// When the project has been deleted, close window
        /// </summary>
        /// <param name="item">project</param>
        public void OnDeleted(IDBItem item)
        {
            Close();
        }

        #endregion

        #region Private Methods
        private void Load_Project(Project p)
        {
            isLoading = true;
            PrjDescription.Text = p.Description;
            PrjName.Text = p.Name;
            StartDatePicker.SelectedDate = p.StartDate;
            EndDatePicker.SelectedDate = p.EndDate;
            isLoading = false;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate == null)
                StartDatePicker.SelectedDate = DateTime.Now;
            if (Validate(false))
            {
                if (isEditMode)     // No action since edits are done live
                    Close();
                else if (!new CreateProjectCmd(PrjName.Text, (DateTime)StartDatePicker.SelectedDate, EndDatePicker.SelectedDate, PrjDescription.Text).Run())
                    ValidateLbl.Content = "Oops, we weren't able to create that";
                else
                    Close();
            }
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ValidateAndUpdate();

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ValidateAndUpdate();

        private void PrjDescription_LostFocus(object sender, RoutedEventArgs e) => ValidateAndUpdate();

        private void PrjName_LostFocus(object sender, TextChangedEventArgs e) => ValidateAndUpdate();


        
        private bool ValidateAndUpdate()
        {
            if (isLoading)
                return false;
            bool result = Validate();
            if (result && isEditMode)
            {
                new EditProjectCmd(Project, PrjName.Text, (DateTime) StartDatePicker.SelectedDate, EndDatePicker.SelectedDate, PrjDescription.Text).Run();
            }
            return result;
        }

        private bool Validate(bool ignoreEmptyName=true)
        {
            if (EndDatePicker.SelectedDate != null && EndDatePicker.SelectedDate < StartDatePicker.SelectedDate)
            {
                ValidateLbl.Content = "End date must be after start date";
                return false;
            }
            if (PrjName.Text == "")
            {
                if (!ignoreEmptyName)
                {
                    ValidateLbl.Content = "Name cannot be empty!";
                    return false;
                }
            }
            else if ((!IsEditMode || Project.Name != PrjName.Text) && !Model.Model.Instance.IsValidProjectName(PrjName.Text))
            {
                ValidateLbl.Content = "A project with that name already exists";
                return false;
            }
            ValidateLbl.Content = "";
            return true;
        }
        #endregion

    }
}
