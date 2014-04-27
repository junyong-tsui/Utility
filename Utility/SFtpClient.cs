using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WinSCP;

namespace Utility
{
    public class SFtpClient
    {
        SessionOptions sessionOptions;

        public SFtpClient(string hostname, string username, string password, Protocol protocol)
        {
            sessionOptions = new SessionOptions
            {
                Protocol = protocol,
                HostName = hostname,
                UserName = username,
                Password = password,
            };
        }

        public List<string> FtpUpload(string localPath, string remotePath)
        {
            List<string> uploadedFiles = new List<string>();

            using (Session session = new Session())
            {
                // Connect
                session.Open(sessionOptions);

                // Upload files
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Binary;

                TransferOperationResult transferResult;
                transferResult = session.PutFiles(localPath, remotePath, false, transferOptions);

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
