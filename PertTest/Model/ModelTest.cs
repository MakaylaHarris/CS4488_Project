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

        public void OnModelUpdate(Project p)
        {
            Console.WriteLine("Update received");
        }

        [TestMethod]
        public void TestSetConnectionString()
        {
            string cantConnect = @"Data Source = (LocalDB)\\MSSQLLocalDB; Database = ABadNameThatDoesntWork; Integrated Security = True;Timeout=1";
            Assert.IsFalse(model.SetConnectionString(cantConnect));
        }
    }
}
