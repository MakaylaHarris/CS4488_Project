using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PertTest.Model
{
    /// <summary>
    /// Class for testing users
    /// Created 2/22/2021 by Robert Nelson
    /// </summary>
    [TestClass]
    public class UserTest
    {
        private string name;
        private string pass;
        private string email;
        private User user;
        private bool isCreated;

        #region Constructor
        public UserTest()
        {
            name = "TestUser_33018t5";
            pass = "TestPass3330";
            email = "User@Test.com";
            isCreated = false;
        }

        ~UserTest()
        {
            if (isCreated)
            {
                user.Delete();
            }
        }
        #endregion

        #region Public Methods
        public User Create()
        {
            if(!isCreated)
            {
                Assert.IsTrue(SmartPert.Model.Model.Instance.Register(name, email, pass, name));
                user = SmartPert.Model.Model.Instance.GetCurrentUser();
                isCreated = true;
            }
            return user;
        }
        #endregion
    }
}
