using Microsoft.VisualStudio.TestTools.UnitTesting;
using PertTest.DAL;
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
    public class UserTest : DBUpdateReceiver
    {
        private DBReader reader;

        #region Constructor
        public UserTest()
        {
            reader = DBReader.Instantiate(this);
        }

        [ClassInitialize]
        public static void init(TestContext context)
        {
            new TestDB(new List<string> { "some_user.sql" });
        }
        #endregion

        #region Test Methods
        [TestMethod]
        public void Test_Register()
        {
            string name = "TestUser_33018t5";
            string pass = "TestPass3330";
            string email = "User@Test.com";
            // Now we should be able to register and test
            Assert.IsTrue(reader.Register(name, pass, name, email));
        }

        [TestMethod]
        public void Test_CreateUserRegister()
        {
            string newName = "Mindy";
            Assert.IsTrue(SmartPert.Model.Model.Instance.CreateUser(newName) != null);
            // Register Test
            Assert.IsTrue(reader.Register(newName, "password", newName, "MindyLoo@gmail.com"));
        }

        [TestMethod]
        public void Test_BadLogin()
        {
            Assert.IsFalse(reader.Login("some_user", "WrongPass"));
            Assert.IsFalse(reader.Login("WrongUser", "password"));
        }

        [TestMethod]
        public void Test_Login()
        {
            Assert.IsTrue(reader.Login("some_user", "password"));
        }

        public void OnDBDisconnect()
        {
            throw new NotImplementedException();
        }

        public void OnDBUpdate(Project p)
        {
            Console.WriteLine("updated");
        }

        #endregion

    }
}
