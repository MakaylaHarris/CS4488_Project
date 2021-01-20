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
using CapstoneProject.Models;
using CapstoneProject.DAL;
using CapstoneProject.Validators;

namespace CapstoneProject.Windows
{
    /// <summary>
    /// Interaction logic for frmAddUser.xaml
    /// </summary>
    public partial class frmAddUser : Window
    {
        public frmAddUser()
        {
            InitializeComponent();
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            if(tbFirstName.Text == "" || tbFirstName.Text == null)
            {
                tbFirstName.BorderBrush = Brushes.Red;
            }
            if(tbLastName.Text == "" || tbLastName.Text == null)
            {
                tbLastName.BorderBrush = Brushes.Red;
            }
            if(tbEmail.Text == "" || tbEmail.Text == null)
            {
                tbEmail.BorderBrush = Brushes.Red;
            }
            else
            {
                OUser oUser = new OUser();
                User user = new User(tbFirstName.Text, tbLastName.Text, tbEmail.Text);
                oUser.Insert(user);
                this.Close();
            }
        }
    }
}
