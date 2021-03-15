using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmartPert.Model;
using SmartPert.View.ViewClasses;
using SmartPert.ViewModelCommands;

namespace SmartPert.ViewModels
{
    /// <summary>
    /// Handles the logic for the Account View
    /// @author: Makayla Linnastruth
    /// @date: 03/14/2021
    /// </summary>
    class AccountViewModel : INotifyPropertyChanged
    {
        private User _user;
        private UserDisplay _tempUser;
        private PasswordDisplay _tempPassword;
        private bool _isUpdated;
        private bool _isPwUpdated;
        private StateSwitcher stateSwitcher;

        /// <summary>
        /// Creates and instance of the Account View Model Class
        /// </summary>
        public AccountViewModel()
        {
            _user = Model.Model.Instance.GetCurrentUser();
            _tempUser = new UserDisplay(User);
            _isUpdated = false;
            AccountUpdateCommand = new AccountUpdateCommand(this);
            PasswordUpdateCommand = new PasswordUpdateCommand(this);
        }

        /// <summary>
        /// Gets the User Instance
        /// </summary>
        public User User
        {
            get { return _user; }
        }
        
        /// <summary>
        /// Gets the UserDisplay Instance
        /// </summary>
        public UserDisplay TempUser
        {
            get { return _tempUser; }
        }

        public PasswordDisplay TempPassword
        {
            get { return _tempPassword; }
        }

        /// <summary>
        /// Gets the AccountUpdateCommand for the view model.
        /// </summary>
        public ICommand AccountUpdateCommand
        {
            get;
            private set;
        }

        public ICommand PasswordUpdateCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a bool value indicating whether the Account can be updated.
        /// </summary>
        public bool CanUpdate { get {
            if (User == null)
            {
                return false;
            }
            else
            {
                return !String.IsNullOrWhiteSpace(_tempUser.Name);
                //Do we care if email is empty?
            }
        }}

        /// <summary>
        /// Gets or sets a bool that signifies the account info is updated
        /// </summary>
        public bool IsUpdated
        {
            get { return _isUpdated; }
            set
            {
                _isUpdated = value;
                OnPropertyChanged("IsUpdated");
            }
        }

        /// <summary>
        /// Gets or sets a bool indicating the password has been updated
        /// </summary>
        public bool IsPwUpdated
        {
            get { return _isPwUpdated; }
            set
            {
                _isPwUpdated = value;
                OnPropertyChanged("IsUpdated");
            }
        }

        /// <summary>
        /// Gets a bool that returns if the password can be updated or not
        /// </summary>
        public bool CanUpdatePw {
            get
            {
                if (User == null && User.Password != TempPassword.CurrentPw || TempPassword.NewPw != TempPassword.ConfirmNewPw)
                {
                    return false;
                }
                else
                {
                    return !String.IsNullOrWhiteSpace(TempPassword.NewPw);
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SaveAccountInfo()
        {
            User.Name = TempUser.Name;
            User.Email = TempUser.Email;
            User.Password = TempUser.Password;
            IsUpdated = true;
        }

        public void SavePasswordInfo()
        {
            User.Password = TempPassword.NewPw;
            IsPwUpdated = true;
        }
    }
}
