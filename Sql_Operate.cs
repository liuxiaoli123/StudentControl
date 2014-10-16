using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Common;

namespace Student
{
    class Sql_Operate
    {
        protected string myConn;
        protected SqlConnection sqlConn;
        protected DataSet ds = new DataSet();
	    public Sql_Operate(string db_ip,string db_username,string db_pwd)
	    {
            myConn = String.Format("server={0};uid={1};pwd={2};database=PlatForm", db_ip, db_username, db_pwd);
            sqlConn = new SqlConnection(myConn);
	    }
        /// <summary>
        /// 打开一个sqlconn连接
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            try
            {
                if (sqlConn == null)
                {
                    sqlConn.Open();
                }
                else
                {
                    if (sqlConn.State.Equals(ConnectionState.Closed))
                        sqlConn.Open();
                    
                }
                return true;
            }
            catch (SqlException sqlE)
            {
                MessageBox.Show("数据库连接异常: " + sqlE.Message);
                return false;
            }

        }

        public void Close()
        {
            if (sqlConn.State.Equals(ConnectionState.Open))
            sqlConn.Close();
        }
        
        /// <summary>
        /// 执行插入、删除、更新操作
        /// </summary>
        /// <param name="sql">sql为查询语句</param>
        /// <returns></returns>
        public bool ExceSql(string sql)
        {
            if(sqlConn.State.Equals(ConnectionState.Closed))
                Open();
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }

        }
        
        /// <summary>
        /// 执行选择操作，返回一个数据阅读器对象
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public SqlDataReader ExceRead(string sql)
        {
            if (sqlConn.State.Equals(ConnectionState.Closed))
                Open();
            SqlCommand cmd = new SqlCommand(sql, sqlConn);
            SqlDataReader read = cmd.ExecuteReader();
            return read;
        }

        /// <summary>
        /// 返回数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet ReadToDataset(string sql,string tableName)
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, myConn);
            //在try结构中填充数据集
            da.Fill(ds,tableName);
            return ds;
        }
        /// <summary>
        /// 增加表进入ds
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        //public DataSet AddTableToDataset(string sql, string tableName)
        //{
        //    SqlDataAdapter da = new SqlDataAdapter(sql, myConn);
        //    da.Fill(ds, tableName);

        //}

        /// <summary>
        /// 返回数据库连接对象
        /// </summary>
        /// <returns></returns>
        public SqlConnection getConnection()
        {
            return sqlConn;
        }
    }

    static class SqlServerData
    {
        private static string exam_rule;
        private static string exam_title;
        private static string exam_ver;

        /// <summary>
        /// 考试须知
        /// </summary>
        public static string Exam_Role
        {
            get { return exam_rule; }
        }

        /// <summary>
        /// 考试名称
        /// </summary>
        public static string Exam_Title
        {
            get { return exam_title; }
        }

        /// <summary>
        /// 考试版本号
        /// </summary>
        public static string Exam_Ver
        {
            get { return exam_ver; }
        }

        public static void ReadTestKnown()
        {
             Sql_Operate testinfo = new Sql_Operate(GloabalData.GBL_DATABASE_SERVER, GloabalData.GBL_DATABASE_USER,
                GloabalData.GBL_DATABASE_PWD);
            testinfo.Open();
            string strSql = "select exam_rule,exam_tilte,exam_ver from Config where id=1";
            try
            {
                SqlDataReader dr = testinfo.ExceRead(strSql);
                if (dr.Read())
                {
                    exam_rule = dr["exam_rule"].ToString();
                    exam_title = dr["exam_tilte"].ToString();
                    exam_ver = dr["exam_ver"].ToString();  
                }
                else
                {
                    MessageBox.Show("读取考试须知错误");
                }
            }
            catch (SqlException e1)
            {
                MessageBox.Show("连接数据库不成功"+e1.Message);
            }
            testinfo.Close();
        }
    }
}
