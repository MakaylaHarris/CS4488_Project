using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private bool _isUpdated;
        private bool _isPwUpdated;
        private bool _isError;
        private bool _isPwMismatch;
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
                OnPropertyChanged("IsPwUpdated");
            }
        }

        /// <summary>
        /// Gets or sets a bool indicating if there is a general error with updating the password
        /// </summary>
        public bool IsError
        {
            get { return _isError; }
            set
            {
                _isError = value;
                OnPropertyChanged("IsError");
            }
        }

        /// <summary>
        /// Gets or sets a bool indicating if the new password fields are mismatched
        /// </summary>
        public bool IsPwMismatch
        {
            get { return _isPwMismatch; }
            set
            {
                _isPwMismatch = value;
                OnPropertyChanged("IsError");
            }
        }

        /// <summary>
        /// Gets a bool that returns if the password can be updated or not
        /// This is where you would have conditionals if you need specific password formatting (certain number of characters, etc.)
        /// </summary>
        public bool CanUpdatePw {
            get
            {
                if (User == null)
                {
                    IsError = true;
                    return false;
                }
                else if (TempUser.NewPw != TempUser.ConfirmNewPw)
                {
                    IsPwMismatch = true;
                    return false;
                }
                else
                {
                    return !String.IsNullOrWhiteSpace(TempUser.ConfirmNewPw);
                }
            }
        }

        #region INotifyPropertyChanged Members
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public void SaveAccountInfo()
        {
            User.Name = TempUser.Name;
            User.Email = TempUser.Email;
            IsUpdated = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SavePasswordInfo()
        {
            //TODO: Get rid of encryption once the passwords are updated
            if (TempUser.CurrentPw == User.Password || TempUser.CurrentPw ==
                System.Text.Encoding.ASCII.GetString(
                    new System.Security.Cryptography.SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(User.Password)))
            )
            {
                User.Password = TempUser.NewPw;
                IsPwMismatch = false;
                IsPwUpdated = true;
                IsUpdated = false;
                IsError = false;

            }
            else
            {
                IsPwMismatch = false;
                IsPwUpdated = false;
                IsUpdated = false;
                IsError = true;
            }
        }
    }
}
