using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Test0
{
    class Program
    {
        static void Main(string[] args)
        {
            //声明链接对象
            SqlConnection conn = new SqlConnection(Test0.Properties.Settings.Default.MyCnnString);
            //打开连接
            try
            {
                //打开链接
                conn.Open();
                //访问数据库并进行相关操作
                System.Threading.Thread.Sleep(3000);
                //
                Console.WriteLine("success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //关闭数据库
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
                Console.WriteLine("database has been closed");
            }
            Console.Read();
        }
    }
}
