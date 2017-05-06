namespace iphlpapidemo
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshLowerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resolveIP = new System.Windows.Forms.ToolStripMenuItem();
            this.hideLowerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsGrid1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsGrid2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsDownloadedCosts = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listView3 = new System.Windows.Forms.ListView();
            this.colProcessProc = new System.Windows.Forms.ColumnHeader();
            this.colPacketsInProc = new System.Windows.Forms.ColumnHeader();
            this.colPacketsOutProc = new System.Windows.Forms.ColumnHeader();
            this.colUploadedProc = new System.Windows.Forms.ColumnHeader();
            this.colDownloadedProc = new System.Windows.Forms.ColumnHeader();
            this.colUploadingSpeed = new System.Windows.Forms.ColumnHeader();
            this.colDownloadingSpeed = new System.Windows.Forms.ColumnHeader();
            this.colUsedPorts = new System.Windows.Forms.ColumnHeader();
            this.colDownloadedCost = new System.Windows.Forms.ColumnHeader();
            this.listView4 = new System.Windows.Forms.ListView();
            this.colParameterName = new System.Windows.Forms.ColumnHeader();
            this.colValue = new System.Windows.Forms.ColumnHeader();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colStateUpper = new System.Windows.Forms.ColumnHeader();
            this.colLocalAddressUpper = new System.Windows.Forms.ColumnHeader();
            this.colRemoteAddressUpper = new System.Windows.Forms.ColumnHeader();
            this.colPIDUpper = new System.Windows.Forms.ColumnHeader();
            this.colProcessUpper = new System.Windows.Forms.ColumnHeader();
            this.colProtocolUpper = new System.Windows.Forms.ColumnHeader();
            this.colPacketsIn = new System.Windows.Forms.ColumnHeader();
            this.colPacketsOut = new System.Windows.Forms.ColumnHeader();
            this.colUploaded = new System.Windows.Forms.ColumnHeader();
            this.colDownloaded = new System.Windows.Forms.ColumnHeader();
            this.colIsDead = new System.Windows.Forms.ColumnHeader();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader12 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadingSpeedView = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadingSpeedView = new System.Windows.Forms.ToolStripMenuItem();
            this.trafficPriceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packetsOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.resetAllToolStripMenuItem,
            this.clearValuesToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(134, 70);
            this.contextMenuStrip1.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // resetAllToolStripMenuItem
            // 
            this.resetAllToolStripMenuItem.Name = "resetAllToolStripMenuItem";
            this.resetAllToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.resetAllToolStripMenuItem.Text = "Reset all";
            this.resetAllToolStripMenuItem.Click += new System.EventHandler(this.resetAllToolStripMenuItem_Click);
            // 
            // clearValuesToolStripMenuItem
            // 
            this.clearValuesToolStripMenuItem.Name = "clearValuesToolStripMenuItem";
            this.clearValuesToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.clearValuesToolStripMenuItem.Text = "Clear values";
            this.clearValuesToolStripMenuItem.Click += new System.EventHandler(this.clearValuesToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(804, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.refreshLowerToolStripMenuItem,
            this.trafficPriceToolStripMenuItem,
            this.resolveIP,
            this.hideLowerToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // sortToolStripMenuItem
            // 
            this.sortToolStripMenuItem.Name = "sortToolStripMenuItem";
            this.sortToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.sortToolStripMenuItem.Text = "Sort";
            this.sortToolStripMenuItem.Click += new System.EventHandler(this.sortToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.refreshToolStripMenuItem.Text = "Refresh upper";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // refreshLowerToolStripMenuItem
            // 
            this.refreshLowerToolStripMenuItem.Name = "refreshLowerToolStripMenuItem";
            this.refreshLowerToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.refreshLowerToolStripMenuItem.Text = "Refresh lower";
            this.refreshLowerToolStripMenuItem.Click += new System.EventHandler(this.refreshLowerToolStripMenuItem_Click);
            // 
            // resolveIP
            // 
            this.resolveIP.Checked = true;
            this.resolveIP.CheckOnClick = true;
            this.resolveIP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.resolveIP.Name = "resolveIP";
            this.resolveIP.Size = new System.Drawing.Size(166, 22);
            this.resolveIP.Text = "Resolve IP address";
            // 
            // hideLowerToolStripMenuItem
            // 
            this.hideLowerToolStripMenuItem.Checked = true;
            this.hideLowerToolStripMenuItem.CheckOnClick = true;
            this.hideLowerToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideLowerToolStripMenuItem.Name = "hideLowerToolStripMenuItem";
            this.hideLowerToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.hideLowerToolStripMenuItem.Text = "Hide lower";
            this.hideLowerToolStripMenuItem.Click += new System.EventHandler(this.hideLowerToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(163, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsGrid1,
            this.tsGrid2,
            this.tsDownloadedCosts});
            this.statusStrip1.Location = new System.Drawing.Point(0, 490);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(804, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsGrid1
            // 
            this.tsGrid1.Name = "tsGrid1";
            this.tsGrid1.Size = new System.Drawing.Size(36, 17);
            this.tsGrid1.Text = "Grid1:";
            // 
            // tsGrid2
            // 
            this.tsGrid2.Name = "tsGrid2";
            this.tsGrid2.Size = new System.Drawing.Size(36, 17);
            this.tsGrid2.Text = "Grid2:";
            // 
            // tsDownloadedCosts
            // 
            this.tsDownloadedCosts.Name = "tsDownloadedCosts";
            this.tsDownloadedCosts.Size = new System.Drawing.Size(101, 17);
            this.tsDownloadedCosts.Text = "Downloaded costs: ";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(804, 466);
            this.panel1.TabIndex = 5;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView2);
            this.splitContainer1.Size = new System.Drawing.Size(804, 466);
            this.splitContainer1.SplitterDistance = 275;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.listView1);
            this.splitContainer3.Size = new System.Drawing.Size(804, 275);
            this.splitContainer3.SplitterDistance = 267;
            this.splitContainer3.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listView3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.listView4);
            this.splitContainer2.Size = new System.Drawing.Size(267, 275);
            this.splitContainer2.SplitterDistance = 178;
            this.splitContainer2.TabIndex = 5;
            // 
            // listView3
            // 
            this.listView3.CheckBoxes = true;
            this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colProcessProc,
            this.colPacketsInProc,
            this.colPacketsOutProc,
            this.colUploadedProc,
            this.colDownloadedProc,
            this.colUploadingSpeed,
            this.colDownloadingSpeed,
            this.colUsedPorts,
            this.colDownloadedCost});
            this.listView3.ContextMenuStrip = this.contextMenuStrip1;
            this.listView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView3.FullRowSelect = true;
            this.listView3.GridLines = true;
            this.listView3.Location = new System.Drawing.Point(0, 0);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(267, 178);
            this.listView3.TabIndex = 5;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            this.listView3.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView3_ItemCheck);
            this.listView3.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView3_ItemSelectionChanged);
            // 
            // colProcessProc
            // 
            this.colProcessProc.Text = "Process";
            this.colProcessProc.Width = 140;
            // 
            // colPacketsInProc
            // 
            this.colPacketsInProc.Text = "Packets In";
            // 
            // colPacketsOutProc
            // 
            this.colPacketsOutProc.Text = "Packets Out";
            // 
            // colUploadedProc
            // 
            this.colUploadedProc.Text = "Uploaded";
            // 
            // colDownloadedProc
            // 
            this.colDownloadedProc.Text = "Downloaded";
            // 
            // colUploadingSpeed
            // 
            this.colUploadingSpeed.Text = "Uploading Speed";
            // 
            // colDownloadingSpeed
            // 
            this.colDownloadingSpeed.Text = "Downloading Speed";
            // 
            // colUsedPorts
            // 
            this.colUsedPorts.Text = "Used Ports";
            // 
            // colDownloadedCost
            // 
            this.colDownloadedCost.Text = "Cost";
            // 
            // listView4
            // 
            this.listView4.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colParameterName,
            this.colValue,
            this.columnHeader1});
            this.listView4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView4.GridLines = true;
            this.listView4.Location = new System.Drawing.Point(0, 0);
            this.listView4.Name = "listView4";
            this.listView4.Size = new System.Drawing.Size(267, 93);
            this.listView4.TabIndex = 0;
            this.listView4.UseCompatibleStateImageBehavior = false;
            this.listView4.View = System.Windows.Forms.View.Details;
            // 
            // colParameterName
            // 
            this.colParameterName.Text = "Parameter";
            this.colParameterName.Width = 100;
            // 
            // colValue
            // 
            this.colValue.Text = "Value";
            this.colValue.Width = 400;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colStateUpper,
            this.colLocalAddressUpper,
            this.colRemoteAddressUpper,
            this.colPIDUpper,
            this.colProcessUpper,
            this.colProtocolUpper,
            this.colPacketsIn,
            this.colPacketsOut,
            this.colUploaded,
            this.colDownloaded,
            this.colIsDead});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(533, 275);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // colStateUpper
            // 
            this.colStateUpper.Text = "State";
            // 
            // colLocalAddressUpper
            // 
            this.colLocalAddressUpper.Text = "Local address";
            this.colLocalAddressUpper.Width = 99;
            // 
            // colRemoteAddressUpper
            // 
            this.colRemoteAddressUpper.Text = "Remote address";
            this.colRemoteAddressUpper.Width = 95;
            // 
            // colPIDUpper
            // 
            this.colPIDUpper.Text = "PID";
            // 
            // colProcessUpper
            // 
            this.colProcessUpper.Text = "Process";
            // 
            // colProtocolUpper
            // 
            this.colProtocolUpper.Text = "Protocol";
            // 
            // colPacketsIn
            // 
            this.colPacketsIn.Text = "Packets In";
            // 
            // colPacketsOut
            // 
            this.colPacketsOut.Text = "Packets Out";
            // 
            // colUploaded
            // 
            this.colUploaded.Text = "Uploaded";
            // 
            // colDownloaded
            // 
            this.colDownloaded.Text = "Downloaded";
            // 
            // colIsDead
            // 
            this.colIsDead.Text = "IsDead";
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader12});
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(0, 0);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(804, 187);
            this.listView2.TabIndex = 3;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "State";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Local address";
            this.columnHeader7.Width = 99;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Remote address";
            this.columnHeader8.Width = 95;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "PID";
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Process";
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Protocol";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 200;
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uploadingSpeedView,
            this.downloadingSpeedView,
            this.uploadedToolStripMenuItem,
            this.packetsOutToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // uploadingSpeedView
            // 
            this.uploadingSpeedView.Checked = true;
            this.uploadingSpeedView.CheckOnClick = true;
            this.uploadingSpeedView.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uploadingSpeedView.Name = "uploadingSpeedView";
            this.uploadingSpeedView.Size = new System.Drawing.Size(167, 22);
            this.uploadingSpeedView.Text = "Uploading speed";
            this.uploadingSpeedView.Click += new System.EventHandler(this.uploadingSpeedView_Click);
            // 
            // downloadingSpeedView
            // 
            this.downloadingSpeedView.Checked = true;
            this.downloadingSpeedView.CheckOnClick = true;
            this.downloadingSpeedView.CheckState = System.Windows.Forms.CheckState.Checked;
            this.downloadingSpeedView.Name = "downloadingSpeedView";
            this.downloadingSpeedView.Size = new System.Drawing.Size(167, 22);
            this.downloadingSpeedView.Text = "Downloading speed";
            this.downloadingSpeedView.Click += new System.EventHandler(this.downloadingSpeedView_Click);
            // 
            // trafficPriceToolStripMenuItem
            // 
            this.trafficPriceToolStripMenuItem.Name = "trafficPriceToolStripMenuItem";
            this.trafficPriceToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.trafficPriceToolStripMenuItem.Text = "Traffic price...";
            this.trafficPriceToolStripMenuItem.Click += new System.EventHandler(this.trafficPriceToolStripMenuItem_Click);
            // 
            // uploadedToolStripMenuItem
            // 
            this.uploadedToolStripMenuItem.Checked = true;
            this.uploadedToolStripMenuItem.CheckOnClick = true;
            this.uploadedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uploadedToolStripMenuItem.Name = "uploadedToolStripMenuItem";
            this.uploadedToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.uploadedToolStripMenuItem.Text = "Uploaded";
            this.uploadedToolStripMenuItem.Click += new System.EventHandler(this.uploadedToolStripMenuItem_Click);
            // 
            // packetsOutToolStripMenuItem
            // 
            this.packetsOutToolStripMenuItem.Checked = true;
            this.packetsOutToolStripMenuItem.CheckOnClick = true;
            this.packetsOutToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.packetsOutToolStripMenuItem.Name = "packetsOutToolStripMenuItem";
            this.packetsOutToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.packetsOutToolStripMenuItem.Text = "Packets Out";
            this.packetsOutToolStripMenuItem.Click += new System.EventHandler(this.packetsOutToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 512);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "Applications Traffic Monitor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearValuesToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshLowerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resolveIP;
        private System.Windows.Forms.ToolStripMenuItem hideLowerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsGrid1;
        private System.Windows.Forms.ToolStripStatusLabel tsGrid2;
        private System.Windows.Forms.ToolStripStatusLabel tsDownloadedCosts;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.ColumnHeader colProcessProc;
        private System.Windows.Forms.ColumnHeader colPacketsInProc;
        private System.Windows.Forms.ColumnHeader colPacketsOutProc;
        private System.Windows.Forms.ColumnHeader colUploadedProc;
        private System.Windows.Forms.ColumnHeader colDownloadedProc;
        private System.Windows.Forms.ColumnHeader colUploadingSpeed;
        private System.Windows.Forms.ColumnHeader colDownloadingSpeed;
        private System.Windows.Forms.ColumnHeader colUsedPorts;
        private System.Windows.Forms.ColumnHeader colDownloadedCost;
        private System.Windows.Forms.ListView listView4;
        private System.Windows.Forms.ColumnHeader colParameterName;
        private System.Windows.Forms.ColumnHeader colValue;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colStateUpper;
        private System.Windows.Forms.ColumnHeader colLocalAddressUpper;
        private System.Windows.Forms.ColumnHeader colRemoteAddressUpper;
        private System.Windows.Forms.ColumnHeader colPIDUpper;
        private System.Windows.Forms.ColumnHeader colProcessUpper;
        private System.Windows.Forms.ColumnHeader colProtocolUpper;
        private System.Windows.Forms.ColumnHeader colPacketsIn;
        private System.Windows.Forms.ColumnHeader colPacketsOut;
        private System.Windows.Forms.ColumnHeader colUploaded;
        private System.Windows.Forms.ColumnHeader colDownloaded;
        private System.Windows.Forms.ColumnHeader colIsDead;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadingSpeedView;
        private System.Windows.Forms.ToolStripMenuItem downloadingSpeedView;
        private System.Windows.Forms.ToolStripMenuItem trafficPriceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem packetsOutToolStripMenuItem;

    }
}

