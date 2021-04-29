using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace SmartPert.View.Account
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Page
    {
        private AccountEditor window;

        public ChangePassword(AccountEditor window)
        {
            InitializeComponent();
            this.window = window;
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            changesSaved.Visibility = Visibility.Hidden;
            Error.Visibility = Visibility.Hidden;
            PwMismatch.Visibility = Visibility.Hidden;
            window.SwitchToAccount();
        }

        private void ChangePw_OnClick(object sender, RoutedEventArgs e)
        {
            string hashValue = System.Text.Encoding.ASCII.GetString(new System.Security.Cryptography.SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(PassBoxCurrent.Password.ToString())));
            window.HandEncryptedPw(hashValue);

            PassBoxCurrent.Clear();
        }
    }
}
