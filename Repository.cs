using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
 
namespace SchedularJob.Infra
{
    [ExcludeFromCodeCoverage]
    public class Repository
    {
        private readonly string connectionString = string.Empty;
 
        public Repository(string connectionString_)
        {
            connectionString = connectionString_;
        }
 
        private SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return connection;
        }
 
 
        private DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType)
        {
            SqlCommand command = new SqlCommand(commandText, connection as SqlConnection);
            command.CommandType = commandType;
            return command;
        }
        public async Task<int> ExecuteText(string sql)
        {
            int returnValue = 0;
            using (DbConnection connection = GetConnection())
            {
                try
                {
 
                    DbCommand cmd = GetCommand(connection, sql, CommandType.Text);
                    cmd.CommandTimeout = 120;
                    returnValue = await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }
            return returnValue;
        }
 
        public async Task<int> ExecuteProcedure(string sql, List<SqlParameter> sqlParameters = null)
        {
            int returnValue = 0;
            using (DbConnection connection = GetConnection())
            {
                try
                {
                    SqlParameter sp = new SqlParameter();
                
                    if (sqlParameters != null)
                    {
                        sp.ParameterName = sqlParameters.First().ParameterName;
                        sp.SqlDbType = sqlParameters.First().SqlDbType;
                        sp.Value = sqlParameters.First().SqlValue ?? sqlParameters.First().Value;
                    }
                    DbCommand cmd = GetCommand(connection, sql, CommandType.StoredProcedure);
                    cmd.CommandTimeout = 2000;
                    cmd.Parameters.Add(sp);
                    returnValue =  await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }
            return returnValue;
        }
 
 
 
        public async Task<string> ExecuteStoreProcedure(string procedureName, List<SqlParameter> parameters = null)//, CommandType commandType = CommandType.StoredProcedure
        {
            string returnValue = null;
            using (DbConnection connection = GetConnection())
            {
                try
                {
 
                    DbCommand cmd = GetCommand(connection, procedureName, CommandType.StoredProcedure);
 
                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
 
                    returnValue = Convert.ToString(await cmd.ExecuteScalarAsync());
 
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }
            return returnValue;
        }
 
        public async Task<string> ExecuteScalar(string sql)
        {
 
 
            using DbConnection connection = GetConnection();
            {
                try
                {
                    DbCommand cmd = GetCommand(connection, sql, CommandType.Text);
                   
                    var querResult = await cmd.ExecuteScalarAsync();
                    if (querResult != null)
                    {
                        return querResult.ToString();
                    }
                    else
                        return string.Empty;
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }
            
        }
 
    }
}
