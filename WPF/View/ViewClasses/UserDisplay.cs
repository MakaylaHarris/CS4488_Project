using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SmartPert.Model;

namespace SmartPert.View.ViewClasses
{
    /// <summary>
    /// Intermediary class for updating user details since user class is updated live
    /// This will allow users to get validation for updates
    /// Created BY: Makayla Linnastruth 03/14/2021
    /// </summary>
    class UserDisplay
    {
        private string _name;
        private string _email;
        private string _password;

        public UserDisplay(User user)
        {
            _name = user.Name;
            _email = user.Email;
            _password = user.Password;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }
}
