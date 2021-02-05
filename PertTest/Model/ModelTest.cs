using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WPF.Model;
using WPF.View;

namespace PertTest.Model
{
    [TestClass]
    public class ModelTest : IViewModel
    {
        private WPF.Model.IModel model;
        public ModelTest()
        {
            model = new WPF.Model.Model(this);
        }

        [TestMethod]
        public void TestSetConnectionString()
        {
            string cantConnect = @"BadConnect";
            Assert.IsFalse(model.SetConnectionString(cantConnect));
        }

        #region Interface Methods
        public bool IsConnected()
        {
            throw new NotImplementedException();
        }

        public void OnModelUpdate(Project p)
        {
            Console.WriteLine("Update received");
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public bool SetConnectionString(string s)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
