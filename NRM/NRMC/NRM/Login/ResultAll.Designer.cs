namespace Login
{
    partial class ResultAll
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
            this.RtextBox1 = new System.Windows.Forms.TextBox();
            this.RtextBox2 = new System.Windows.Forms.TextBox();
            this.RtextBox3 = new System.Windows.Forms.TextBox();
            this.RtextBox4 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RtextBox1
            // 
            this.RtextBox1.Location = new System.Drawing.Point(314, 131);
            this.RtextBox1.Multiline = true;
            this.RtextBox1.Name = "RtextBox1";
            this.RtextBox1.Size = new System.Drawing.Size(749, 91);
            this.RtextBox1.TabIndex = 0;
            // 
            // RtextBox2
            // 
            this.RtextBox2.Location = new System.Drawing.Point(314, 281);
            this.RtextBox2.Multiline = true;
            this.RtextBox2.Name = "RtextBox2";
            this.RtextBox2.Size = new System.Drawing.Size(749, 91);
            this.RtextBox2.TabIndex = 1;
            // 
            // RtextBox3
            // 
            this.RtextBox3.Location = new System.Drawing.Point(314, 443);
            this.RtextBox3.Multiline = true;
            this.RtextBox3.Name = "RtextBox3";
            this.RtextBox3.Size = new System.Drawing.Size(749, 91);
            this.RtextBox3.TabIndex = 2;
            // 
            // RtextBox4
            // 
            this.RtextBox4.Location = new System.Drawing.Point(314, 605);
            this.RtextBox4.Multiline = true;
            this.RtextBox4.Name = "RtextBox4";
            this.RtextBox4.Size = new System.Drawing.Size(749, 91);
            this.RtextBox4.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(593, 822);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(202, 68);
            this.button1.TabIndex = 4;
            this.button1.Text = "获取数据";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ResultAll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1467, 1251);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.RtextBox4);
            this.Controls.Add(this.RtextBox3);
            this.Controls.Add(this.RtextBox2);
            this.Controls.Add(this.RtextBox1);
            this.Name = "ResultAll";
            this.Text = "ResultAll";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox RtextBox1;
        private System.Windows.Forms.TextBox RtextBox2;
        private System.Windows.Forms.TextBox RtextBox3;
        private System.Windows.Forms.TextBox RtextBox4;
        private System.Windows.Forms.Button button1;
    }
}