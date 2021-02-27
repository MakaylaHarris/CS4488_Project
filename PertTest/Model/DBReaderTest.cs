using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPert.Model;

namespace PertTest.Model
{
    [TestClass]
    public class DBReaderTest : DBUpdateReceiver
    {
        private DBReader reader;
        private string name;
        private string pass;
        private string email;

        public DBReaderTest()
        {
            reader = DBReader.Instantiate(this);
            name = "SomeRandomUser5551";
            pass = "Pass";
            email = "somethin@somethin.co";
        }

        #region Tests
        [TestMethod]
        public void TestNewConnectionTest()
        {
            Assert.IsTrue(reader.Connected);
        }

        [TestMethod]
        public void TestLoginRegisterDelete()
        {
            // First Setup
            Unregister();
            // Now we should be able to register and test
            Assert.IsTrue(reader.Register(name, pass, name, email));
            // And Login
            Assert.IsTrue(reader.Login(name, pass));
            // And Delete
            Assert.IsTrue(reader.DeleteUser(name));

        }

        [TestMethod]
        public void TestCreateUserRegister()
        {
            Unregister();
            Assert.IsTrue(SmartPert.Model.Model.Instance.CreateUser(name) != null);
            // Register Test
            Assert.IsTrue(reader.Register(name, pass, name, email));
            reader.DeleteUser(name);
        }

        [TestMethod]
        public void TestBadLogin()
        {
            Assert.IsFalse(reader.Login(name, "WrongPass"));
            Assert.IsFalse(reader.Login("WrongUser", pass));
        }
        #endregion

        private void Unregister()
        {
            // see if the user is already in database
            if (reader.Login(name, pass))
                reader.DeleteUser(name);

        }

        public void OnDBUpdate(Project p)
        {
            Console.WriteLine("DBReader sent update");
        }

        public void OnDBDisconnect()
        {
            throw new NotImplementedException();
        }
    }
}
