using System.Windows;
using System.Windows.Controls;

namespace SmartPert.View.Login
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        private readonly LoginWindow window;

        /// <summary>
        /// Register page part of login window
        /// </summary>
        /// <param name="window">LoginWindow</param>
        public RegisterPage(LoginWindow window)
        {
            InitializeComponent();
            this.window = window;
        }

        #region Private Methods
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            window.SwitchToLogin();
        }

        private bool isValidInput(bool ignoreNull)
        {
            if (!IsValidUsername(ignoreNull))
                return false;
            if (!IsValidEmail(ignoreNull))
                return false;
            if (!PasswordsMatch(ignoreNull))
                return false;
            status.Content = "";
            return true;
        }

        private bool IsValidEmail(bool ignoreNull)
        {
            string address = email.Text;
            if (address == "" && ignoreNull)
                return true;
            if(!window.IsValidEmail(address))
            {
                status.Content = "Invalid email '" + email.Text + "'";
                return false;
            }
            return true;
        }

        private bool IsValidUsername(bool ignoreNull)
        {
            string name = username.Text;
            if (name == "" && ignoreNull)
                return true;
            if(!window.IsValidUserName(name))
            {
                status.Content = "Invalid username '" + name + "'";
                return false;
            }
            return true;
        }

        private bool PasswordsMatch(bool ignoreNull)
        {
            string pass = password.Password;
            string verified = confirmPassword.Password;
            if (ignoreNull && (pass == "" || verified == ""))
                return true;
            if(pass != verified)
            {
                status.Content = "Passwords don't match!";
                return false;
            }
            return true;
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            if (isValidInput(ignoreNull: false))
            {
                //encryot
                string pwEncrypt = System.Text.Encoding.ASCII.GetString(
                    new System.Security.Cryptography.SHA256Managed().ComputeHash(
                        Encoding.UTF8.GetBytes(password.Password)));
                window.Register(username.Text, pwEncrypt, email.Text, fullname.Text);
            }
                
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            window.Cancel();
        }

        private void textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            isValidInput(ignoreNull: true);
        }

        #endregion

    }
}
