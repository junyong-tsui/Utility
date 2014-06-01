// -----------------------------------------------------------------------
// <copyright file="CsvHelper.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Utility
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Collections.ObjectModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CsvHelper
    {
        public const char Separator = ',';
        public const char Separator_Replacement = '#';
        public const char Qualifier = '"';

        public string CsvFile { get; set; }

        public DataTable CsvTable { get; set; }

        /// <summary>
        /// Gets the list of columns of the csv file.
        /// </summary>
        public Collection<string> ColumnList
        {
            get
            {
                Collection<string> result = new Collection<string>();

                if (this.CsvTable != null)
                {
                    foreach (DataColumn column in this.CsvTable.Columns)
                    {
                        result.Add(column.ColumnName);
                    }
                }

                return result;
            }
        }

        public CsvHelper()
        {
            this.CsvTable = new DataTable();
        }

        public CsvHelper(string csvFile)
            : this()
        {
            this.CsvFile = csvFile;
        }

        /// <summary>
        /// Reads the csv file and fill the csv data into the datatable.
        /// </summary>
        ///

        public void ConvertToUtf8()
        {
            FileStream fs = File.OpenRead(this.CsvFile);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
            fs.Close();

            Encoding tempEncoding = GetFileEncoding(bytes);

            byte[] decoded = Encoding.Convert(tempEncoding, Encoding.UTF8, bytes, 0, bytes.Length);
            FileStream fw = File.OpenWrite(this.CsvFile);
            fw.Write(decoded, 0, (int)decoded.Length);
            fw.Close();
        }

        public Encoding GetFileEncodig(string path)
        {
            FileStream fs = File.OpenRead(path);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
            fs.Close();

            return GetFileEncoding(bytes);
        }

        public static Encoding GetFileEncoding(byte[] buffer)
        {
            Encoding enc = Encoding.Default;

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)

                enc = Encoding.UTF8;

            else if (buffer[0] == 0xfe && buffer[1] == 0xff)

                enc = Encoding.Unicode;

            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)

                enc = Encoding.UTF32;

            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)

                enc = Encoding.UTF7;

            return enc;
        }

        /// <summary>
        /// Read CSV file
        /// </summary>
        /// <param name="headStartAt">default is 0. If head start at 1 or 2, this method will skip previous unrelevant lines</param>
        public void Read_SkipGarbageRows(int headStartAt)
        {
            if (!File.Exists(this.CsvFile))
            {
                throw new FileNotFoundException("Can't find file:" + this.CsvFile);
            }

            StreamReader sr = null;

            //StreamReader sr = new StreamReader(this.CsvFile, System.Text.UTF8Encoding.UTF8);

            try
            {
                sr = new StreamReader(this.CsvFile, this.GetFileEncodig(this.CsvFile));
                string line = string.Empty;

                for (int i = 0; i < headStartAt; i++)
                    sr.ReadLine();

                line = sr.ReadLine();

                if (line != null)
                {
                    GenerateColumns(line);
                }
                else
                {
                    throw new Exception("File is empty: " + this.CsvFile);
                }

                line = sr.ReadLine();

                while (line != null)
                {
                    AddRow(line);
                    line = sr.ReadLine();
                }
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        public void Read()
        {
            if (!File.Exists(this.CsvFile))
            {
                throw new FileNotFoundException("Can't find file:" + this.CsvFile);
            }

            StreamReader sr = null;

            //StreamReader sr = new StreamReader(this.CsvFile, System.Text.UTF8Encoding.UTF8);

            try
            {
                sr = new StreamReader(this.CsvFile, this.GetFileEncodig(this.CsvFile));

                string line = sr.ReadLine();

                // todo: need to add function to skip create column
                if (line != null)
                {
                    GenerateColumns(line);
                }
                else
                {
                    throw new Exception("File is empty: " + this.CsvFile);
                }

                line = sr.ReadLine();

                while (line != null)
                {
                    AddRow(line);
                    line = sr.ReadLine();
                }
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        /// <summary>
        /// Reads the csv file and fill the csv data into the datatable.
        /// </summary>
        public void Read(int numRows)
        {
            if (!File.Exists(this.CsvFile))
            {
                throw new FileNotFoundException("Can't find file:" + this.CsvFile);
            }

            StreamReader sr = null;

            try
            {
                sr = new StreamReader(this.CsvFile, this.GetFileEncodig(this.CsvFile));

                string line = sr.ReadLine();

                if (line != null)
                {
                    GenerateColumns(line);
                }
                else
                {
                    throw new Exception("File is empty: " + this.CsvFile);
                }

                line = sr.ReadLine();

                while (line != null && numRows > 0)
                {
                    AddRow(line);
                    line = sr.ReadLine();
                    numRows--;
                }
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        /// <summary>
        /// Read CSV file with "column rule" for varification
        /// </summary>
        /// <param name="columnRule"></param>
        /// <returns></returns>
        public bool Read(List<string> columnRule)
        {
            bool result = true;

            if (!File.Exists(this.CsvFile))
            {
                throw new FileNotFoundException("Can't find file:" + this.CsvFile);
            }

            StreamReader sr = null;

            //StreamReader sr = new StreamReader(this.CsvFile, System.Text.UTF8Encoding.UTF8);

            try
            {
                sr = new StreamReader(this.CsvFile, this.GetFileEncodig(this.CsvFile));

                string line = sr.ReadLine();

                if (line != null)
                {
                    GenerateColumns(line);
                }
                else
                {
                    throw new Exception("File is empty: " + this.CsvFile);
                }

                line = sr.ReadLine();

                while (line != null)
                {
                    AddRow(line);
                    line = sr.ReadLine();
                }
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }

            // Column Varification
            int columnNo = 0;
            foreach (string rule in columnRule)
            {
                if (this.CsvTable.Columns[columnNo].ToString().ToLower() == rule.ToLower())
                { }
                else
                {
                    result = false;
                }
                columnNo++;
            }

            return result;
        }

        /// <summary>
        /// write the datatable content into a csv file at the specified location.
        /// </summary>
        /// <param name="csvLocation">csv file output location</param>
        public void Write(string csvLocation)
        {
            if (this.CsvTable != null)
            {
                Write(this.CsvTable.CreateDataReader(), csvLocation, this.ColumnList);
            }
        }

        public void Append(string csvLocation)
        {
            if (this.CsvTable != null)
            {
                Append(this.CsvTable.CreateDataReader(), csvLocation, this.ColumnList);
            }
        }

        /// <summary>
        /// write the datatable content into a csv file at the specified location.
        /// </summary>
        /// <param name="csvLocation">csv file output location</param>
        public void Write(string csvLocation, Encoding encoding)
        {
            if (this.CsvTable != null)
            {
                Write(this.CsvTable.CreateDataReader(), csvLocation, this.ColumnList, encoding);
            }
        }

        public void Write(string csvLocation, bool includeHeaders)
        {
            if (this.CsvTable != null)
            {
                Write(this.CsvTable.CreateDataReader(), csvLocation, this.ColumnList, includeHeaders);
            }
        }

        private void GenerateColumns(string columnLine)
        {
            string[] buffer = columnLine.Split(Separator);

            foreach (string columnData in buffer)
            {
                if (!CsvTable.Columns.Contains(columnData.Trim()))
                {
                    this.CsvTable.Columns.Add(columnData.Trim());
                }
                else
                {
                    this.CsvTable.Columns.Add(columnData.Trim() + DateTime.Now.Second);
                }
            }
        }

        private void AddRow(string rowLine)
        {
            string[] buffer = rowLine.Split(Separator);
            List<string> result = new List<string>();
            for (int i = 0; i < buffer.Length; i++)
            {
                // if fields contain '"', make fields as one field
                if (buffer[i].IndexOf('"') >= 0)
                {
                    // However, if '"' is shown twice in a field, skip the process
                    if (buffer[i].IndexOf('"', buffer[i].IndexOf('"') + 1) >= 0)
                    {
                        result.Add(buffer[i]);
                    }
                    else
                    {
                        string temp = buffer[i];
                        int x = 0;
                        for (x = i + 1; x < buffer.Length; x++)
                        {
                            temp += Separator + buffer[x];
                            if (buffer[x].IndexOf('"') >= 0)
                            {
                                //x = buffer.Length;
                                break;
                            }
                        }
                        temp = temp.Substring(temp.IndexOf('"') + 1, temp.LastIndexOf('"') - temp.IndexOf('"') - 1);
                        result.Add(temp);
                        i = x;
                    }
                }
                else
                {
                    result.Add(buffer[i]);
                }
            }

            /* if (buffer.Length != CsvTable.Columns.Count)
             {
                 throw new Exception("File is not well-formed:" + this.CsvFile);
             }*/

            DataRow row = CsvTable.NewRow();
            for (int i = 0; i < result.Count; i++)
            {
                try
                {
                    row[i] = result[i];
                }
                catch (Exception)
                {
                    ///the row migth be malformed just ignore and keep reading the rest of the file
                }
            }

            this.CsvTable.Rows.Add(row);
        }

        /// <summary>
        /// Output the data in the DataReader to the specified location according to the output columns structure.
        /// </summary>
        /// <param name="r">data reader</param>
        /// <param name="csvLocation">outpur csv location</param>
        /// <param name="columns">output columns</param>
        public static void Write(IDataReader r, string csvLocation, Collection<string> columns)
        {
            IOHelper.WriteTextFile(GetCsvRawData(r, columns), csvLocation);
        }

        public static void Write(IDataReader r, string csvLocation, Collection<string> columns, Encoding encoding)
        {
            IOHelper.WriteTextFile(GetCsvRawData(r, columns), csvLocation, encoding);
        }

        public static void Write(IDataReader r, string csvLocation, Collection<string> columns, bool includeHeaders)
        {
            IOHelper.WriteTextFile(GetCsvRawData(r, columns, includeHeaders), csvLocation);
        }

        public string GetCsvData()
        {
            return GetCsvData(true);
        }

        public string GetCsvData(bool includeHeaders)
        {
            return CsvHelper.GetCsvRawData(this.CsvTable.CreateDataReader(), this.ColumnList, includeHeaders);
        }

        private static string GetCsvRawData(IDataReader r, Collection<string> columns, bool includeHeaders)
        {
            StringBuilder sb = new StringBuilder();

            if (includeHeaders)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    // sb.Append(columns[i].Replace(Separator, Separator_Replacement));
                    // put qualifier if value has ','
                    sb.Append((columns[i].IndexOf(",") > -1) ? (Qualifier + columns[i] + Qualifier) : columns[i]);

                    if (i < columns.Count - 1)
                    {
                        sb.Append(Separator);
                    }
                    else
                    {
                        sb.Append(SystemConstants.LINE_BREAK);
                    }
                }
            }

            while (r.Read())
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    // sb.Append(columns[i].Replace(Separator, Separator_Replacement));
                    // put qualifier if value has ','
                    sb.Append((r[columns[i]].ToString().IndexOf(",") > -1) ? (Qualifier + r[columns[i]].ToString() + Qualifier) : r[columns[i]].ToString());

                    if (i < columns.Count - 1)
                    {
                        sb.Append(Separator);
                    }
                    else
                    {
                        sb.Append(SystemConstants.LINE_BREAK);
                    }
                }
            }

            return sb.ToString();
        }

        private static string GetCsvRawData(IDataReader r, Collection<string> columns)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < columns.Count; i++)
            {
                // sb.Append(columns[i].Replace(Separator, Separator_Replacement));
                // put qualifier if value has ','
                sb.Append((columns[i].IndexOf(",") > -1) ? (Qualifier + columns[i] + Qualifier) : columns[i]);

                if (i < columns.Count - 1)
                {
                    sb.Append(Separator);
                }
                else
                {
                    sb.Append(SystemConstants.LINE_BREAK);
                }
            }

            while (r.Read())
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    // sb.Append(columns[i].Replace(Separator, Separator_Replacement));
                    // put qualifier if value has ','
                    sb.Append((r[columns[i]].ToString().IndexOf(",") > -1) ? (Qualifier + r[columns[i]].ToString() + Qualifier) : r[columns[i]].ToString());

                    if (i < columns.Count - 1)
                    {
                        sb.Append(Separator);
                    }
                    else
                    {
                        sb.Append(SystemConstants.LINE_BREAK);
                    }
                }
            }

            return sb.ToString();
        }

        public byte[] ToBinary()
        {
            byte[] result = null;

            if (this.CsvTable != null)
            {
                string rawData = GetCsvRawData(this.CsvTable.CreateDataReader(), this.ColumnList);

                ASCIIEncoding encoder = new ASCIIEncoding();
                result = encoder.GetBytes(rawData);
            }

            return result;
        }

        public override string ToString()
        {
            string result = null;

            if (this.CsvTable != null)
            {
                result = GetCsvRawData(this.CsvTable.CreateDataReader(), this.ColumnList);
            }

            return result;
        }

        public static void Append(IDataReader dataReader, string csvLocation, Collection<string> columns)
        {
            //Gets the existing data in the csv file
            string data = IOHelper.ReadTextFile(csvLocation);
            //Add the data to string builder
            StringBuilder sb = new StringBuilder(data);
            //Gets the data from the actual datareader
            string dataToAppend = GetCsvRawData(dataReader, columns, false);
            //Appends the new data to string builder
            sb.Append(dataToAppend);
            //Writes the file
            IOHelper.WriteTextFile(sb.ToString(), csvLocation);
        }
    }
}