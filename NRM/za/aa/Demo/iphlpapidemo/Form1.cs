using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IpHlpApidotnet;
using System.Diagnostics;
using AppTrafficMonitor;
using System.Threading;
using System.IO;
using log4net;

namespace iphlpapidemo
{
    public partial class Form1 : Form
    {
        private IpHlpApidotnet.IPHelper _IpHlpAPI;
        public AddNewItem m_AddNewItem;
        public InsertItem m_InsertItem;
        public ChangedItem m_ChangedItem;
        public DeletedItem m_DeletedItem;

        public delegate void AddNewItem(TCPUDPConnection item);
        public delegate void InsertItem(TCPUDPConnection item, int Position);
        public delegate void ChangedItem(TCPUDPConnection item, int Position);
        public delegate void DeletedItem(TCPUDPConnection item, int Position);

        public AddNewItemApp m_AddNewItemApp;
        public InsertItemApp m_InsertItemApp;
        public ChangedItemApp m_ChangedItemApp;

        public delegate void AddNewItemApp(AppTCPUDPConnection item);
        public delegate void InsertItemApp(AppTCPUDPConnection item, int Position);
        public delegate void ChangedItemApp(AppTCPUDPConnection item, int Position);
        
        //private TCPUDPConnections conns = newTCPUDPConnections();
        private ConnectionsWatcher conns = new ConnectionsWatcher();
        public Form1()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string file = path + "\\log4net.config";
            log4net.Config.DOMConfigurator.Configure(new System.IO.FileInfo(file));

            m_AddNewItem = new AddNewItem(this.AddItem);
            m_InsertItem = new InsertItem(this.AddOrInsertItem);
            m_ChangedItem = new ChangedItem(this.ChangeItem);
            m_DeletedItem = new DeletedItem(this.DeleteItem);

            m_AddNewItemApp = new AddNewItemApp(this.AddItemApp);
            m_InsertItemApp = new InsertItemApp(this.AddOrInsertItemApp);
            m_ChangedItemApp = new ChangedItemApp(this.ChangeItemApp);

            InitializeComponent();

            conns.DownloadPricePerMegabyte = 4;
            conns.UploadPricePerMegabyte = 0;

            _IpHlpAPI = new IpHlpApidotnet.IPHelper();

            //hack to initially try to reduce the memory footprint of the app (admin only)
            try
            {
                Process loProcess = Process.GetCurrentProcess();
                loProcess.MaxWorkingSet = loProcess.MaxWorkingSet;
                loProcess.Dispose();
            }
            catch { }
            System.Timers.Timer ShrinkTimer = new System.Timers.Timer();
            ShrinkTimer.Interval = 60000;
            ShrinkTimer.Elapsed += new System.Timers.ElapsedEventHandler(ShrinkTimer_Elapsed);
            ShrinkTimer.Start();
        }

        void MonitoredApplications_ItemChanged(object sender, AppTCPUDPConnection item, int Pos)
        {
            try
            {
                this.Invoke(m_ChangedItemApp, item, Pos);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        void MonitoredApplications_ItemInserted(object sender, AppTCPUDPConnection item, int Position)
        {
            try
            {
                this.Invoke(m_InsertItemApp, item, Position);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        void MonitoredApplications_ItemAdded(object sender, AppTCPUDPConnection item)
        {
            try
            {
                this.Invoke(m_AddNewItemApp, item);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        void ShrinkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //hack to initially try to reduce the memory footprint of the app (admin only)
            try
            {
                Process loProcess = Process.GetCurrentProcess();
                loProcess.MaxWorkingSet = loProcess.MaxWorkingSet;
                loProcess.Dispose();
            }
            catch { }
        }

        void conns_ItemDeleted(object sender, TCPUDPConnection item, int Position)
        {
            try
            {
                this.Invoke(m_DeletedItem, item, Position);
            }
            catch { }
        }

        void conns_ItemChanged(object sender, TCPUDPConnection item, int Position)
        {
            try
            {
                this.Invoke(m_ChangedItem, item, Position);
            }
            catch { }
        }

        void conns_ItemInserted(object sender, TCPUDPConnection item, int Position)
        {
            try
            {
                this.Invoke(m_InsertItem, item, Position);
            }
            catch { }
        }

        void conns_ItemAdded(object sender, TCPUDPConnection item)
        {
            try
            {
                this.Invoke(m_AddNewItem, item);
            }
            catch {}
        }

        private void AddItemApp(AppTCPUDPConnection conn)
        {
            try
            {
                AddOrInsertItemApp(conn, -1);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }

        private void AddOrInsertItemApp(AppTCPUDPConnection conn, int Position)
        {
            try
            {
                ListViewItem lvi = null;
                if (Position != -1)
                {
                    lvi = listView3.Items.Insert(Position, conn.ProcessName);
                }
                else
                {
                    lvi = listView3.Items.Add(conn.ProcessName);
                }
                lvi.Checked = conn.IsMonitored;
                lvi.SubItems.Add(conn.PacketsIn.ToString());// lvi.SubItems.Add(conn.Local.ToString());
                lvi.SubItems.Add(conn.PacketsOut.ToString());
                lvi.SubItems.Add(Utils.TrafficToStr(conn.UploadedUseful, conn.UploadedReal));
                lvi.SubItems.Add(Utils.TrafficToStr(conn.DownloadedUseful, conn.DownloadedReal));
                lvi.SubItems.Add(conn.UploadingSpeed);
                lvi.SubItems.Add(conn.DownloadingSpeed);
                lvi.SubItems.Add(conn.UsedPorts);
                lvi.SubItems.Add(conn.DownloadedCosts.ToString("C"));// + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol);
            }
            catch { }
        }

        private void ChangeItemApp(AppTCPUDPConnection conn, int Position)
        {
            try
            {
                ListViewItem lvi = listView3.Items[Position];
                lvi.Checked = conn.IsMonitored;
                lvi.SubItems[colProcessProc.Index].Text = conn.ProcessName;
                lvi.SubItems[colPacketsInProc.Index].Text = conn.PacketsIn.ToString();
                lvi.SubItems[colPacketsOutProc.Index].Text = conn.PacketsOut.ToString();
                lvi.SubItems[colUploadedProc.Index].Text = Utils.TrafficToStr(conn.UploadedUseful, conn.UploadedReal);
                lvi.SubItems[colDownloadedProc.Index].Text = Utils.TrafficToStr(conn.DownloadedUseful, conn.DownloadedReal);
                lvi.SubItems[colUploadingSpeed.Index].Text = conn.UploadingSpeed;
                lvi.SubItems[colDownloadingSpeed.Index].Text = conn.DownloadingSpeed;
                lvi.SubItems[colUsedPorts.Index].Text = conn.UsedPorts;
                lvi.SubItems[colDownloadedCost.Index].Text = conn.DownloadedCosts.ToString("C");// +System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
                tsDownloadedCosts.Text = "Costs: " + conns.MonitoredApplications.DownloadedCosts.ToString("C");
            }
            catch { }
        }

        private void AddItem(TCPUDPConnection conn)
        {
            try
            {
                AddOrInsertItem(conn, -1);
            }
            catch { }
        }

        private void AddOrInsertItem(TCPUDPConnection conn, int Position)
        {
            string state = String.Empty;
            if (conn.Protocol == Protocol.TCP)
                state = conn.State;
            ListViewItem lvi = null;
            if (Position != -1)
            {
                lvi = listView1.Items.Insert(Position, state);
            }
            else
            {
                lvi = listView1.Items.Add(state);
            }
            lvi.SubItems.Add(conn.LocalAddress);// lvi.SubItems.Add(conn.Local.ToString());
            if (conn.Protocol == Protocol.TCP)
                lvi.SubItems.Add(conn.RemoteAddress);//lvi.SubItems.Add(conn.Remote.ToString());
            else
                lvi.SubItems.Add("");
            lvi.SubItems.Add(conn.PID.ToString());
            lvi.SubItems.Add(conn.ProcessName);
            lvi.SubItems.Add(conn.Protocol.ToString());
            if (conn is ExTCPUDPConnection)
            {
                lvi.SubItems.Add((conn as ExTCPUDPConnection).PacketsIn.ToString());
                lvi.SubItems.Add((conn as ExTCPUDPConnection).PacketsOut.ToString());
                lvi.SubItems.Add(Utils.TrafficToStr((conn as ExTCPUDPConnection).UploadedUsefulTotal, (conn as ExTCPUDPConnection).UploadedRealTotal));
                lvi.SubItems.Add(Utils.TrafficToStr((conn as ExTCPUDPConnection).DownloadedUsefulTotal, (conn as ExTCPUDPConnection).DownloadedRealTotal));
            }
            else
            {
                lvi.SubItems.Add("0");
                lvi.SubItems.Add("0");
                lvi.SubItems.Add("0/0");
                lvi.SubItems.Add("0/0");
            }
            lvi.SubItems.Add(conn.IsDead.ToString());
            tsGrid1.Text = "Grid1 items: " + listView1.Items.Count.ToString();
        }

        private void ChangeItem(TCPUDPConnection conn, int Position)
        {
            string state = String.Empty;
            if (conn.Protocol == Protocol.TCP)
                state = conn.State;
            ListViewItem lvi = listView1.Items[Position];
            lvi.SubItems[colStateUpper.Index].Text = state;
            lvi.SubItems[colPIDUpper.Index].Text = conn.PID.ToString();
            lvi.SubItems[colProcessUpper.Index].Text = conn.ProcessName;
            if (conn is ExTCPUDPConnection)
            {
                lvi.SubItems[colPacketsIn.Index].Text = (conn as ExTCPUDPConnection).PacketsIn.ToString();
                lvi.SubItems[colPacketsOut.Index].Text = (conn as ExTCPUDPConnection).PacketsOut.ToString();
                lvi.SubItems[colUploaded.Index].Text = Utils.TrafficToStr((conn as ExTCPUDPConnection).UploadedUsefulTotal, (conn as ExTCPUDPConnection).UploadedRealTotal);
                lvi.SubItems[colDownloaded.Index].Text = Utils.TrafficToStr((conn as ExTCPUDPConnection).DownloadedUsefulTotal, (conn as ExTCPUDPConnection).DownloadedRealTotal);
            }
            lvi.SubItems[colIsDead.Index].Text = (conn as ExTCPUDPConnection).IsDead.ToString();
            Application.DoEvents();
        }

        private void DeleteItem(TCPUDPConnection conn, int Position)
        {
            listView1.Items.RemoveAt(Position);
            tsGrid1.Text = "Grid1 items: " + listView1.Items.Count.ToString();
        }

        private void RefreshLower()
        {
            listView2.Items.Clear();
            _IpHlpAPI.GetExTcpConnections();
            //TCPUDPConnections.LocalHostName;
            for (int i = 0; i < _IpHlpAPI.TcpExConnections.dwNumEntries; i++)
            {
                ListViewItem lvi = listView2.Items.Add(_IpHlpAPI.TcpExConnections.table[i].StrgState);
                if (resolveIP.Checked)
                {
                    //lvi.SubItems.Add(_IpHlpAPI.TcpExConnections.table[i].Local.ToString());
                    //lvi.SubItems.Add(_IpHlpAPI.TcpExConnections.table[i].Remote.ToString());
                    lvi.SubItems.Add(Utils.GetHostName(_IpHlpAPI.TcpExConnections.table[i].Local, conns.LocalHostName));
                    lvi.SubItems.Add(Utils.GetHostName(_IpHlpAPI.TcpExConnections.table[i].Remote, conns.LocalHostName));
                }
                else
                {
                    lvi.SubItems.Add(_IpHlpAPI.TcpExConnections.table[i].Local.ToString());
                    lvi.SubItems.Add(_IpHlpAPI.TcpExConnections.table[i].Remote.ToString());
                }
                lvi.SubItems.Add(_IpHlpAPI.TcpExConnections.table[i].dwProcessId.ToString());
                lvi.SubItems.Add(_IpHlpAPI.TcpExConnections.table[i].ProcessName);
                lvi.SubItems.Add("TCP");
            }
            _IpHlpAPI.GetExUdpConnections();
            for (int i = 0; i < _IpHlpAPI.UdpExConnections.dwNumEntries; i++)
            {
                ListViewItem lvi = listView2.Items.Add("");
                if (resolveIP.Checked)
                {
                    //lvi.SubItems.Add(_IpHlpAPI.UdpExConnections.table[i].Local.ToString());
                    lvi.SubItems.Add(Utils.GetHostName(_IpHlpAPI.UdpExConnections.table[i].Local, conns.LocalHostName));
                }
                else
                {
                    lvi.SubItems.Add(_IpHlpAPI.UdpExConnections.table[i].Local.ToString());
                }
                lvi.SubItems.Add("");
                lvi.SubItems.Add(_IpHlpAPI.UdpExConnections.table[i].dwProcessId.ToString());
                lvi.SubItems.Add(_IpHlpAPI.UdpExConnections.table[i].ProcessName);
                lvi.SubItems.Add("UDP");
            }
            tsGrid2.Text = "Grid2 items: " + listView2.Items.Count.ToString();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void sortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            conns.Sort();
            this.RefreshUpper();
        }

        private void RefreshUpper()
        {
            listView1.Items.Clear();
            for (int i = 0; i < conns.Count; i++)
            {
                AddItem(conns[i]); //.conn
            }
            listView3.Items.Clear();
            for (int i = 0; i < conns.MonitoredApplications.Count; i++)
            {
                AddItemApp(conns.MonitoredApplications[i]); //.conn
            }
            tsGrid1.Text = "Grid1 items: " + listView1.Items.Count.ToString();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.RefreshUpper();
        }

        private void refreshLowerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.RefreshLower();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            conns.Stop();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conns.ItemAdded += new TCPUDPConnections.ItemAddedEvent(conns_ItemAdded);
            conns.ItemInserted += new TCPUDPConnections.ItemInsertedEvent(conns_ItemInserted);
            conns.ItemChanged += new TCPUDPConnections.ItemChangedEvent(conns_ItemChanged);
            conns.ItemDeleted += new TCPUDPConnections.ItemDeletedEvent(conns_ItemDeleted);

            conns.MonitoredApplications.ItemAdded += new MonitoredApplications.ItemAddedEvent(MonitoredApplications_ItemAdded);
            conns.MonitoredApplications.ItemInserted += new MonitoredApplications.ItemInsertedEvent(MonitoredApplications_ItemInserted);
            conns.MonitoredApplications.ItemChanged += new MonitoredApplications.ItemChangedEvent(MonitoredApplications_ItemChanged);

            RefreshUpper();
            RefreshLower();
            conns.IsDumpToFile = false;
            conns.Start();
            //listView2.Visible = false;
            splitContainer1.Panel2Collapsed = true;
        }

        private void hideLowerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //listView2.Visible = hideLowerToolStripMenuItem.Checked;
            splitContainer1.Panel2Collapsed = hideLowerToolStripMenuItem.Checked;
        }

        public void SelectAll(bool IsSelect)
        {
            foreach (ListViewItem item in listView3.Items)
            {
                item.Checked = IsSelect;
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SelectAll(true);
        }

        private void resetAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SelectAll(false);
        }

        private void FillProcessInfo(string ProcessName)
        {
            Process[] procs = Process.GetProcessesByName(ProcessName);
            listView4.Items.Clear();
            foreach (Process proc in procs)
            {
                try
                {
                    ListViewItem lvi = listView4.Items.Add("Process");
                    lvi.SubItems.Add(proc.ProcessName);
                    string str = proc.StartInfo.Arguments;
                    if (str != String.Empty)
                    {
                        lvi = listView4.Items.Add("Arguments");
                        lvi.SubItems.Add(str);
                    }
                    str = proc.StartInfo.UserName;
                    if (str != String.Empty)
                    {
                        lvi = listView4.Items.Add("UserName");
                        lvi.SubItems.Add(str);
                    }
                    str = proc.StartInfo.WorkingDirectory;
                    if (str != String.Empty)
                    {
                        lvi = listView4.Items.Add("WorkingDirectory");
                        lvi.SubItems.Add(str);
                    }
                    lvi = listView4.Items.Add("ProcessID");
                    lvi.SubItems.Add(proc.Id.ToString());
                    lvi = listView4.Items.Add("FileName");
                    lvi.SubItems.Add(proc.MainModule.FileName);
                    lvi = listView4.Items.Add("StartTime");
                    lvi.SubItems.Add(proc.StartTime.ToString());
                    lvi = listView4.Items.Add("ModuleMemorySize");
                    lvi.SubItems.Add(proc.MainModule.ModuleMemorySize.ToString());
                    lvi = listView4.Items.Add("Modules");
                    string tmp = String.Empty;
                    string prefix = String.Empty;
                    foreach (ProcessModule pm in proc.Modules)
                    {
                        lvi = listView4.Items.Add(String.Empty);
                        lvi.SubItems.Add(pm.ModuleName);
                        lvi.SubItems.Add(pm.FileVersionInfo.FileVersion);
                        /*tmp += prefix + pm.ModuleName;
                        if (prefix == String.Empty)
                            prefix = ", ";*/
                    }
                    lvi.SubItems.Add(tmp);
                    lvi = listView4.Items.Add("");
                }
                catch { }
            }
        }

        private void listView3_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.CurrentValue != e.NewValue)
            {
                conns.MonitoredApplications[e.Index].IsMonitored = (e.NewValue == CheckState.Checked);
            }
            //MessageBox.Show("Current value: " + e.CurrentValue.ToString() + "\n" +
              //              "New value: " + e.NewValue.ToString());
        }

        private void clearValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView3.SelectedIndices[0] != -1)
            {
                conns.MonitoredApplications[listView3.SelectedIndices[0]].ClearValues();
                conns.MonitoredApplications.ItemChangedEventHandler(null, listView3.SelectedIndices[0]);
            }
        }

        private void listView3_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                FillProcessInfo(conns.MonitoredApplications[e.ItemIndex].ProcessName);
            }
        }

        private int colUploadingSpeedWidth = 100;
        private void uploadingSpeedView_Click(object sender, EventArgs e)
        {
            if (uploadingSpeedView.Checked)
                listView3.Columns[colUploadingSpeed.Index].Width = colUploadingSpeedWidth;
            else
            {
                colUploadingSpeedWidth = listView3.Columns[colUploadingSpeed.Index].Width;
                listView3.Columns[colUploadingSpeed.Index].Width = 0;
            }
        }

        private int colDownloadingSpeedWidth = 100;
        private void downloadingSpeedView_Click(object sender, EventArgs e)
        {
            if (downloadingSpeedView.Checked)
                listView3.Columns[colDownloadingSpeed.Index].Width = colDownloadingSpeedWidth;
            else
            {
                colDownloadingSpeedWidth = listView3.Columns[colDownloadingSpeed.Index].Width;
                listView3.Columns[colDownloadingSpeed.Index].Width = 0;
            }

        }

        private void trafficPriceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPricePerMeg price_dlg = new frmPricePerMeg();
            price_dlg.DownloadingPricePerMegabyte = conns.DownloadPricePerMegabyte;
            price_dlg.UploadingPricePerMegabyte = conns.UploadPricePerMegabyte;
            if (price_dlg.ShowDialog() == DialogResult.OK)
            {
                conns.DownloadPricePerMegabyte = price_dlg.DownloadingPricePerMegabyte;
                conns.UploadPricePerMegabyte = price_dlg.UploadingPricePerMegabyte;
            }
        }

        private int colUploadedWidth = 100;
        private void uploadedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (uploadedToolStripMenuItem.Checked)
                listView3.Columns[colUploadedProc.Index].Width = colUploadedWidth;
            else
            {
                colUploadedWidth = listView3.Columns[colUploadedProc.Index].Width;
                listView3.Columns[colUploadedProc.Index].Width = 0;
            }
        }

        private int colPacketsOutWidth = 100;
        private void packetsOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (packetsOutToolStripMenuItem.Checked)
                listView3.Columns[colPacketsOutProc.Index].Width = colPacketsOutWidth;
            else
            {
                colPacketsOutWidth = listView3.Columns[colPacketsOutProc.Index].Width;
                listView3.Columns[colPacketsOutProc.Index].Width = 0;
            }
        }
    }
}