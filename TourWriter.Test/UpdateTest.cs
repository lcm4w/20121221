using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TourWriter.Test
{
    [TestClass]
    public class UpdateTest
    {
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

        string baseUrl = "http://support.tourwriter.com/";
        private string standardUserId = "dummy id";
        private string devUserId = "13b8e136-405f-402f-a4bb-3913879be702";

        [TestMethod]
        public void Default_Update()
        {
            string response = GetDownloadString(baseUrl + "update.aspx");

            if (response.ToLower().Contains("exception"))
                Assert.Fail(response);

            Assert.IsTrue(
                response.ToLower().EndsWith(".exe"), 
                "Update reponse did not contain '.exe'");
        }

        [TestMethod]
        public void StandardUser_UpdateRequired()
        {
            string response = GetDownloadString(baseUrl +
                "update.aspx?guid=" + standardUserId + "&version=0.0");

            if (response.ToLower().Contains("exception"))
                Assert.Fail(response);

            Assert.IsTrue(
                !response.ToLower().Contains("beta") && 
                response.ToLower().EndsWith(".exe"),
                "Update reponse should contain '.beta' and/or '.exe'");
        }

        [TestMethod]
        public void StandardUser_UpdateNotRequired()
        {
            string response = GetDownloadString(baseUrl +
                "update.aspx?guid=" + standardUserId + "&version=9999.9999");

            if(response.ToLower().Contains("exception"))
                Assert.Fail(response);

            Assert.IsTrue(
                response.ToLower().Contains("none"), 
                "Update reponse did not contain 'none'");
        }

        [TestMethod]
        public void DevUser_UpdateRequired()
        {
            string response = GetDownloadString(baseUrl +
                "update.aspx?guid=" + devUserId + "&version=0.0");

            if (response.ToLower().Contains("exception"))
                Assert.Fail(response);

            Assert.IsTrue(
                response.ToLower().Contains("beta") && 
                response.ToLower().EndsWith(".exe"), 
                "Update reponse should contain '.beta' and/or '.exe'");
        }

        [TestMethod]
        public void DevUser_UpdateNotRequired()
        {
            string response = GetDownloadString(baseUrl +
                "update.aspx?guid=" + devUserId + "&version=9999.9999");

            if (response.ToLower().Contains("exception"))
                Assert.Fail(response);

            Assert.IsTrue(
                response.ToLower().Contains("none"), 
                "Update reponse should contain 'none'");
        }


        static string GetDownloadString(string url)
        {
            try
            {
                return new System.Net.WebClient().DownloadString(url);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
