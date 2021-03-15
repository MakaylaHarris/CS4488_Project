using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using SmartPert.ViewModels;

namespace SmartPert.View.Account
{
    /// <summary>
    /// Interaction logic for AccountEditor.xaml
    /// </summary>
    public partial class AccountEditor : Window
    {
        private AccountInfo accountInfoPage;
        private ChangePassword changePasswordPage;

        public AccountEditor()
        {
            InitializeComponent();
            DataContext = new AccountViewModel();
            accountInfoPage = new AccountInfo(this);
            Content = accountInfoPage;
        }

        /// <summary>
        /// Switches page states to password page 
        /// </summary>
        public void SwitchToPassword()
        {
            if (changePasswordPage == null)
            {
                changePasswordPage = new ChangePassword(this);
            }

            this.Content = changePasswordPage;
        }

        /// <summary>
        /// Switches page states to account info page 
        /// </summary>
        public void SwitchToAccount()
        {
            this.Content = accountInfoPage;
        }
    }
}
