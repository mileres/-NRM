using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace Test1
{
    class Program
    {
       
        static void Main(string[] args)
        {
            //测试连接数据库
            #region
            //SqlConnection conn = new SqlConnection(Test1.Properties.Settings.Default.MyCnnString);
            ////打开连接
            //try
            //{
            //    //打开链接
            //    conn.Open();
            //    //访问数据库并进行相关操作
            //    System.Threading.Thread.Sleep(3000);
            //    //
            //    Console.WriteLine("success");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            //finally
            //{
            //    //关闭数据库
            //    if (conn.State == System.Data.ConnectionState.Open)
            //        conn.Close();
            //    Console.WriteLine("database has been closed");
            //}
            //Console.Read();
            #endregion


            //测试读取数据库
            //项目的连接字符串
            string cnnString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NRMDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            //初始化数据库连接的实例       
            SqlConnection conn = new SqlConnection(cnnString);
            try
            {
                //打开链接，打开后就可以对数据库进行操作了。
                conn.Open();
                //创建数据库连接对象conn的命令对象
                SqlCommand cmd = conn.CreateCommand();
                //设置命令行对象的SQL语句
                cmd.CommandText = "SELECT TOP 10 * FROM products";
                //返回SqlDataReader实例
                SqlDataReader reader = cmd.ExecuteReader();
                //当到达 表 的最后一行时 返回FALSE ，否则返回 TRUE。
                //通过循环来读取每一条数据。
                while (reader.Read())
                {
                    Console.WriteLine("产品ID：{0}，产品名称：{1}", reader.GetInt32(0), reader.GetString(1));
                }
                //关闭 阅读器
                reader.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //关闭数据库连接。
                conn.Close();
            }
            Console.Read();
        }
    }
}
