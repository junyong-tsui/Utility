using System.Collections.Generic;
using WinSCP;

namespace Utility.FTP
{
    public class SFtpClient : Ftp
    {
        private readonly SessionOptions _sessionOptions;

        public SFtpClient(string hostname, string username, string password, Protocol protocol)
        {
            _sessionOptions = new SessionOptions
            {
                Protocol = protocol,
                HostName = hostname,
                UserName = username,
                Password = password,
            };
        }

        // S-FTP connection
        public SFtpClient(string hostname, string username, string password, string hostKey, string privateKeyPath)
        {
            if (string.IsNullOrEmpty(hostKey))
            {
                _sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = hostname,
                    UserName = username,
                    Password = password,
                    GiveUpSecurityAndAcceptAnySshHostKey = true,
                    SshPrivateKeyPath = privateKeyPath,
                };
            }
            else
            {
                _sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = hostname,
                    UserName = username,
                    Password = password,
                    SshHostKeyFingerprint = hostKey,
                    SshPrivateKeyPath = privateKeyPath,
                };
            }
        }

        // FTP-S connection
        public SFtpClient(string hostname, string username, string password, FtpSecure ftpSecure, int portNumber)
        {
            _sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = hostname,
                UserName = username,
                Password = password,
                FtpSecure = ftpSecure,
                PortNumber = portNumber,
                GiveUpSecurityAndAcceptAnyTlsHostCertificate = true
            };
        }

        public List<string> FtpUpload(string localPath, string remotePath)
        {
            var uploadedFiles = new List<string>();

            using (var session = new Session())
            {
                // Connect
                session.Open(_sessionOptions);

                // Upload files
                var transferOptions = new TransferOptions
                {
                    TransferMode = TransferMode.Binary
                };

                TransferOperationResult transferResult = session.PutFiles(localPath, remotePath, false, transferOptions);

                // Throw on any error
                transferResult.Check();

                // Print results
                foreach (TransferEventArgs transfer in transferResult.Transfers)
                {
                    uploadedFiles.Add(transfer.FileName);
                }
            }

            return uploadedFiles;
        }
    }
}