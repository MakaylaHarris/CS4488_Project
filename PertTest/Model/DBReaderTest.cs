using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pert.Model;

namespace PertTest.Model
{
    [TestClass]
    public class DBReaderTest : DBUpdateReceiver
    {
        private DBReader reader;

        public DBReaderTest()
        {
            reader = DBReader.Instantiate(this);
        }

        #region Tests
        [TestMethod]
        public void TestNewConnectionTest()
        {
            string cantConnect = @"BadConnect";
            Assert.IsFalse(reader.TestNewConnection(cantConnect));
        }

        [TestMethod]
        public void TestLogin()
        {
            reader.Register("TestUser", "Pass", "Name", "Email");
            Assert.IsTrue(reader.Login("TestUser", "Pass"));
            Assert.IsFalse(reader.Login("NotAUser", "Pass"));
            Assert.IsFalse(reader.Login("TestUser", "IncorrectPass"));
        }

        [TestMethod]
        public void TestRegister()
        {
            // setup: These user names must not be in the database!
            string unregistered = "UnregisteredUser";
            string notAUser = "NotAUser";
            reader.CreateUser(unregistered);

            // Already registered test
            Assert.IsFalse(reader.Register("TestUser", "Pass", "Mischievious OR SELECT 1;", "email"));
            // Register Test
            Assert.IsTrue(reader.Register(notAUser, "Pass", "Name", "Email"));
            reader.DeleteUser(notAUser);        // Immediately delete so it doesn't stay in database
            Assert.IsTrue(reader.Register(unregistered, "Pass", "Name", "Email"));
            reader.DeleteUser(unregistered);    // Immediately delete so it doesn't stay in database
        }

        #endregion


        public void OnDBUpdate(Project p)
        {
            Console.WriteLine("DBReader sent update");
        }

    }
}
