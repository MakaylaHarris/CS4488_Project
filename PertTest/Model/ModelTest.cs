using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SmartPert.Model;
using SmartPert.View;

namespace PertTest.Model
{
    [TestClass]
    public class ModelTest : IViewModel
    {
        private SmartPert.Model.IModel model;
        public ModelTest()
        {
            model = SmartPert.Model.Model.GetInstance(this);
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
