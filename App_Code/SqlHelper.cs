using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// SqlHelper 数据库操作辅助类
/// </summary>
public class SqlHelper
{
    // 数据库连接字符串
    private static readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

    #region 执行SQL语句或存储过程，返回影响的行数

    /// <summary>
    /// 执行SQL语句或存储过程，返回影响的行数
    /// </summary>
    /// <param name="cmdText">SQL语句或存储过程名</param>
    /// <param name="parameters">参数数组</param>
    /// <returns>返回影响的行数</returns>
    public static int ExecuteNonQuery(string cmdText, params SqlParameter[] parameters)
    {
        return ExecuteNonQuery(cmdText, CommandType.Text, parameters);
    }

    /// <summary>
    /// 执行SQL语句或存储过程，返回影响的行数
    /// </summary>
    /// <param name="cmdText">SQL语句或存储过程名</param>
    /// <param name="cmdType">命令类型</param>
    /// <param name="parameters">参数数组</param>
    /// <returns>返回影响的行数</returns>
    public static int ExecuteNonQuery(string cmdText, CommandType cmdType, params SqlParameter[] parameters)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                command.CommandType = cmdType;
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }
    }

    #endregion

    #region 执行SQL语句或存储过程，返回第一行第一列的值

    /// <summary>
    /// 执行SQL语句或存储过程，返回第一行第一列的值
    /// </summary>
    /// <param name="cmdText">SQL语句或存储过程名</param>
    /// <param name="parameters">参数数组</param>
    /// <returns>返回第一行第一列的值</returns>
    public static object ExecuteScalar(string cmdText, params SqlParameter[] parameters)
    {
        return ExecuteScalar(cmdText, CommandType.Text, parameters);
    }

    /// <summary>
    /// 执行SQL语句或存储过程，返回第一行第一列的值
    /// </summary>
    /// <param name="cmdText">SQL语句或存储过程名</param>
    /// <param name="cmdType">命令类型</param>
    /// <param name="parameters">参数数组</param>
    /// <returns>返回第一行第一列的值</returns>
    public static object ExecuteScalar(string cmdText, CommandType cmdType, params SqlParameter[] parameters)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                command.CommandType = cmdType;
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                return command.ExecuteScalar();
            }
        }
    }

    #endregion

    #region 执行SQL语句或存储过程，返回SqlDataReader对象

    /// <summary>
    /// 执行SQL语句或存储过程，返回SqlDataReader对象
    /// </summary>
    /// <param name="cmdText">SQL语句或存储过程名</param>
    /// <param name="parameters">参数数组</param>
    /// <returns>返回SqlDataReader对象</returns>
    public static SqlDataReader ExecuteReader(string cmdText, params SqlParameter[] parameters)
    {
        return ExecuteReader(cmdText, CommandType.Text, parameters);
    }

    /// <summary>
    /// 执行SQL语句或存储过程，返回SqlDataReader对象
    /// </summary>
    /// <param name="cmdText">SQL语句或存储过程名</param>
    /// <param name="cmdType">命令类型</param>
    /// <param name="parameters">参数数组</param>
    /// <returns>返回SqlDataReader对象</returns>
    public static SqlDataReader ExecuteReader(string cmdText, CommandType cmdType, params SqlParameter[] parameters)
    {
        SqlConnection connection = new SqlConnection(connectionString);
        try
        {
            SqlCommand command = new SqlCommand(cmdText, connection);
            command.CommandType = cmdType;
            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        catch
        {
            connection.Close();
            throw;
        }
    }

    #endregion

    #region 执行SQL语句或存储过程，返回DataTable对象

    /// <summary>
    /// 执行SQL语句或存储过程，返回DataTable对象
    /// </summary>
    /// <param name="cmdText">SQL语句或存储过程名</param>
    /// <param name="parameters">参数数组</param>
    /// <returns>返回DataTable对象</returns>
    public static DataTable ExecuteDataTable(string cmdText, params SqlParameter[] parameters)
    {
        return ExecuteDataTable(cmdText, CommandType.Text, parameters);
    }

    /// <summary>
    /// 执行SQL语句或存储过程，返回DataTable对象
    /// </summary>
    /// <param name="cmdText">SQL语句或存储过程名</param>
    /// <param name="cmdType">命令类型</param>
    /// <param name="parameters">参数数组</param>
    /// <returns>返回DataTable对象</returns>
    public static DataTable ExecuteDataTable(string cmdText, CommandType cmdType, params SqlParameter[] parameters)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                command.CommandType = cmdType;
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }

    #endregion

    #region 执行SQL语句或存储过程，返回DataSet对象

    /// <summary>
    /// 执行SQL语句或存储过程，返回DataSet对象
    /// </summary>
    /// <param name="cmdText">SQL语句或存储过程名</param>
    /// <param name="parameters">参数数组</param>
    /// <returns>返回DataSet对象</returns>
    public static DataSet ExecuteDataSet(string cmdText, params SqlParameter[] parameters)
    {
        return ExecuteDataSet(cmdText, CommandType.Text, parameters);
    }

    /// <summary>
    /// 执行SQL语句或存储过程，返回DataSet对象
    /// </summary>
    /// <param name="cmdText">SQL语句或存储过程名</param>
    /// <param name="cmdType">命令类型</param>
    /// <param name="parameters">参数数组</param>
    /// <returns>返回DataSet对象</returns>
    public static DataSet ExecuteDataSet(string cmdText, CommandType cmdType, params SqlParameter[] parameters)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(cmdText, connection))
            {
                command.CommandType = cmdType;
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
        }
    }

    #endregion

    #region 批量执行SQL语句

    /// <summary>
    /// 批量执行SQL语句
    /// </summary>
    /// <param name="sqlList">SQL语句列表</param>
    /// <returns>返回是否执行成功</returns>
    public static bool ExecuteNonQueryTrans(List<string> sqlList)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            try
            {
                foreach (string sql in sqlList)
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    /// <summary>
    /// 批量执行SQL语句，带参数
    /// </summary>
    /// <param name="sqlList">SQL语句列表</param>
    /// <param name="parametersList">参数列表</param>
    /// <returns>返回是否执行成功</returns>
    public static bool ExecuteNonQueryTrans(List<string> sqlList, List<SqlParameter[]> parametersList)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlTransaction transaction = connection.BeginTransaction();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.Transaction = transaction;

            try
            {
                for (int i = 0; i < sqlList.Count; i++)
                {
                    command.CommandText = sqlList[i];
                    command.Parameters.Clear();
                    if (parametersList != null && parametersList.Count > i && parametersList[i] != null)
                    {
                        command.Parameters.AddRange(parametersList[i]);
                    }
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    #endregion

    #region 分页查询

    /// <summary>
    /// 执行分页查询，返回DataTable对象
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="fields">要查询的字段列表</param>
    /// <param name="where">查询条件</param>
    /// <param name="orderBy">排序字段</param>
    /// <param name="pageSize">每页记录数</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="recordCount">返回记录总数</param>
    /// <returns>返回查询结果</returns>
    public static DataTable ExecutePager(string tableName, string fields, string where, string orderBy, int pageSize, int pageIndex, out int recordCount)
    {
        if (string.IsNullOrEmpty(fields)) fields = "*";
        if (string.IsNullOrEmpty(where)) where = "1=1";
        if (string.IsNullOrEmpty(orderBy)) orderBy = "(SELECT NULL)";

        // 计算记录总数
        string countSql = $"SELECT COUNT(*) FROM {tableName} WHERE {where}";
        recordCount = Convert.ToInt32(ExecuteScalar(countSql));

        // 分页查询
        string sql = $@"WITH PagedData AS
                        (
                            SELECT {fields}, ROW_NUMBER() OVER (ORDER BY {orderBy}) AS RowNum
                            FROM {tableName}
                            WHERE {where}
                        )
                        SELECT * FROM PagedData
                        WHERE RowNum BETWEEN {(pageIndex - 1) * pageSize + 1} AND {pageIndex * pageSize}";

        return ExecuteDataTable(sql);
    }

    #endregion
}
