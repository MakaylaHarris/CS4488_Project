using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
        private AccountViewModel viewModel;

        public AccountEditor()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            viewModel = new AccountViewModel();
            DataContext = viewModel;
            accountInfoPage = new AccountInfo(this);
            Content = accountInfoPage;
        }

        /// <summary>
        /// Switches page states to password page 
        /// </summary>
        public void SwitchToPassword()
        {
            changePasswordPage = new ChangePassword(this);
            viewModel.IsUpdated = false;
            this.Content = changePasswordPage;
           
        }

        /// <summary>
        /// Switches page states to account info page 
        /// </summary>
        public void SwitchToAccount()
        {
            viewModel.IsError = false;
            viewModel.IsPwMismatch = false;
            viewModel.IsPwUpdated = false;
            viewModel.TempUser.NewPw = "";
            viewModel.TempUser.ConfirmNewPw = "";
            this.Content = accountInfoPage;
        }

        /// <summary>
        /// Gets the encrypted password from the window and hands it to the viewmodel temp user for comparison
        /// </summary>
        /// <param name="encryptedPw"></param>
        public void HandEncryptedPw(string encryptedPw)
        {
            viewModel.TempUser.NewPw = System.Text.Encoding.ASCII.GetString(
                new System.Security.Cryptography.SHA256Managed().ComputeHash(
                    Encoding.UTF8.GetBytes(viewModel.TempUser.NewPw)));
            viewModel.TempUser.ConfirmNewPw = System.Text.Encoding.ASCII.GetString(
                new System.Security.Cryptography.SHA256Managed().ComputeHash(
                    Encoding.UTF8.GetBytes(viewModel.TempUser.ConfirmNewPw)));
            viewModel.TempUser.CurrentPw = encryptedPw;


        }
    }
}
