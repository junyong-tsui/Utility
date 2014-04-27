using System;
using System.IO;
using System.Net;
using System.Text;

namespace Utility
{
    public class FtpClient
    {
        private string Url { get; set; }

        private string RemoteDir { get; set; }

        private string FtpId { get; set; }

        private string FtpPassword { get; set; }

        private bool IsPassive { get; set; }

        private FtpWebRequest FtpRequest { get; set; }

        # region Constructor

        public FtpClient(string url, string remoteDir, string id, string pw, string sourceFileName)
        {
            Url = url;
            RemoteDir = remoteDir;
            FtpId = id;
            FtpPassword = pw;
            IsPassive = true;

            RemoteDir = RemoveUnnecessaryCharacterfromRemoteDir(RemoteDir);

            // set up ftp
            FtpRequest = (FtpWebRequest)FtpWebRequest.Create(String.Format(@"ftp://{0}/{1}/{2}", Url, RemoteDir, sourceFileName));
            FtpRequest.UsePassive = IsPassive;
            FtpRequest.KeepAlive = false;
            FtpRequest.Credentials = new NetworkCredential(FtpId, FtpPassword);
        }

        # endregion

        # region public methods

        public void FtpUpload(string sourceFullPath)
        {
            FtpRequest.Method = WebRequestMethods.Ftp.UploadFile;

            byte[] fileContents = File.ReadAllBytes(sourceFullPath);
            FtpRequest.ContentLength = fileContents.Length;

            Stream requestStream = FtpRequest.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)FtpRequest.GetResponse();

            response.Close();
        }

        public bool IsFileExist()
        {
            //Try to obtain filesize: if we get error msg containing "550"
            //the file does not exist
            try
            {
                long size = GetSize();
                return true;
            }
            catch (Exception ex)
            {
                //only handle expected not-found exception
                if (ex is System.Net.WebException)
                {
                    //file does not exist/no rights error = 550
                    if (ex.Message.Contains("550"))
                    {
                        //clear
                        return false;
                    }
                    else
                    {
                        // todo: need to check how it works
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Compare the size between source and target
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool IsFileEqual(long size)
        {
            if (GetSize() == size)
                return true;
            else
                return false;
        }

        # endregion

        # region private methods

        /// <summary>
        /// Gets the size of an file in remote server
        /// </summary>
        /// <param name="ftp"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private long GetSize()
        {
            long size;
            FtpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            using (FtpWebResponse response = (FtpWebResponse)FtpRequest.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string content = reader.ReadToEnd();
                size = content.Length;
                reader.Close();

                return size;
            }
        }

        private string RemoveUnnecessaryCharacterfromRemoteDir(string remoteDir)
        {
            if (remoteDir.IndexOf("/") == 0)
                remoteDir = remoteDir.Remove(0, 1);

            return remoteDir;
        }

        # endregion
    }
}