using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;
using Utility.FTP;
using WinSCP;

namespace UtilityTests.FTP
{
    [TestClass]
    public class FtpClientTests
    {
        /// <summary>
        /// The test needs Syncplify.me installed in local system.
        /// </summary>
        [TestMethod]
        public void FtpUploadTest()
        {
            string url = "localhost";
            string remoteDir = "/";
            string id = "admin";
            string pw = "admin";
            string sourceFile = @"C:\Users\joe\Documents\Temp\test.csv";
            var ftpClient = new FtpClient(url, remoteDir, id, pw);
            ftpClient.FtpUpload(sourceFile);

            // If there is no error, it is assumed as true
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void FtpScpUploadTest()
        {
            string url = "localhost";
            string remoteDir = "/";
            string id = "admin";
            string pw = "admin";
            string sourceFileName = @"C:\Users\joe\Documents\Temp\test.csv";
            var ftpClient = new SFtpClient(url, id, pw, Protocol.Ftp);
            ftpClient.FtpUpload(sourceFileName, remoteDir);

            // If there is no error, it is assumed as true
            Assert.IsTrue(true);
        }
    }
}