using SmartPert.Command;
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
    public partial class ProjectCreator : Window
    {
        private readonly Model.Model model;
        /// <summary>
        /// Constructor
        /// </summary>
        public ProjectCreator()
        {
            InitializeComponent();
            model = Model.Model.Instance;
            StartDatePicker.SelectedDate = DateTime.Now;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate == null)
                StartDatePicker.SelectedDate = DateTime.Now;
            if (Validate(false))
                if (!new CreateProjectCmd(PrjName.Text, (DateTime)StartDatePicker.SelectedDate, EndDatePicker.SelectedDate, PrjDescription.Text).Run())
                    ValidateLbl.Content = "Oops, we weren't able to create that";
                else
                    Close();
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => Validate();

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => Validate();

        private void PrjName_TextChanged(object sender, TextChangedEventArgs e) => Validate();
        
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
            else if (!model.IsValidProjectName(PrjName.Text))
            {
                ValidateLbl.Content = "A project with that name already exists";
                return false;
            }
            ValidateLbl.Content = "";
            return true;
        }
    }
}
