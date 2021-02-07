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

namespace SmartPert.View.Login
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private LoginWindow window;

        /// <summary>
        /// The login part of the login window
        /// </summary>
        /// <param name="window">parent window</param>
        public LoginPage(LoginWindow window)
        {
            InitializeComponent();
            this.window = window;
            UserName.Text = Properties.Settings.Default.UserName;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!window.Login(UserName.Text, Password.Password))
                errMessage.Visibility = Visibility.Visible;
        }
 
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            window.Cancel();
        }

        private void RegisterSwitch_Click(object sender, RoutedEventArgs e)
        {
            window.SwitchToRegister();
        }
    }
}
