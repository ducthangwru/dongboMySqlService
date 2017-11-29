using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DongBoListVip
{
    public class MySqlDataHelper
    {
        ILog logs = LogManager.GetLogger(typeof(MySqlDataHelper));
        /// <summary>
        /// ConnectionString connection to SQL Server 2005
        /// </summary>
        /// <returns>ConnectionString</returns>  
        /// 
        public string GetConnectionString()
        {
            string appsetting = ConfigurationManager.AppSettings["SERVER_KEY_TONGDAI"];
            return LHLog.Decrypt(appsetting);
        }
        /// <summary>
        /// Execute a store procedure return a instance of SqlDataReader
        /// </summary>
        /// <param name="procName">Store Procedure Name</param>
        /// <param name="procParams">Param List</param>
        /// <returns>A instance of SqlDataReader contain result of stored procedure</returns>
        public MySqlDataReader ExecuteReader(string procName, bool IsProc, params MySqlParameter[] procParams)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            string paramName = "";
            try
            {
                conn = new MySqlConnection(GetConnectionString());
                conn.Open();
                cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = procName;
                if (IsProc)
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
                logs.Error("Command:" + procName + ",Command Parameter:" + paramName);
                logs.Error(ex); throw (ex);
            }
            return reader;
        }

        public MySqlDataReader ExecuteReader(string strQuery)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            try
            {
                conn = new MySqlConnection(GetConnectionString());
                conn.Open();
                cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = strQuery;
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                logs.Error("Command:" + strQuery);
                logs.Error(ex); throw (ex);

            }
            return reader;
        }
        /// <summary>
        /// Execute a store procedure return all records  to influence
        /// </summary>
        /// <param name="procName">Stored procedure name</param>
        /// <param name="procParams">Param List</param>
        /// <returns>return all records  to be affected after executing the stored procedure</returns>
        public int ExecuteNonQuery(string procName, bool IsProc, params MySqlParameter[] procParams)
        {
            MySqlCommand cmd = null;
            MySqlConnection conn = null;
            int affectedRows = 0;
            string paramName = "";

            try
            {
                conn = new MySqlConnection(GetConnectionString());
                conn.Open();
                cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = procName;
                if (IsProc)
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
                if (!ex.Message.Contains("Duplicate entry") && !ex.Message.Contains("Deadlock found when trying to get lock; try restarting transaction"))
                {
                    logs.Error("Command:" + procName + ",Command Parameter:" + paramName);
                    logs.Error(ex); throw (ex);
                }

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                conn.Close();
            }
            return affectedRows;
        }
        public int ExecuteNonQuery(string strQuery)
        {
            MySqlCommand cmd = null;
            MySqlConnection conn = null;
            int affectedRows = 0;
            try
            {
                conn = new MySqlConnection(GetConnectionString());
                conn.Open();
                cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = strQuery;
                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Duplicate entry") && !ex.Message.Contains("Deadlock found when trying to get lock; try restarting transaction"))
                {
                    logs.Error("Command:" + strQuery);
                    logs.Error(ex); throw (ex);
                }

            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                conn.Close();
            }
            return affectedRows;
        }

        /// <summary>
        /// Execute a store procedure return first row and first column of all records
        /// </summary>
        /// <param name="procName">Stored procedure name</param>
        /// <param name="procParams">Param List</param>
        /// <returns>return first row and first column of a stored procedure is boxing</returns>
        public object ExecuteScalar(string procName, bool IsProc, params MySqlParameter[] procParams)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            object value = null;
            string paramName = "";

            try
            {
                conn = new MySqlConnection(GetConnectionString());
                conn.Open();
                cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = procName;
                if (IsProc)
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
                logs.Error("Command:" + procName + ",Command Parameter:" + paramName);
                logs.Error(ex); throw (ex);

            }
            finally
            {
                if (conn != null)
                {
                    conn.Dispose();
                }
            }
            return value;
        }
        public object ExecuteScalar(string strQuery)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            object value = null;
            try
            {
                conn = new MySqlConnection(GetConnectionString());
                conn.Open();
                cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = strQuery;
                value = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                logs.Error("Command:" + strQuery);
                logs.Error(ex); throw (ex);

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
        public DataSet ExecuteDataSet(string procName, bool IsProc, params MySqlParameter[] procParams)
        {
            MySqlConnection conn = null;
            MySqlDataAdapter adapter = null;
            DataSet ds = new DataSet();
            MySqlCommand cmd = null;
            string paramName = "";
            try
            {
                conn = new MySqlConnection(GetConnectionString());
                cmd = new MySqlCommand(procName, conn);
                if (IsProc)
                    cmd.CommandType = CommandType.StoredProcedure;
                if (procParams != null)
                {
                    for (int i = 0; i < procParams.Length; i++)
                    {
                        cmd.Parameters.Add(procParams[i]);
                        paramName += procParams[i].ParameterName + ":" + procParams[i].Value + "|";
                    }
                }
                adapter = new MySqlDataAdapter(cmd);
                adapter.SelectCommand.CommandTimeout = 600;
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                logs.Error("Command:" + procName + ",Command Parameter:" + paramName);
                logs.Error(ex); throw (ex);

            }
            finally
            {
                if (adapter != null)
                {
                    adapter.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }

            }
            return ds;
        }
        public DataSet ExecuteDataSet(string strQuery)
        {
            MySqlConnection conn = null;
            MySqlDataAdapter adapter = null;
            DataSet ds = new DataSet();
            MySqlCommand cmd = null;
            try
            {
                string connstr = GetConnectionString();
                conn = new MySqlConnection(connstr);
                cmd = new MySqlCommand(strQuery, conn);
                cmd.CommandTimeout = 360;
                adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                logs.Error("Command:" + strQuery);
                logs.Error(ex); throw (ex);

            }
            finally
            {
                if (adapter != null)
                {
                    adapter.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }

            }
            return ds;
        }
        /// <summary>
        /// Create a new param with MySqlParameter type 
        /// </summary>
        /// <param name="paramName">Param Name</param>
        /// <param name="value">Param Value</param>
        /// <returns>A instance of MySqlParameter</returns>
        public MySqlParameter CreateParameter(string paramName, object value)
        {
            MySqlParameter param = new MySqlParameter(paramName, value);
            return param;
        }
    }
}
