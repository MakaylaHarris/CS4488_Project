using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPert.Model;
using PertTest.DAL;

namespace PertTest.Model
{
    [TestClass]
    public class DBReaderTest : DBUpdateReceiver
    {
        private DBReader reader;

        public DBReaderTest()
        {
            new TestDB();
            reader = DBReader.Instantiate(this);
        }

        #region Tests
        [TestMethod]
        public void Test_NewConnection()
        {
            Assert.IsTrue(reader.Connected);
        }
        #endregion

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
