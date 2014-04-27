using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Utility
{
    public class DBConnection : IDisposable
    {
        internal delegate void Callback(SqlDataReader r);

        #region Constants

        private const string DEFAULT_CONNECTION_STRING = "User ID=***;Password=***;data source=192.168.10.20; initial catalog=dbname";

        #endregion

        #region Fields

        private string connectionString = null;
        private SqlConnection connection;

        #endregion

        #region Properties

        public string ConnectionString
        {
            set
            {
                this.connectionString = value;
            }
            get
            {
                return connectionString;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Create a database connection with the default connection string
        /// </summary>
        public DBConnection()
        {

        }

        /// <summary>
        /// Create a database connection using the given connection string
        /// </summary>
        /// <param name="connection_string">The connection string to attempt to connect to the database with.</param>
        public DBConnection(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #endregion

        #region Methods

        #region Disposing Methods

        /// <summary>
        /// Implement Disposable and call Finalize using garbage collector
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free other state (managed objects).
            }
            // Free your own state (unmanaged objects).
            CloseConnection();
        }

        ~DBConnection()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }
        #endregion

        /// <summary>
        /// Attempts to open the connection if it is not already open. Throws an exception if it cannot.
        /// </summary>
        public void OpenConnection()
        {
            if (connection == null)
            {
                connection = new SqlConnection(connectionString);
            }

            if (connection.State == ConnectionState.Broken)
            {
                connection.Close();

                Exception ex = new Exception("Database Connection is broken.");
                throw ex;
            }
            else if (connection.State != ConnectionState.Open)
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Closes the connection if it is in any state except closed.
        /// </summary>
        public void CloseConnection()
        {
            if (connection != null)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }

                connection = null;
            }
        }

        /// <summary>
        /// Returns a SqlDataReader object which contains the rows returned by the given query.
        /// </summary>
        /// <param name="query">The SQL query to be run</param>
        /// <returns>A SqlDataReader object</returns>
        public SqlDataReader GetDataReader(string query)
        {
            OpenConnection();

            SqlCommand sql_command = new SqlCommand(query, connection);

            return sql_command.ExecuteReader(); ;
        }

        /// <summary>
        /// Runs the given query in the database. Usually for INSERT and UPDATE queries. 
        /// </summary>
        /// <param name="query">The SQL query to be run</param>
        /// <returns>True if the query was run successfully, False otherwise.</returns>
        public void RunQuery(string query)
        {
            OpenConnection();

            SqlCommand sql_command = new SqlCommand(query, connection);

            sql_command.ExecuteNonQuery();
        }

        /// <summary>
        /// Runs a stored procedure from the database.
        /// </summary>
        /// <param name="proc_name">The name of the stored procedure</param>
        /// <param name="parameters">The list of parameter names</param>
        /// <param name="values">The list of parameter values</param>
        /// <returns>True if successful, False otherwise</returns>
        public int RunStoredProcedure(string procName, Dictionary<string, string> parameters)
        {
            return RunStoredProcedure(procName, parameters, true);
        }

        /// <summary>
        /// Runs a stored procedure from the database.
        /// </summary>
        /// <param name="proc_name">The name of the stored procedure</param>
        /// <param name="parameters">The list of parameter names</param>
        /// <param name="values">The list of parameter values</param>
        /// <returns>True if successful, False otherwise</returns>
        public int RunStoredProcedure(string procName, Dictionary<string, string> parameters, bool useIdentity)
        {
            int identity = SystemConstants.NULL;

            OpenConnection();

            // Create a SQL command object, and set it up to run a stored procedure.
            SqlCommand sqlCommand = new SqlCommand(procName, connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            // Now add the procedure parameters and values
            for (int i = 0; i < parameters.Count; i++)
            {
                SqlParameter param = new SqlParameter(parameters.ElementAt(i).Key, parameters.ElementAt(i).Value);
                sqlCommand.Parameters.Add(param);
            }

            if (useIdentity)
            {
                // Add the output parameter which must exist in the stored procedure for this function to succeed.
                sqlCommand.Parameters.Add("@ident", SqlDbType.Int);
                sqlCommand.Parameters["@ident"].Direction = ParameterDirection.Output;

                sqlCommand.ExecuteNonQuery();

                // Gets the new id for the item entered into the database (or updated in the case of update queries).
                // This relies on the output variable existing in the stored procedure, otherwise an exception is thrown.
                identity = (int)sqlCommand.Parameters["@ident"].Value;

                // Sql server returns a negative integer to indicate an error result.
                if (identity < 0)
                {
                    throw new Exception(String.Format("Running stored procedure:{0} didn't return the expected identity.", procName));
                }
            }
            else
            {
                sqlCommand.ExecuteNonQuery();
            }

            return identity;
        }

        /// <summary>
        /// Runs the given stored procedure in the database.
        /// </summary>
        /// <param name="proc_name">The name of the stored procedure</param>
        public void RunStoredProcedure(string procName)
        {
            OpenConnection();

            SqlCommand sqlCommand = new SqlCommand(procName, connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            sqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Runs the stored procedure, taking the parameters and values given.
        /// </summary>
        /// <param name="proc_name">The name of the procedure</param>
        /// <param name="parameters">The list of parameters</param>
        /// <param name="values">The corresponding list of values</param>
        /// <returns>The rows returned by the query, null if an error occurs</returns>
        public SqlDataReader GetDataReaderFromStoredProcedure(string procName, Dictionary<string, string> parameters)
        {
            SqlDataReader reader = null;

            OpenConnection();

            SqlCommand sqlCommand = new SqlCommand(procName, connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            // Now add the procedure parameters and values
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters.ElementAt(i).Value != null)
                {
                    SqlParameter param = new SqlParameter(parameters.ElementAt(i).Key, parameters.ElementAt(i).Value);
                    sqlCommand.Parameters.Add(param);
                }
                else
                {
                    SqlParameter param = new SqlParameter(parameters.ElementAt(i).Key, System.DBNull.Value);
                    sqlCommand.Parameters.Add(param);
                }
            }

            reader = sqlCommand.ExecuteReader();

            return reader;
        }

        /// <summary>
        /// Runs the given stored procedure, returning results usually for a SELECT statement
        /// </summary>
        /// <param name="proc_name">The name of the stored procedure</param>
        /// <returns>The rows returned by the query, null if an error occurs</returns>
        public SqlDataReader GetDataReaderFromStoredProcedure(string procName)
        {
            return GetDataReaderFromStoredProcedure(procName, new Dictionary<string, string>());
        }

        /// <summary>
        /// Runs the stored procedure, which returns an integer count, taking the parameters and values given.
        /// </summary>
        /// <param name="proc_name">The name of the procedure</param>
        /// <param name="parameters">The list of parameters</param>
        /// <returns>count</returns>
        public int GetCountFromStoredProcedure(string procName, Dictionary<string, string> parameters)
        {
            int result = 0;

            SqlDataReader reader = GetDataReaderFromStoredProcedure(procName, parameters);

            if (reader.Read())
            {
                result = (int)reader[0];
            }

            return result;
        }

        /// <summary>
        /// Excute stored procedure and excute the call back method with the returned data. 
        /// </summary>
        /// <param name="proc_name">stored procedure name</param>
        /// <param name="parameter_list">stored procedure parameter list</param>
        /// <param name="callback">callback method</param>
        internal static void CallbackFromStoredProcedure(string procName, Dictionary<string, string> parameterList, Callback callback)
        {
            using (DBConnection cn = new DBConnection())
            {
                SqlDataReader r = null;

                try
                {
                    r = cn.GetDataReaderFromStoredProcedure(procName, parameterList);

                    if (callback != null)
                    {
                        callback(r);
                    }
                }
                finally
                {
                    if (r != null)
                    {
                        r.Close();
                    }

                    cn.CloseConnection();
                }
            }
        }

        #endregion
    }
}
