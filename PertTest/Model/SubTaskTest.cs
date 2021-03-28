using Microsoft.VisualStudio.TestTools.UnitTesting;
using PertTest.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace PertTest.Model
{
    /// <summary>
    /// Summary description for SubTaskTest
    /// Created 3/28/2021 by Robert Nelson
    /// </summary>
    [TestClass]
    public class SubTaskTest
    {
        public SubTaskTest()
        {
        }

        [ClassInitialize]
        public static void init(TestContext context)
        {
            new TestDB(new List<string> { "project_boat.sql" });
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            //
            // TODO: Add test logic here
            //
        }
    }
}
