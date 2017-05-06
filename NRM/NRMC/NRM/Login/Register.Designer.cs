namespace Login
{
    partial class Register
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.registerTextBoxUserName = new System.Windows.Forms.TextBox();
            this.registerTextBoxPassword = new System.Windows.Forms.TextBox();
            this.registerTextBoxPasswordConfirm = new System.Windows.Forms.TextBox();
            this.btnRegisterSubmit = new System.Windows.Forms.Button();
            this.btnRegisterBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(454, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(418, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "用户名（数字或字母、4-8位）";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(454, 337);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(403, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "密码（数字和字母、6-12位）";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(454, 461);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(343, 30);
            this.label3.TabIndex = 2;
            this.label3.Text = "确认密码（两次需一致）";
            // 
            // registerTextBoxUserName
            // 
            this.registerTextBoxUserName.Location = new System.Drawing.Point(459, 231);
            this.registerTextBoxUserName.Multiline = true;
            this.registerTextBoxUserName.Name = "registerTextBoxUserName";
            this.registerTextBoxUserName.Size = new System.Drawing.Size(483, 62);
            this.registerTextBoxUserName.TabIndex = 3;
            this.registerTextBoxUserName.Leave += new System.EventHandler(this.registerTextBoxUserName_Leave);
            // 
            // registerTextBoxPassword
            // 
            this.registerTextBoxPassword.Location = new System.Drawing.Point(459, 370);
            this.registerTextBoxPassword.Multiline = true;
            this.registerTextBoxPassword.Name = "registerTextBoxPassword";
            this.registerTextBoxPassword.PasswordChar = '*';
            this.registerTextBoxPassword.Size = new System.Drawing.Size(483, 62);
            this.registerTextBoxPassword.TabIndex = 4;
            this.registerTextBoxPassword.Leave += new System.EventHandler(this.registerTextBoxPassword_Leave);
            // 
            // registerTextBoxPasswordConfirm
            // 
            this.registerTextBoxPasswordConfirm.Location = new System.Drawing.Point(459, 494);
            this.registerTextBoxPasswordConfirm.Multiline = true;
            this.registerTextBoxPasswordConfirm.Name = "registerTextBoxPasswordConfirm";
            this.registerTextBoxPasswordConfirm.PasswordChar = '*';
            this.registerTextBoxPasswordConfirm.Size = new System.Drawing.Size(483, 62);
            this.registerTextBoxPasswordConfirm.TabIndex = 5;
            this.registerTextBoxPasswordConfirm.Leave += new System.EventHandler(this.registerTextBoxPasswordConfirm_Leave);
            // 
            // btnRegisterSubmit
            // 
            this.btnRegisterSubmit.Location = new System.Drawing.Point(805, 686);
            this.btnRegisterSubmit.Name = "btnRegisterSubmit";
            this.btnRegisterSubmit.Size = new System.Drawing.Size(137, 56);
            this.btnRegisterSubmit.TabIndex = 6;
            this.btnRegisterSubmit.Text = "提交";
            this.btnRegisterSubmit.UseVisualStyleBackColor = true;
            this.btnRegisterSubmit.Click += new System.EventHandler(this.btnRegisterSubmit_Click);
            // 
            // btnRegisterBack
            // 
            this.btnRegisterBack.Location = new System.Drawing.Point(459, 686);
            this.btnRegisterBack.Name = "btnRegisterBack";
            this.btnRegisterBack.Size = new System.Drawing.Size(137, 56);
            this.btnRegisterBack.TabIndex = 7;
            this.btnRegisterBack.Text = "返回";
            this.btnRegisterBack.UseVisualStyleBackColor = true;
            this.btnRegisterBack.Click += new System.EventHandler(this.btnRegisterBack_Click);
            // 
            // Register
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1406, 1051);
            this.Controls.Add(this.btnRegisterBack);
            this.Controls.Add(this.btnRegisterSubmit);
            this.Controls.Add(this.registerTextBoxPasswordConfirm);
            this.Controls.Add(this.registerTextBoxPassword);
            this.Controls.Add(this.registerTextBoxUserName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Register";
            this.Text = "Register";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox registerTextBoxUserName;
        private System.Windows.Forms.TextBox registerTextBoxPassword;
        private System.Windows.Forms.TextBox registerTextBoxPasswordConfirm;
        private System.Windows.Forms.Button btnRegisterSubmit;
        private System.Windows.Forms.Button btnRegisterBack;
    }
}