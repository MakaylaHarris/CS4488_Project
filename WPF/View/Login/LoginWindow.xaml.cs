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
using Pert.Model;

namespace Pert.View.Login
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// Created 2/5/2021 by Robert Nelson
    /// </summary>
    public partial class LoginWindow : Window
    {
        private LoginPage loginPage;
        private RegisterPage registerPage;
        private IModel model;

        /// <summary>
        /// Initialize login process
        /// </summary>
        /// <param name="model">The underlying model</param>
        public LoginWindow(IModel model)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            loginPage = new LoginPage(this);
            this.Content = loginPage;
            this.model = model;
        }

        #region Public Methods
        /// <summary>
        /// Switches to register page
        /// </summary>
        public void SwitchToRegister()
        {
            if (registerPage == null)
                registerPage = new RegisterPage(this);
            Content = registerPage;
        }

        /// <summary>
        /// Switches to login page
        /// </summary>
        public void SwitchToLogin()
        {
            Content = loginPage;
        }

        /// <summary>
        /// Logs in
        /// </summary>
        /// <param name="username">username or email</param>
        /// <param name="password">password</param>
        /// <returns>true on success</returns>
        public bool Login(string username, string password)
        {
            bool result = model.Login(username, password);
            if(result)
            {
                Close();
            }
            return result;
        }

        /// <summary>
        /// Attempts to register
        /// </summary>
        /// <param name="username">unique username</param>
        /// <param name="password">password</param>
        /// <param name="email">unique email</param>
        /// <param name="name">name to be shown</param>
        /// <returns></returns>
        public bool Register(string username, string password, string email, string name)
        {
            bool result = model.Register(username, email, password, name);
            if (result)
                Close();
            return result;
        }

        /// <summary>
        /// Determines if username is valid, must be unique and have no @ symbol
        /// </summary>
        /// <param name="username">username</param>
        /// <returns></returns>
        public bool IsValidUserName(string username) => model.IsValidNewUsername(username);

        /// <summary>
        /// Determines if email is valid, must be unique and have @ symbol
        /// </summary>
        /// <param name="email">email</param>
        /// <returns></returns>
        public bool IsValidEmail(string email) => model.IsValidNewEmail(email);

        /// <summary>
        /// Cancels login/registration
        /// </summary>
        public void Cancel() => Close();
        #endregion
    }
}
