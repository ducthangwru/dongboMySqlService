using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DongBoListVip
{
    public class SqlDataHelper
    {
        /// <summary>
        /// ConnectionString connection to SQL Server 2008
        /// </summary>
        /// <returns>ConnectionString</returns>  
        /// 
        string strConnection = "";
        private static Log_Sytems logs = new Log_Sytems();
        log4net.ILog log = log4net.LogManager.GetLogger(typeof(SqlDataHelper));
        public SqlDataHelper(string strConnect)
        {
            this.strConnection = GetConnectionString(strConnect);

        }

        public SqlDataHelper()
        {
            this.strConnection = GetConnectionString();
        }

        public string GetConnectionString(string strConnect)
        {
            //return WebConfigurationManager.AppSettings["Collectric"];
            //return WebConfigurationManager.AppSettings["Collectric"];
            // string sv = ConfigurationManager.AppSettings["Server"];
            // string db = ConfigurationManager.AppSettings["Database"];
            //  string uid = ConfigurationManager.AppSettings["Username"];
            //  string pwd = ConfigurationManager.AppSettings["Password"];

            // return "Server=" + sv + ";Database=" + db + ";User ID=" + uid + ";Password=" + pwd + ";Max Pool Size = 150;";
            // string ConnectionString = WebConfigurationManager.AppSettings["ConnectionString"];
            string ConnectionString = LHLog.Decrypt(strConnect);
            return ConnectionString;

        }

        public string GetConnectionString()
        {
            string appsetting = ConfigurationManager.AppSettings["SERVER_KEY"];
            string ConnectionString = LHLog.Decrypt(appsetting);
            return ConnectionString;

        }

        /// <summary>
        /// Execute a store procedure return a instance of SqlDataReader
        /// </summary>
        /// <param name="procName">Store Procedure Name</param>
        /// <param name="procParams">Param List</param>
        /// <returns>A instance of SqlDataReader contain result of stored procedure</returns>
        public SqlDataReader ExecuteReader(string procName, params SqlParameter[] procParams)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            string paramName = "";
            try
            {
                conn = new SqlConnection(strConnection);
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;
                if (procParams != null)
                {
                    for (int i = 0; i < procParams.Length; i++)
                    {
                        cmd.Parameters.Add(procParams[i]);
                        paramName += procParams[i].ParameterName + ":" + procParams[i].Value + "|";
                    }
                }
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                log.Error("Command:" + cmd.CommandText + ",Command Parameter:" + paramName);
                //log.Error(ex);

                throw ex;
            }
            finally
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return reader;
        }
        public SqlDataReader ExecuteReader(string strQuery)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                conn = new SqlConnection(strConnection);
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = strQuery;

                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                log.Error("Command:" + cmd.CommandText);
                log.Error(ex);

                throw ex;
            }
            finally
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return reader;
        }
        /// <summary>
        /// Execute a store procedure return all records  to influence
        /// </summary>
        /// <param name="procName">Stored procedure name</param>
        /// <param name="procParams">Param List</param>
        /// <returns>return all records  to be affected after executing the stored procedure</returns>
        public int ExecuteNonQuery(string procName, params SqlParameter[] procParams)
        {
            SqlCommand cmd = null;
            SqlConnection conn = null;
            int affectedRows = 0;
            string paramName = "";
            try
            {
                conn = new SqlConnection(strConnection);
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;
                if (procParams != null)
                {
                    for (int i = 0; i < procParams.Length; i++)
                    {
                        cmd.Parameters.Add(procParams[i]);
                        paramName += procParams[i].ParameterName + ":" + procParams[i].Value + "|";
                    }
                }
                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error("Command:" + cmd.CommandText + ",Command Parameter:" + paramName);
                log.Error(ex);

                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return affectedRows;
        }
        public int ExecuteNonQuery(string procName, bool StoreProcedure, params SqlParameter[] procParams)
        {
            SqlCommand cmd = null;
            SqlConnection conn = null;
            int affectedRows = 0;
            string paramName = "";
            try
            {
                conn = new SqlConnection(strConnection);
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = procName;
                if (StoreProcedure)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                if (procParams != null)
                {
                    for (int i = 0; i < procParams.Length; i++)
                    {
                        cmd.Parameters.Add(procParams[i]);
                        paramName += procParams[i].ParameterName + ":" + procParams[i].Value + "|";
                    }
                }
                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error("Command:" + cmd.CommandText + ",Command Parameter:" + paramName);
                log.Error(ex);

                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return affectedRows;
        }
        public int ExecuteNonQuery(string strQuery)
        {
            SqlCommand cmd = null;
            SqlConnection conn = null;
            int affectedRows = 0;
            try
            {
                conn = new SqlConnection(strConnection);
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = strQuery;

                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error("Command:" + cmd.CommandText);
                log.Error(ex);


                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return affectedRows;
        }

        /// <summary>
        /// Execute a store procedure return first row and first column of all records
        /// </summary>
        /// <param name="procName">Stored procedure name</param>
        /// <param name="procParams">Param List</param>
        /// <returns>return first row and first column of a stored procedure is boxing</returns>
        public object ExecuteScalar(string procName, params SqlParameter[] procParams)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            object value = null;
            string paramName = "";
            try
            {
                conn = new SqlConnection(strConnection);
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = procName;
                cmd.CommandType = CommandType.StoredProcedure;
                if (procParams != null)
                {
                    for (int i = 0; i < procParams.Length; i++)
                    {
                        cmd.Parameters.Add(procParams[i]);
                        paramName += procParams[i].ParameterName + ":" + procParams[i].Value + "|";
                    }
                }
                value = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                log.Error("Command:" + cmd.CommandText + ",Command Parameter:" + paramName);
                log.Error(ex);

                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return value;
        }
        public object ExecuteScalar(string procName, bool StoreProcedure, params SqlParameter[] procParams)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            object value = null;
            string paramName = "";
            try
            {
                conn = new SqlConnection(strConnection);
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = procName;
                if (StoreProcedure)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                if (procParams != null)
                {
                    for (int i = 0; i < procParams.Length; i++)
                    {
                        cmd.Parameters.Add(procParams[i]);
                        paramName += procParams[i].ParameterName + ":" + procParams[i].Value + "|";
                    }
                }
                value = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                log.Error("Command:" + cmd.CommandText + ",Command Parameter:" + paramName);
                log.Error(ex);

                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return value;
        }
        public object ExecuteScalar(string strQuery)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            object value = null;

            try
            {
                conn = new SqlConnection(strConnection);
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = strQuery;

                value = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                log.Error("Command:" + cmd.CommandText);
                log.Error(ex);

                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return value;
        }
        /// <summary>
        /// Execute procedure with DataSet type
        /// </summary>
        /// <param name="procName">Stored procedure name</param>
        /// <param name="procParams">Param List</param>
        /// <returns>A instance of DataSet</returns>
        public DataSet ExecuteDataSet(string procName, params SqlParameter[] procParams)
        {
            SqlConnection conn = null;
            SqlDataAdapter adapter = null;
            DataSet ds = new DataSet();
            SqlCommand cmd = null;
            string paramName = "";
            try
            {
                conn = new SqlConnection(strConnection);
                cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (procParams != null)
                {
                    for (int i = 0; i < procParams.Length; i++)
                    {
                        cmd.Parameters.Add(procParams[i]);
                        paramName += procParams[i].ParameterName + ":" + procParams[i].Value + "|";
                    }
                }
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                log.Error("Command:" + cmd.CommandText + ",Command Parameter:" + paramName);
                log.Error(ex);

                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (adapter != null)
                {
                    adapter.Dispose();


                }
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return ds;
        }
        public DataSet ExecuteDataSet(string procName, bool StoreProcedure, params SqlParameter[] procParams)
        {
            SqlConnection conn = null;
            SqlDataAdapter adapter = null;
            DataSet ds = new DataSet();
            SqlCommand cmd = null;
            string paramName = "";
            try
            {
                conn = new SqlConnection(strConnection);
                cmd = new SqlCommand(procName, conn);
                if (StoreProcedure)
                    cmd.CommandType = CommandType.StoredProcedure;
                if (procParams != null)
                {
                    for (int i = 0; i < procParams.Length; i++)
                    {
                        cmd.Parameters.Add(procParams[i]);
                        paramName += procParams[i].ParameterName + ":" + procParams[i].Value + "|";
                    }
                }
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                log.Error("Command:" + cmd.CommandText + ",Command Parameter:" + paramName);
                log.Error(ex);

                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (adapter != null)
                {
                    adapter.Dispose();


                }
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return ds;
        }
        public DataSet ExecuteDataSet(string strQuery)
        {
            SqlConnection conn = null;
            SqlDataAdapter adapter = null;
            DataSet ds = new DataSet();
            SqlCommand cmd = null;
            try
            {
                conn = new SqlConnection(strConnection);
                cmd = new SqlCommand(strQuery, conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                log.Error("Command:" + cmd.CommandText);
                log.Error(ex);

                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (adapter != null)
                {
                    adapter.Dispose();


                }
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return ds;
        }
        /// <summary>
        /// Create a new param with SqlParameter type 
        /// </summary>
        /// <param name="paramName">Param Name</param>
        /// <param name="value">Param Value</param>
        /// <returns>A instance of SqlParameter</returns>
        public SqlParameter CreateParameter(string paramName, object value)
        {
            SqlParameter param = new SqlParameter(paramName, value);
            return param;
        }
    }
}
