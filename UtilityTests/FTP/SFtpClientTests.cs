using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.FTP;
using WinSCP;

namespace UtilityTests.FTP
{
    [TestClass]
    public class SFtpClientTests
    {
        [TestMethod]
        public void FtpSClientTest()
        {
            string serverName = "test.rebex.net";
            string userName = "demo";
            string password = "password";
            int port = 990;
            var localFileInfo = new FileInfo(@"C:\Users\joe\Documents\Temp\test.csv");
            string remoteDirectory = "";

            // FTP-S
            bool actual = false;
            var sftp = new SFtpClient(serverName, userName, password, FtpSecure.Implicit, port);
            try
            {
                sftp.FtpUpload(localFileInfo.FullName, @"/" + remoteDirectory + @"/" + localFileInfo.Name);
            }
            catch (Exception ex)
            {
                const string error = "No permission.";
                actual = ex.InnerException.Message.Contains(error);
            }

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void SFtpClientTest()
        {
            string serverName = "localhost";
            string userName = "admin";
            string password = "test";
            var localFileInfo = new FileInfo(@"C:\Users\joe\Documents\Temp\test.csv");
            string remoteDirectory = "";
            string privateKeyPath = @"C:\Users\joe\Documents\Security\SSH Keys\testPrivateKey.ppk";

            // S-FTP
            var sftp = new SFtpClient(serverName, userName, password, string.Empty, privateKeyPath);
            sftp.FtpUpload(localFileInfo.FullName, @"/" + remoteDirectory + @"/" + localFileInfo.Name);
        }
    }
}