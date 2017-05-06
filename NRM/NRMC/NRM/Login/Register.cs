using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpPcap;

namespace Login
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 验证用户名是否符合要求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerTextBoxUserName_Leave(object sender, EventArgs e)
        {
            string str = registerTextBoxUserName.Text;
            
            if(str.Length < 4 || str.Length > 8)
            {
                MessageBox.Show("用户名必须为4-8位");
            }else
            {
                SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NRMDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");


                string sql = "select * from UserInfo where UserInfo.user_name=@username";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@username", registerTextBoxUserName.Text);
                

                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("用户名已被注册");
                }
            }
        }


        /// <summary>
        /// 验证密码是否符合要求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerTextBoxPassword_Leave(object sender, EventArgs e)
        {
            char[] arr = registerTextBoxPassword.Text.ToCharArray();
            if (arr.Length < 6 || arr.Length > 12)
            {
                MessageBox.Show("密码必须为6-12位");
            }
        }
       
       
        /// <summary>
        /// 验证两次密码是否一致
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void registerTextBoxPasswordConfirm_Leave(object sender, EventArgs e)
        {
            bool bl = registerTextBoxPassword.Text.Equals(registerTextBoxPasswordConfirm.Text);
            if (!bl)
                MessageBox.Show("两次密码不一致");
        }


        /// <summary>
        /// 注册并向数据库添加数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegisterSubmit_Click(object sender, EventArgs e)
        {
         
            SqlConnection sqlcon = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NRMDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            sqlcon.Open();
            string InsertSql = "insert into UserInfo(user_name, user_password) values ('" + registerTextBoxUserName.Text + "','" + registerTextBoxPassword.Text + "')";

            SqlCommand cmd1 = new SqlCommand(InsertSql, sqlcon);

            cmd1.ExecuteNonQuery();
            StaticParty.regForm.Close();
        }

        /// <summary>
        /// 返回登录页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegisterBack_Click(object sender, EventArgs e)
        {
            StaticParty.regForm.Close();
        }
    }
}
