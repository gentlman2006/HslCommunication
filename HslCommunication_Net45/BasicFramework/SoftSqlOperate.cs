
#if !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

#if NET45
using System.Threading.Tasks;
#endif

namespace HslCommunication.BasicFramework
{
    //=================================================================================================
    //
    //       创建时间：2017年09月03日 20:56:18
    //       有关数据库操作的方法，进行总结精简，提供了三个非常常用的方法方便调用
    //
    //=================================================================================================



    /// <summary>
    /// 数据库操作的相关类，包含了常用的方法，避免大量的重复代码
    /// </summary>
    public static class SoftSqlOperate
    {

        /// <summary>
        /// 普通的执行SQL语句，并返回影响行数，该方法应该放到try-catch代码块中
        /// </summary>
        /// <param name="conStr">数据库的连接字符串</param>
        /// <param name="cmdStr">sql语句，适合插入，更新，删除</param>
        /// <returns>返回受影响的行数</returns>
        /// <exception cref="SqlException"></exception>
        public static int ExecuteSql(string conStr, string cmdStr)
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                return ExecuteSql(conn, cmdStr);
            }
        }

        /// <summary>
        /// 普通的执行SQL语句，并返回影响行数，该方法应该放到try-catch代码块中
        /// </summary>
        /// <param name="conn">数据库的连接对象</param>
        /// <param name="cmdStr">sql语句，适合插入，更新，删除</param>
        /// <returns>返回受影响的行数</returns>
        /// <exception cref="SqlException"></exception>
        public static int ExecuteSql(SqlConnection conn, string cmdStr)
        {
            using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 选择数据表的执行SQL语句，并返回最终数据表，该方法应该放到try-catch代码块中
        /// </summary>
        /// <param name="conStr">数据库的连接字符串</param>
        /// <param name="cmdStr">sql语句，选择数据表的语句</param>
        /// <returns>结果数据表</returns>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static DataTable ExecuteSelectTable(string conStr, string cmdStr)
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                return ExecuteSelectTable(conn, cmdStr);
            }
        }

        /// <summary>
        /// 选择数据表的执行SQL语句，并返回最终数据表，该方法应该放到try-catch代码块中
        /// </summary>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="cmdStr">sql语句，选择数据表的语句</param>
        /// <returns>结果数据表</returns>
        /// <exception cref="SqlException"></exception>
        public static DataTable ExecuteSelectTable(SqlConnection conn, string cmdStr)
        {
            using (SqlDataAdapter sda = new SqlDataAdapter(cmdStr, conn))
            {
                using (DataSet ds = new DataSet())
                {
                    sda.Fill(ds);
                    return ds.Tables[0];
                }
            }
        }

        /// <summary>
        /// 选择指定类型数据集合执行SQL语句，并返回指定类型的数据集合，该方法应该放到try-catch代码块中
        /// </summary>
        /// <param name="conStr">数据库的连接字符串</param>
        /// <param name="cmdStr">sql语句，选择数据表的语句</param>
        /// <returns>结果数据集合</returns>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static List<T> ExecuteSelectEnumerable<T>(string conStr, string cmdStr) where T : ISqlDataType, new()
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                return ExecuteSelectEnumerable<T>(conn, cmdStr);
            }
        }

        /// <summary>
        /// 选择指定类型数据集合执行SQL语句，并返回指定类型的数据集合，该方法应该放到try-catch代码块中
        /// </summary>
        /// <param name="conn">数据库的连接对象</param>
        /// <param name="cmdStr">sql语句，选择数据表的语句</param>
        /// <returns>结果数据集合</returns>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static List<T> ExecuteSelectEnumerable<T>(SqlConnection conn, string cmdStr) where T : ISqlDataType, new()
        {
            using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
            {
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    List<T> list = new List<T>();
                    while (sdr.Read())
                    {
                        T item = new T();
                        item.LoadBySqlDataReader(sdr);
                        list.Add(item);
                    }
                    return list;
                }
            }
        }

        /// <summary>
        /// 更新指定类型数据执行SQL语句，并返回指定类型的数据集合，该方法应该放到try-catch代码块中
        /// </summary>
        /// <param name="conStr">数据库的连接字符串</param>
        /// <param name="cmdStr">sql语句，选择数据表的语句</param>
        /// <returns>结果数据</returns>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T ExecuteSelectObject<T>(string conStr, string cmdStr) where T : ISqlDataType, new()
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                return ExecuteSelectObject<T>(conn, cmdStr);
            }
        }

        /// <summary>
        /// 更新指定类型数据执行SQL语句，并返回指定类型的数据集合，该方法应该放到try-catch代码块中
        /// </summary>
        /// <param name="conn">数据库的连接对象</param>
        /// <param name="cmdStr">sql语句，选择数据表的语句</param>
        /// <returns>结果数据</returns>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T ExecuteSelectObject<T>(SqlConnection conn, string cmdStr) where T : ISqlDataType, new()
        {
            using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
            {
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.Read())
                    {
                        T item = new T();
                        item.LoadBySqlDataReader(sdr);
                        return item;
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }



        /// <summary>
        /// 用于选择聚合函数值的方法，例如Count，Average，Max，Min，Sum等最终只有一个结果值的对象
        /// </summary>
        /// <param name="conStr">数据库的连接字符串</param>
        /// <param name="cmdStr">sql语句，选择数据表的语句</param>
        /// <returns>返回的int数据</returns>
        public static int ExecuteSelectCount(string conStr, string cmdStr)
        {
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                return ExecuteSelectCount(conn, cmdStr);
            }
        }


        /// <summary>
        /// 用于选择聚合函数值的方法，例如Count，Average，Max，Min，Sum等最终只有一个结果值的对象
        /// </summary>
        /// <param name="conn">数据库的连接对象</param>
        /// <param name="cmdStr">sql语句，选择数据表的语句</param>
        /// <returns>返回的int数据</returns>
        public static int ExecuteSelectCount(SqlConnection conn, string cmdStr)
        {
            using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
            {
                int temp = 0;
                SqlDataReader sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    temp = Convert.ToInt32(sdr[0]);
                }
                sdr.Close();
                return temp;
            }
        }
    }


    /// <summary>
    /// 数据库对应类的读取接口
    /// </summary>
    public interface ISqlDataType
    {
        /// <summary>
        /// 根据sdr对象初始化数据的方法
        /// </summary>
        /// <param name="sdr">数据库reader对象</param>
        void LoadBySqlDataReader(SqlDataReader sdr);

    }
}


#endif