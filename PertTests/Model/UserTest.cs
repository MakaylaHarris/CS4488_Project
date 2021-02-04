using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WPF.Model;

namespace PertTests.Model
{
    [TestClass]
    public class UserTest : User
    {
        public UserTest(string name, string email = "", string password = "", string username = "") : base(name, email, password, username)
        {
            this.Name = name;
            this.Email = Email;
            this.Password = Password;
            this.Username = Username;
        }
        UserTest userTest1 = new UserTest("Dan");

        [TestMethod]
        public void ShowUsers()
        {
            userTest1.Insert();
            
        }
    }
}
