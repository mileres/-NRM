using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using DevExpress.XtraCharts;
using System.Text.RegularExpressions;
using System.Net;
using SharpPcap;
using PacketDotNet;
using System.IO;

namespace Application_Monitor
{
    public partial class Monitor : Form
    {
        #region 构造函数 以及 基本方法
        public Monitor()
        {
            InitializeComponent();
        }

        ProcessPerformanceInfo ProcInfo;
        const int KB_DIV = 1024;
        const int MB_DIV = 1024 * 1024;
        const int GB_DIV = 1024 * 1024 * 1024;
        #endregion

        #region 显示本机内存CPU网络信息
        private void ShowSystemInfo(int processID)
        {
            try
            {
                //获取当前进程对象
                Process cur = Process.GetProcessById(processID);
                string instanceName = GetInstanceName(cur);

                InitialCurrentMonitor(cur);
                ProcInfo.InstanceName = instanceName;

                PerformanceCounter curpcp = new PerformanceCounter("Process", "Working Set - Private", instanceName);
                PerformanceCounter curpc = new PerformanceCounter("Process", "Working Set", instanceName);
                PerformanceCounter curtime = new PerformanceCounter("Process", "% Processor Time", instanceName);

                PerformanceCounter curIORead = new PerformanceCounter("Process", "IO Read Bytes/sec", instanceName);
                PerformanceCounter curIOWrite = new PerformanceCounter("Process", "IO Write Bytes/sec", instanceName);

                PerformanceCounter totalcpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                PerformanceCounter totalIORead = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
                PerformanceCounter totalIOWrite = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");

                //上次记录CPU的时间
                TimeSpan prevCpuTime = TimeSpan.Zero;
                //Sleep的时间间隔
                int interval = 1000;



                SystemInfo sys = new SystemInfo();

                StringBuilder stringContent = new StringBuilder();
                Dictionary<string, decimal> dicDataCollection = new Dictionary<string, decimal>();
                while (true)
                {
                    if (bgw_MonitorPCStatus.CancellationPending)
                    {
                        return;
                    }

                    stringContent.Clear();
                    //第一种方法计算CPU使用率
                    //当前时间
                    TimeSpan curCpuTime = cur.TotalProcessorTime;
                    //计算
                    double value = (curCpuTime - prevCpuTime).TotalMilliseconds / interval / Environment.ProcessorCount * 100;
                    prevCpuTime = curCpuTime;

                    ProcInfo.WorkingSet = (decimal)curpc.NextValue() / 1024;
                    ProcInfo.ProcessMemory = (decimal)curpc.NextValue() / 1024;
                    ProcInfo.ProcessMemoryPercent = Math.Round((decimal)(curpc.NextValue() / 1024) / sys.PhysicalMemory, 10) * 100;
                    ProcInfo.CpuTime = Math.Round((decimal)value, 10);
                    ProcInfo.PrivateWorkingSet = (decimal)curpcp.NextValue() / 1024;
                    ProcInfo.PCCurrentMemory = (sys.PhysicalMemory - sys.MemoryAvailable);
                    ProcInfo.PCCpuTime = (decimal)sys.CpuLoad;
                    ProcInfo.PCCurrentMemoryPercent = Math.Round((decimal)(sys.PhysicalMemory - sys.MemoryAvailable) / sys.PhysicalMemory, 10) * 100;


                    stringContent.AppendLine(string.Format("{0}:{1}  {2:N}KB", cur.ProcessName, "工作集        ", ProcInfo.WorkingSet));//这个工作集是动态更新的
                    stringContent.AppendLine(string.Format("系统内存占比：{0}%", ProcInfo.ProcessMemoryPercent));
                    stringContent.AppendLine(string.Format("CPU使用率：{0}%", ProcInfo.CpuTime));
                    stringContent.AppendLine(string.Format("{0}:{1}  {2:N}KB", cur.ProcessName, "私有工作集    ", ProcInfo.PrivateWorkingSet));
                    stringContent.AppendLine(string.Format("\r系统CPU使用率：{0}%\r\n系统内存使用大小：{1}MB", ProcInfo.PCCpuTime, ProcInfo.PCCurrentMemory / MB_DIV));
                    stringContent.AppendLine(string.Format("系统内存占用率:{0}%", ProcInfo.PCCurrentMemoryPercent));



                    stringContent.AppendLine(string.Format("proc NetSendBytes : {0:N} B", ProcInfo.NetSendBytes));
                    stringContent.AppendLine(string.Format("proc NetRecvBytes : {0:N} B", ProcInfo.NetRecvBytes));

                    ProcInfo.CalInternetSpeed(1);

                    stringContent.AppendLine(string.Format("proc NetSendBytes Speed : {0:N} KB/s", ProcInfo.SendSpeed));
                    stringContent.AppendLine(string.Format("proc NetSend Data BrandWidth : {0} Mbits", ProcInfo.SendBrandWidth));

                    stringContent.AppendLine(string.Format("proc NetRecvBytes Speed: {0:N} KB/s", ProcInfo.RecSpeed));
                    stringContent.AppendLine(string.Format("proc NetRecv Data BrandWidth : {0} Mbits", ProcInfo.RecBrandWidth));

                    stringContent.AppendLine(ProcInfo.GetPortAndSourceAddressRecBytesReuslt());
                    stringContent.AppendLine(ProcInfo.GetPortAndSourceAddressSendBytesReuslt());

                    stringContent.AppendLine("");

                    stringContent.AppendLine(string.Format("proc IO Read:{0} KB/s", curIORead.NextValue() / KB_DIV));
                    stringContent.AppendLine(string.Format("proc IO Write:{0} KB/s", curIOWrite.NextValue() / KB_DIV));

                    stringContent.AppendLine(string.Format("系统 IO Read:{0} KB/s", totalIORead.NextValue() / KB_DIV));
                    stringContent.AppendLine(string.Format("系统 IO Write:{0} KB/s", totalIOWrite.NextValue() / KB_DIV));

                    ProcInfo.CollectionSnap();
                    TextInvokeMethod(txt_Content, stringContent.ToString());

                    Thread.Sleep(interval);
                }
            }
            catch
            {
 
            }
        }

        private void ShowNetworkInfo(int processID)
        {
            try
            {
                //获取当前进程对象
                Process cur = Process.GetProcessById(processID);
                string instanceName = GetInstanceName(cur);

                InitialCurrentMonitor(cur);
                ProcInfo.InstanceName = instanceName;

                #region 开始抓包
                InterMonitorRefresh(cur.Id);
                #endregion

                int interval = 1000;
                StringBuilder stringContent = new StringBuilder();
                while (true)
                {
                    if (bgw_MonitorNetwork.CancellationPending)
                        break;

                    stringContent.Clear();

                    List<int> newPorts = ProcInfo.CheckNewPorts();

                    if (newPorts.Count > 0)
                    {
                        StartCapture(ProcInfo.CurrentIP, newPorts, ProcInfo.deviceCount);
                    }

                    Thread.Sleep(interval);
                }
            }
            catch
            {
 
            }
        }

        private void InitialCurrentMonitor(Process cur)
        {
            if (ProcInfo == null)
            {
                ProcInfo = new ProcessPerformanceInfo();
                ProcInfo.IntervalOfMonitor = 60;
                ProcInfo.ProcessID = cur.Id;
                ProcInfo.ProcessName = cur.ProcessName;
            }
        }
        #endregion

        #region 文本框显示代理
        private void Monitor_Shown(object sender, EventArgs e)
        {

        }

        delegate void TextInvoke(TextBox text, string content);

        private void TextInvokeMethod(TextBox text, string content)
        {
            if (text.InvokeRequired)
            {
                TextInvoke ti = new TextInvoke(TextInvokeMethod);

                text.Invoke(ti, new object[] { text, content });
            }
            else
            {
                text.Text = content;
            }
        }

        delegate void ChartInvoke(ChartControl chart, Dictionary<string, decimal> dicData);


        #endregion

        #region 事件处理
        private void bgw_ShowInfo_DoWork(object sender, DoWorkEventArgs e)
        {
            int processId = Convert.ToInt32(e.Argument);
            ShowSystemInfo(processId);
        }

        private void bgw_ShowInfo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ProcInfo != null)
                ProcInfo.Dispose();
            cbo_Process.Enabled = true;
        }

        private void device_OnPacketArrivalSend(object sender, CaptureEventArgs e)
        {
            var len = e.Packet.Data.Length;
            EthernetPacket packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data) as EthernetPacket;

            var udpPacket = PacketDotNet.UdpPacket.GetEncapsulated(packet);
            var ipPacket = PacketDotNet.IpPacket.GetEncapsulated(packet);
            var tcpPacket = PacketDotNet.TcpPacket.GetEncapsulated(packet);
            if (udpPacket != null)
                ProcInfo.CalPortAndSourceAddressSendBytes(udpPacket.SourcePort, ipPacket.DestinationAddress.ToString(), len);
            if (tcpPacket != null)
                ProcInfo.CalPortAndSourceAddressSendBytes(tcpPacket.SourcePort, ipPacket.DestinationAddress.ToString(), len);

            ProcInfo.NetSendBytes += len;
        }

        private void device_OnPacketArrivalRecv(object sender, CaptureEventArgs e)
        {
            var len = e.Packet.Data.Length;
            var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);

            var udpPacket = PacketDotNet.UdpPacket.GetEncapsulated(packet);
            var ipPacket = PacketDotNet.IpPacket.GetEncapsulated(packet);
            var tcpPacket = PacketDotNet.TcpPacket.GetEncapsulated(packet);
            if (udpPacket != null)
                ProcInfo.CalPortAndSourceAddressRecBytes(udpPacket.DestinationPort, ipPacket.SourceAddress.ToString(), len);
            if (tcpPacket != null)
                ProcInfo.CalPortAndSourceAddressRecBytes(tcpPacket.DestinationPort, ipPacket.SourceAddress.ToString(), len);

            ProcInfo.NetRecvBytes += len;
        }

        private void bgw_MonitorNetwork_DoWork(object sender, DoWorkEventArgs e)
        {
             int processId = Convert.ToInt32(e.Argument);
             ShowNetworkInfo(processId);
        }

        private void bgw_MonitorNetwork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        #endregion

        #region 业务处理方法
        private string GetInstanceName(Process cur)
        {
            Process[] processes = Process.GetProcessesByName(cur.ProcessName);

            string instanceName = string.Empty;
            List<Process> processList = processes.OrderBy(s => s.StartTime).ToList();

            if (processList.Count > 1)
            {
                for (int i = 0; i < processList.Count; i++)
                {
                    if (processList[i].Id == cur.Id)
                    {
                        if (i == 0)
                        {
                            instanceName = processList[i].ProcessName;
                        }
                        else
                            instanceName = processList[i].ProcessName + "#" + (i).ToString();
                    }
                }
            }
            else
                instanceName = cur.ProcessName;

            return instanceName;
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            if (bgw_MonitorPCStatus.IsBusy && !bgw_MonitorPCStatus.CancellationPending)
            {
                bgw_MonitorPCStatus.CancelAsync();
            }

            if (bgw_MonitorNetwork.IsBusy && !bgw_MonitorNetwork.CancellationPending)
            {
                bgw_MonitorNetwork.CancelAsync();
            }
        }

        private void btn_Monitor_Click(object sender, EventArgs e)
        {
            if (cbo_Process.SelectedValue == null)
                return;

            if (bgw_MonitorPCStatus.IsBusy)
                return;
            cbo_Process.Enabled = false;
            bgw_MonitorPCStatus.WorkerSupportsCancellation = true;
            bgw_MonitorPCStatus.RunWorkerAsync(cbo_Process.SelectedValue);

            bgw_MonitorNetwork.WorkerSupportsCancellation = true;
            bgw_MonitorNetwork.RunWorkerAsync(cbo_Process.SelectedValue);
        }

        private void btn_LoadProcess_Click(object sender, EventArgs e)
        {
            if (!cbo_Process.Enabled)
                return;

            DataTable dtProcess = GetProcessTable();

            foreach (Process item in Process.GetProcesses())
            {
                if (item.Id == 0)
                    continue;
                DataRow drNew = dtProcess.Rows.Add();

                drNew["ID"] = item.Id;
                drNew["NAME"] = item.Id.ToString() + " " + item.ProcessName + " " + item.MainWindowTitle;
                drNew["SORT_NAME"] = item.ProcessName;
            }

            DataView dv = dtProcess.AsDataView();
            dv.Sort = "SORT_NAME,ID";

            cbo_Process.DataSource = dv;
            cbo_Process.ValueMember = "ID";
            cbo_Process.DisplayMember = "NAME";
        }

        private DataTable GetProcessTable()
        {
            DataTable dtResult = new DataTable();

            DataColumn dc = new DataColumn();
            dc.ColumnName = "ID";
            dc.DataType = typeof(int);
            dtResult.Columns.Add(dc);

            dc = new DataColumn();
            dc.ColumnName = "NAME";
            dc.DataType = typeof(string);
            dtResult.Columns.Add(dc);

            dc = new DataColumn();
            dc.ColumnName = "SORT_NAME";
            dc.DataType = typeof(string);
            dtResult.Columns.Add(dc);

            return dtResult;
        }

        private List<int> GetProcessPorts(int pid)
        {
            //进程id
            //int pid = ProcInfo.ProcessID;
            //存放进程使用的端口号链表
            List<int> ports = new List<int>();

            try
            {
                #region 获取指定进程对应端口号
                Process pro = new Process();
                pro.StartInfo.FileName = "cmd.exe";
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.RedirectStandardInput = true;
                pro.StartInfo.RedirectStandardOutput = true;
                pro.StartInfo.RedirectStandardError = true;
                pro.StartInfo.CreateNoWindow = true;
                pro.Start();
                pro.StandardInput.WriteLine("netstat -ano");
                pro.StandardInput.WriteLine("exit");
                Regex reg = new Regex("\\s+", RegexOptions.Compiled);
                string line = null;
                ports.Clear();
                while ((line = pro.StandardOutput.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("TCP", StringComparison.OrdinalIgnoreCase))
                    {
                        line = reg.Replace(line, ",");
                        string[] arr = line.Split(',');
                        if (arr[4] == pid.ToString())
                        {
                            string soc = arr[1];
                            int pos = soc.LastIndexOf(':');
                            int pot = int.Parse(soc.Substring(pos + 1));
                            ports.Add(pot);
                        }
                    }
                    else if (line.StartsWith("UDP", StringComparison.OrdinalIgnoreCase))
                    {
                        line = reg.Replace(line, ",");
                        string[] arr = line.Split(',');
                        if (arr[3] == pid.ToString())
                        {
                            string soc = arr[1];
                            int pos = soc.LastIndexOf(':');
                            int pot = int.Parse(soc.Substring(pos + 1));
                            ports.Add(pot);
                        }
                    }
                }
                pro.Close();
                #endregion
            }
            catch
            { ports = new List<int>(); }

            return ports;
        }

        private List<string> GetMachineIPInfo()
        {
            //获取本机IP地址
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            string IP = string.Empty;

            foreach (var item in ipHostEntry.AddressList)
            {
                if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IP = item.ToString();
                    break;
                }
            }

            //获取本机网络设备
            var devices = CaptureDeviceList.Instance;
            int count = devices.Count;
            if (count < 1)
            {
                //Console.WriteLine("No device found on this machine");
                return new List<string>();
            }

            return new List<string>() { IP, count.ToString() };
        }

        public void CaptureFlowSend(string IP, int portID, int deviceID)
        {
            ICaptureDevice device = (ICaptureDevice)CaptureDeviceList.New()[deviceID];

            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrivalSend);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            string filter = "src host " + IP + " and src port " + portID;
            device.Filter = filter;
            device.StartCapture();
            ProcInfo.dev.Add(device);
        }

        public void CaptureFlowRecv(string IP, int portID, int deviceID)
        {
            ICaptureDevice device = CaptureDeviceList.New()[deviceID];
            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrivalRecv);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            string filter = "dst host " + IP + " and dst port " + portID;
            device.Filter = filter;
            device.StartCapture();
            ProcInfo.dev.Add(device);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IP">IP Address</param>
        /// <param name="ports">process ports</param>
        /// <param name="count">CaptureDeviceList count</param>
        private void StartCapture(string IP, List<int> ports, int count)
        {
            //开始抓包
            for (int i = 0; i < count; ++i)
            {
                for (int j = 0; j < ports.Count; ++j)
                {
                    CaptureFlowRecv(IP, ports[j], i);
                    CaptureFlowSend(IP, ports[j], i);
                }
            }
        }
        #endregion

        #region 网络流量监控

        private void InterMonitorRefresh(int processID)
        {
            if (ProcInfo == null)
            {
                ProcInfo = new ProcessPerformanceInfo();
            }

            List<int> ports = GetProcessPorts(processID);
            List<string> iPInfo = GetMachineIPInfo();

            if (iPInfo.Count == 0 || iPInfo.Count < 2)
                return;

            ProcInfo.ports = ports;
            ProcInfo.CurrentIP = iPInfo[0];
            ProcInfo.deviceCount = Convert.ToInt32(iPInfo[1]);
            StartCapture(ProcInfo.CurrentIP, ProcInfo.ports, ProcInfo.deviceCount);
        }

        #endregion

        #region 窗体事件
        private void Monitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bgw_MonitorPCStatus.IsBusy)
            {
                bgw_MonitorPCStatus.CancelAsync();
                e.Cancel = true;

                MessageBox.Show("Ending Task,Please try it again");
            }
        }
        #endregion

    }

    //记录特定进程性能信息的类
    public class ProcessPerformanceInfo : IDisposable
    {
        const int KB_DIV = 1024;
        const int MB_DIV = 1024 * 1024;
        const int GB_DIV = 1024 * 1024 * 1024;
        const decimal BrandWidthToSpeed = (decimal)1024 / 8;//KB/s
        const decimal SpeedToBrandWidth = (decimal)8 / 1024;//M

        public int ProcessID { get; set; }//进程ID
        public string ProcessName { get; set; }//进程名
        public string InstanceName { get; set; }//实例名

        public decimal PrivateWorkingSet { get; set; }//私有工作集(KB)
        public decimal WorkingSet { get; set; }//工作集(KB)
        public decimal CpuTime { get; set; }//CPU占用率(%)
        public decimal PCCpuTime { get; set; }//CPU占用率(%)
        public decimal IOOtherBytes { get; set; }//每秒IO操作（不包含控制操作）读写数据的字节数(KB)

        public decimal ProcessMemory { get; set; }//进程占用内存
        public decimal PCCurrentMemory { get; set; }//总占用内存
        public decimal ProcessMemoryPercent { get; set; }//进程占用内存百分比
        public decimal PCCurrentMemoryPercent { get; set; }//总占用内存百分比

        public int IOOtherOperations { get; set; }//每秒IO操作数（不包括读写）(个数)
        public long NetSendBytes { get; set; }//网络发送数据字节数
        public long NetRecvBytes { get; set; }//网络接收数据字节数
        public long NetTotalBytes { get; set; }//网络数据总字节数

        public long NetSendBytesOld { get; set; }//网络发送数据字节数
        public long NetRecvBytesOld { get; set; }//网络接收数据字节数

        public decimal SendSpeed { get; set; }//网络发送数据字节数
        public decimal RecSpeed { get; set; }//网络接收数据字节数

        public List<int> ports { get; set; }
        public int deviceCount{get;set;}
        public string CurrentIP { get; set; }

        public int IntervalOfMonitor { get; set; }
        /// <summary>
        /// 监控状态 由四位字符串组成。前两位为进程状态(CPU,Memory),后两位为PC状态
        /// 等级为3位 1：低 （<= 峰值一半） 2:中 （> 峰值一半 < 峰值） 3：高 （>= 峰值）
        /// </summary>
        public string StatusOfMonitor { get; set; }

        //峰值若为0 ，则默认为 70%
        public decimal PeakCpu { get; set; }
        public decimal PeakMemory { get; set; }

        public decimal PeakPCCpu { get; set; }
        public decimal PeakPCMemory { get; set; }

        public List<int> CheckNewPorts()
        {
            List<int> newPorts = GetProcessPorts(this.ProcessID);
            List<int> result = new List<int>();
            bool isExistsNew = newPorts.Any(s => !this.ports.Contains(s));

            if (isExistsNew)
            {
                result = newPorts.Where(s => !this.ports.Contains(s)).ToList();
                this.ports = new List<int>(newPorts);
            }

            return result;
        }

        private List<int> GetProcessPorts(int pid)
        {
            //进程id
            //int pid = ProcInfo.ProcessID;
            //存放进程使用的端口号链表
            List<int> ports = new List<int>();

            try
            {
                #region 获取指定进程对应端口号
                Process pro = new Process();
                pro.StartInfo.FileName = "cmd.exe";
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.RedirectStandardInput = true;
                pro.StartInfo.RedirectStandardOutput = true;
                pro.StartInfo.RedirectStandardError = true;
                pro.StartInfo.CreateNoWindow = true;
                pro.Start();
                pro.StandardInput.WriteLine("netstat -ano");
                pro.StandardInput.WriteLine("exit");
                Regex reg = new Regex("\\s+", RegexOptions.Compiled);
                string line = null;
                ports.Clear();
                while ((line = pro.StandardOutput.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("TCP", StringComparison.OrdinalIgnoreCase))
                    {
                        line = reg.Replace(line, ",");
                        string[] arr = line.Split(',');
                        if (arr[4] == pid.ToString())
                        {
                            string soc = arr[1];
                            int pos = soc.LastIndexOf(':');
                            int pot = int.Parse(soc.Substring(pos + 1));
                            if (!ports.Contains(pot))
                                ports.Add(pot);
                        }
                    }
                    else if (line.StartsWith("UDP", StringComparison.OrdinalIgnoreCase))
                    {
                        line = reg.Replace(line, ",");
                        string[] arr = line.Split(',');
                        if (arr[3] == pid.ToString())
                        {
                            string soc = arr[1];
                            int pos = soc.LastIndexOf(':');
                            int pot = int.Parse(soc.Substring(pos + 1));
                            if (!ports.Contains(pot))
                                ports.Add(pot);
                        }
                    }
                }
                pro.Close();
                #endregion
            }
            catch
            { ports = new List<int>(); }

            return ports;
        }

        public decimal SendBrandWidth
        {
            get 
            {
                return this.SendSpeed * SpeedToBrandWidth;
            }
        }//网络发送数据占用带宽
        public decimal RecBrandWidth 
        {
            get
            {
                return this.RecSpeed * SpeedToBrandWidth;
            }
        }//网络接收数据占用带宽

        public Dictionary<int, Dictionary<string, long>> dicPortAndSourceAddressSendBytes = new Dictionary<int, Dictionary<string, long>>();
        public Dictionary<int, Dictionary<string, long>> dicPortAndSourceAddressRecBytes = new Dictionary<int, Dictionary<string, long>>();

        public void CalPortAndSourceAddressSendBytes(int port, string DestinationAddress, long length)
        {
            if (!dicPortAndSourceAddressSendBytes.ContainsKey(port))
            {
                dicPortAndSourceAddressSendBytes.Add(port, new Dictionary<string, long>());
            }

            if (!dicPortAndSourceAddressSendBytes[port].ContainsKey(DestinationAddress))
            {
                dicPortAndSourceAddressSendBytes[port].Add(DestinationAddress, 0);
            }

            dicPortAndSourceAddressSendBytes[port][DestinationAddress] += length;
        }

        public void CalPortAndSourceAddressRecBytes(int port, string SourceAddress, long length)
        {
            if (!dicPortAndSourceAddressRecBytes.ContainsKey(port))
            {
                dicPortAndSourceAddressRecBytes.Add(port, new Dictionary<string, long>());
            }

            if (!dicPortAndSourceAddressRecBytes[port].ContainsKey(SourceAddress))
            {
                dicPortAndSourceAddressRecBytes[port].Add(SourceAddress, 0);
            }

            dicPortAndSourceAddressRecBytes[port][SourceAddress] += length;
        }

        public string GetPortAndSourceAddressSendBytesReuslt()
        {
            string result = string.Empty;

            foreach (var item in dicPortAndSourceAddressSendBytes.Keys)
            {
                foreach (var subItem in dicPortAndSourceAddressSendBytes[item].Keys)
                {
                    result += string.Format(" Port:{0},DestinationAddress:{1} Send Bytes:{2} Bytes"
                        , item, subItem, dicPortAndSourceAddressSendBytes[item][subItem]);
                    result += Environment.NewLine;
                }
            }

            return result;
        }

        public string GetPortAndSourceAddressRecBytesReuslt()
        {
            string result = string.Empty;

            foreach (var item in dicPortAndSourceAddressRecBytes.Keys)
            {
                foreach (var subItem in dicPortAndSourceAddressRecBytes[item].Keys)
                {
                    result += string.Format(" Port:{0},SourceAddress:{1} Rec Bytes:{2} Bytes"
                        , item, subItem, dicPortAndSourceAddressRecBytes[item][subItem]);
                    result += Environment.NewLine;
                }
            }

            return result;
        }


        public void CalInternetSpeed(decimal time)
        {
            if (time == 0)
                return;
            SendSpeed = Math.Round((NetSendBytes - NetSendBytesOld) / time / KB_DIV, 10);
            NetSendBytesOld = NetSendBytes;

            RecSpeed = Math.Round((NetRecvBytes - NetRecvBytesOld) / time / KB_DIV, 10);
            NetRecvBytesOld = NetRecvBytes;
        }

        public List<ICaptureDevice> dev = new List<ICaptureDevice>();

        /// <summary>
        /// 实现IDisposable的方法
        /// </summary>
        public void Dispose()
        {
            NetSendBytes = 0;
            NetRecvBytes = 0;
            NetTotalBytes = 0;
            foreach (ICaptureDevice d in dev)
            {
                d.StopCapture();
                d.Close();
            }

            dicPortAndSourceAddressSendBytes.Clear();
            dicPortAndSourceAddressRecBytes.Clear();
        }

        #region DataCollection
        Dictionary<DateTime, ProcessPerformanceInfo> dicCollectionSnap;

        public void CollectionSnap()
        {
            if (dicCollectionSnap == null)
                dicCollectionSnap = new Dictionary<DateTime, ProcessPerformanceInfo>();

            dicCollectionSnap.Add(DateTime.Now, this.Clone());

            if (dicCollectionSnap.Count == this.IntervalOfMonitor)
            {
                //计算平均值
                CalStatus();
                //将内容保存到磁盘。
                SaveLogInFile();
                //为了避免内存过大删除快照
                dicCollectionSnap = new Dictionary<DateTime, ProcessPerformanceInfo>();
            }
        }

        public void CalStatus()
        {
            //查询快照 60次的内容中平均值超过峰值,则给与不同状态，状态数字越大，风险等级越高
            decimal tmpProcCpu = dicCollectionSnap.Values.Sum(s => s.CpuTime) / this.IntervalOfMonitor;
            decimal tmpProcMemoryPercent = dicCollectionSnap.Values.Sum(s => s.PCCurrentMemoryPercent) / this.IntervalOfMonitor;

            decimal tmpPCCpu = dicCollectionSnap.Values.Sum(s => s.PCCpuTime) / this.IntervalOfMonitor;
            decimal tmpPCMemoryPercent = dicCollectionSnap.Values.Sum(s => s.PCCurrentMemoryPercent) / this.IntervalOfMonitor;

            this.StatusOfMonitor =
                CalStatusLevel(tmpProcCpu, this.PeakCpu) + 
                CalStatusLevel(tmpProcMemoryPercent,this.PeakMemory) +
                CalStatusLevel(tmpPCCpu, this.PeakPCCpu) + CalStatusLevel(tmpPCMemoryPercent,this.PeakPCMemory);
        }

        public string CalStatusLevel(decimal source,decimal standard)
        {
            if (standard == 0)
                standard = 70;

            if (source >= standard)
                return "3";
            else if (source < standard && source > (standard / 2))
                return "2";
            else
                return "1";
        }

        public void SaveLogInFile()
        {
            string Path = Application.StartupPath + "\\Monitor\\";
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string fileFullName = Path + fileName + ".txt";
            try
            {
                if (!Directory.Exists(Path))
                {
                    Directory.CreateDirectory(Path);
                }

                using (StreamWriter sw = new StreamWriter(fileFullName, true))
                {
                    sw.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(this));
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(fileFullName, true))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }
        #endregion

        public ProcessPerformanceInfo Clone()
        {
            ProcessPerformanceInfo obj = new ProcessPerformanceInfo();

            obj.ProcessID = this.ProcessID;
            obj.ProcessName = this.ProcessName;
            obj.PrivateWorkingSet = this.PrivateWorkingSet;
            obj.WorkingSet = this.WorkingSet;
            obj.CpuTime = this.CpuTime;
            obj.PCCpuTime = this.PCCpuTime;
            obj.IOOtherBytes = this.IOOtherBytes;
            obj.ProcessMemory = this.ProcessMemory;
            obj.PCCurrentMemory = this.PCCurrentMemory;
            obj.ProcessMemoryPercent = this.ProcessMemoryPercent;
            obj.PCCurrentMemoryPercent = this.PCCurrentMemoryPercent;
            obj.IOOtherOperations = this.IOOtherOperations;
            obj.NetSendBytes = this.NetSendBytes;
            obj.NetRecvBytes = this.NetRecvBytes;
            obj.NetTotalBytes = this.NetTotalBytes;
            obj.NetSendBytesOld = this.NetSendBytesOld;
            obj.NetRecvBytesOld = this.NetRecvBytesOld;
            obj.SendSpeed = this.SendSpeed;
            obj.RecSpeed = this.RecSpeed;

            obj.dicPortAndSourceAddressSendBytes = new Dictionary<int, Dictionary<string, long>>(this.dicPortAndSourceAddressSendBytes);
            obj.dicPortAndSourceAddressRecBytes = new Dictionary<int, Dictionary<string, long>>(this.dicPortAndSourceAddressRecBytes);
                        
            return obj;

        }
    }
}
