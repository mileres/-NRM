using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //连接字符串
        const string CONN_STRING = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NRMDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        //适配器对象
        private SqlDataAdapter adt = new SqlDataAdapter("select user_name as 用户名, user_password as 密码 from UserInfo", CONN_STRING);
        //创建 存放对象的表格。
        private DataTable table = new DataTable();
        private void button1_Click(object sender, EventArgs e)
        {
            //清空表格
            table.Clear();
            try
            {
                //填充数据
                adt.Fill(table);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show("已填充" + table.Rows.Count.ToString() + "条记录。");
            this.dataGridView1.DataSource = table;
        }
    }
}
