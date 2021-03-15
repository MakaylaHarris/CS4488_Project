using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPert.Model;

namespace SmartPert.View.ViewClasses
{
    /// <summary>
    /// Intermediary class for updating user details since password class is updated live
    /// This will allow users to get validation for updates
    /// Created BY: Makayla Linnastruth 03/14/2021
    /// </summary>
    class PasswordDisplay
    {
        private string _currentPw;
        private string _newPw;
        private string _confirmNewPw;

        public PasswordDisplay()
        {

        }

        public string NewPw
        {
            get { return _newPw; }
            set { _newPw = value; }
        }

        public string ConfirmNewPw
        {
            get { return _confirmNewPw; }
            set { _confirmNewPw = value; }
        }

        public string CurrentPw
        {
            get { return _currentPw; }
            set { _currentPw = value; }
        }
    }
}
