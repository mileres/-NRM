namespace Application_Monitor
{
    partial class Monitor
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txt_Content = new System.Windows.Forms.TextBox();
            this.bgw_MonitorPCStatus = new System.ComponentModel.BackgroundWorker();
            this.cbo_Process = new System.Windows.Forms.ComboBox();
            this.btn_Monitor = new System.Windows.Forms.Button();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.btn_LoadProcess = new System.Windows.Forms.Button();
            this.bgw_MonitorNetwork = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // txt_Content
            // 
            this.txt_Content.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txt_Content.Location = new System.Drawing.Point(12, 12);
            this.txt_Content.Multiline = true;
            this.txt_Content.Name = "txt_Content";
            this.txt_Content.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_Content.Size = new System.Drawing.Size(570, 517);
            this.txt_Content.TabIndex = 0;
            // 
            // bgw_MonitorPCStatus
            // 
            this.bgw_MonitorPCStatus.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_ShowInfo_DoWork);
            this.bgw_MonitorPCStatus.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_ShowInfo_RunWorkerCompleted);
            // 
            // cbo_Process
            // 
            this.cbo_Process.FormattingEnabled = true;
            this.cbo_Process.Location = new System.Drawing.Point(588, 12);
            this.cbo_Process.Name = "cbo_Process";
            this.cbo_Process.Size = new System.Drawing.Size(290, 20);
            this.cbo_Process.TabIndex = 2;
            // 
            // btn_Monitor
            // 
            this.btn_Monitor.Location = new System.Drawing.Point(588, 75);
            this.btn_Monitor.Name = "btn_Monitor";
            this.btn_Monitor.Size = new System.Drawing.Size(75, 23);
            this.btn_Monitor.TabIndex = 3;
            this.btn_Monitor.Text = "Begin";
            this.btn_Monitor.UseVisualStyleBackColor = true;
            this.btn_Monitor.Click += new System.EventHandler(this.btn_Monitor_Click);
            // 
            // btn_Stop
            // 
            this.btn_Stop.Location = new System.Drawing.Point(710, 75);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(75, 23);
            this.btn_Stop.TabIndex = 3;
            this.btn_Stop.Text = "Stop";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // btn_LoadProcess
            // 
            this.btn_LoadProcess.Location = new System.Drawing.Point(588, 38);
            this.btn_LoadProcess.Name = "btn_LoadProcess";
            this.btn_LoadProcess.Size = new System.Drawing.Size(144, 23);
            this.btn_LoadProcess.TabIndex = 4;
            this.btn_LoadProcess.Text = "Load Process";
            this.btn_LoadProcess.UseVisualStyleBackColor = true;
            this.btn_LoadProcess.Click += new System.EventHandler(this.btn_LoadProcess_Click);
            // 
            // bgw_MonitorNetwork
            // 
            this.bgw_MonitorNetwork.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_MonitorNetwork_DoWork);
            this.bgw_MonitorNetwork.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_MonitorNetwork_RunWorkerCompleted);
            // 
            // Monitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 541);
            this.Controls.Add(this.btn_LoadProcess);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.btn_Monitor);
            this.Controls.Add(this.cbo_Process);
            this.Controls.Add(this.txt_Content);
            this.Name = "Monitor";
            this.Text = "Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Monitor_FormClosing);
            this.Shown += new System.EventHandler(this.Monitor_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_Content;
        private System.ComponentModel.BackgroundWorker bgw_MonitorPCStatus;
        private System.Windows.Forms.ComboBox cbo_Process;
        private System.Windows.Forms.Button btn_Monitor;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.Button btn_LoadProcess;
        private System.ComponentModel.BackgroundWorker bgw_MonitorNetwork;
    }
}

