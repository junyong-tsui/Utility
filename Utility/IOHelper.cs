// -----------------------------------------------------------------------
// <copyright file="IOHelper.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Utility
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class IOHelper
    {
        /// <summary>
        /// Reads the text content from a file
        /// </summary>
        /// <param name="template_location">the physical location of the file</param>
        /// <returns>text content</returns>
        public static string ReadTextFile(string fileLocation)
        {
            string result = null;

            if (File.Exists(fileLocation))
            {
                StreamReader sr = null;

                try
                {
                    sr = File.OpenText(fileLocation);
                    result = sr.ReadToEnd();
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Close();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Writes the text content to a file
        /// </summary>
        /// <param name="content">text content</param>
        /// <param name="file_location">the physical location of the file</param>
        public static void WriteTextFile(string content, string fileLocation)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(fileLocation);
                sw.Write(content);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
        /// <summary>
        /// Writes the text content to a file
        /// </summary>
        /// <param name="content">text content</param>
        /// <param name="file_location">the physical location of the file</param>
        public static void WriteTextFile(string content, string fileLocation, Encoding encoding)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(fileLocation, false, encoding, 4096);
                sw.Write(content);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fileLocation"></param>
        public static void AttatchToTextFile(string content, string fileLocation)
        {
            StreamWriter sw = null;

            try
            {
                sw = File.AppendText(fileLocation);
                sw.Write(content);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="fileLocation"></param>
        public static void AppendLineToTextFile(string line, string fileLocation)
        {
            StreamWriter sw = null;

            try
            {
                sw = File.AppendText(fileLocation);
                sw.WriteLine(line);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public static byte[] ReadFileStream(string fileLocation)
        {
            byte[] result = null;

            if (File.Exists(fileLocation))
            {
                FileStream fs = null;

                try
                {
                    fs = new FileStream(fileLocation, FileMode.Open);
                    result = new byte[fs.Length];
                    fs.Read(result, 0, result.Length);
                }
                catch (Exception )
                {
                    result = null;
                    // System_Reporter.ReportException(ex);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string RunExternalProcess(string command, string args)
        {
            Process process = new Process();

            process.EnableRaisingEvents = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = args;

            process.Start();

            string result = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static bool StringExists(string file, string searchText)
        {
            //Declare reader as a new StreamReader with file as the file to use
            System.IO.StreamReader reader = new System.IO.StreamReader(file);
            //Declare text as the reader reading to the end
            String text = reader.ReadLine();
            bool result = false;
            while (text == string.Empty)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(text, searchText))
                {
                    result = true;
                    break;
                }
                text = reader.ReadLine();
            }
            //If the searchText is a match
            return result;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        /// <summary>
        /// Convert a byte array to an Object
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        public static Object ByteArrayStrToObject(string arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(StringHelper.StrToByteArray(arrBytes), 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }

        public static void LogToFile(string log_file_path, string log_msg)
        {
            StreamWriter stream;
            string log_format = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";

            FileInfo f = new FileInfo(log_file_path);
            Directory.CreateDirectory(f.DirectoryName);
            if (!File.Exists(log_file_path))
            {
                stream = File.CreateText(log_file_path);
            }
            else
            {
                stream = File.AppendText(log_file_path);
            }
            try
            {
                stream.WriteLine(log_format + log_msg);
                stream.Flush();
                stream.Close();
            }
            catch (Exception)
            {
                // string errMsg = "Fail to write log file.";
                // System_Reporter.ReportException(e, errMsg, "Webstats", "");
            }
            finally
            {
                stream.Close();
            }
        }

        /// <summary>
        /// Add date at the end of original file name. If exist, overwrites
        /// </summary>
        /// <param name="fullpath"></param>
        public static string changeFileNameWithDate(string fullpath)
        {
            FileInfo fileInfo = new FileInfo(fullpath);
            string filenameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.Name);
            string extension = fileInfo.Name.Split('.').ToList().Last();
            string fullPath4NewName = fileInfo.DirectoryName + @"\" + filenameWithoutExt + "_" + DateTime.Now.ToString("yyyyMMdd") + "." + extension;

            if (File.Exists(fullPath4NewName))
                File.Delete(fullPath4NewName);

            fileInfo.MoveTo(fullPath4NewName);

            return fullPath4NewName;
        }

        /// <summary>
        /// Get file info data from a directory
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static List<FileInfo> GetFileInfos(string inputPath, string pattern)
        {
            IEnumerable<string> inputFiles = Directory.EnumerateFiles(inputPath, pattern);
            List<FileInfo> fileinfos = new List<FileInfo>();
            foreach (var item in inputFiles)
            {
                FileInfo fileinfo = new FileInfo(item);
                fileinfos.Add(fileinfo);
            }
            return fileinfos;
        }
    }
}
