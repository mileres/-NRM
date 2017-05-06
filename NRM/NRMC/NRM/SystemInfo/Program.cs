using SharpPcap;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;

namespace SystemInfo
{
    public class ProcessPerformanceInfo : IDisposable
    {
      public int ProcessID { get; set; }//进程ID
      public string ProcessName { get; set; }//进程名
      public float PrivateWorkingSet { get; set; }//私有工作集(KB)
      public float WorkingSet { get; set; }//工作集(KB)
      public float CpuTime { get; set; }//CPU占用率(%)
      public float IOOtherBytes { get; set; }//每秒IO操作（不包含控制操作）读写数据的字节数(KB)
      public int IOOtherOperations { get; set; }//每秒IO操作数（不包括读写）(个数)
      public long NetSendBytes { get; set; }//网络发送数据字节数
      public long NetRecvBytes { get; set; }//网络接收数据字节数
      public long NetTotalBytes { get; set; }//网络数据总字节数
      public List<ICaptureDevice> dev = new List<ICaptureDevice>();

        /// <summary>
        /// 实现IDisposable的方法
        /// </summary>
        public void Dispose()  //手动垃圾回收
        {
            foreach (ICaptureDevice d in dev)
            {
                d.StopCapture();
                d.Close();
            }
        }
        public ProcessPerformanceInfo ProcInfo { get; set; }
        public ProcessPerformanceInfo(int pid)
        {
            this.ProcessID = pid;
        }
        /// <summary>
        /// 获取进程端口号
        /// </summary>
        public List<int> GetProInterface() //获取进程端口
        {
            //进程id
            int pid = this.ProcessID;  //
            //存放进程使用的端口号链表
            List<int> ports = new List<int>();

           // 获取指定进程对应端口号
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
            return ports;

            //foreach(IpPort item in ports)
            //{
            //    Console.Write(item +"  ");
            //}
        }

        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        public string GetIPAddr()
        {
            //获取本机IP地址
            IPAddress[] addrList = Dns.GetHostByName(Dns.GetHostName()).AddressList;
            string IP = addrList[0].ToString();
            return IP;
        }

        public int GetDevicesCount()
        {
            //获取本机网络设备
            var devices = CaptureDeviceList.Instance;
            int count = devices.Count;
            if (count < 1)
            {
                Console.WriteLine("No device found on this machine");
                return 0;
            }
            else
            {
                return count;
            }
        }

        /// <summary>
        /// 获取端口发送的流
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="portID"></param>
        /// <param name="deviceID"></param>

        
        public List<ICaptureDevice> CaptureFlowSend(string IP, int portID, int deviceID)
        {
            ICaptureDevice device = (ICaptureDevice)CaptureDeviceList.New()[deviceID];

            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrivalSend);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            string filter = "src host " + IP + " and src port " + portID;
            device.Filter = filter;
            device.StartCapture();
            this.dev.Add(device);

            return dev;
        }

        /// <summary>
        /// 获取端口输出的流
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="portID"></param>
        /// <param name="deviceID"></param>
        public List<ICaptureDevice> CaptureFlowRecv(string IP, int portID, int deviceID)
        {
            ICaptureDevice device = CaptureDeviceList.New()[deviceID];
            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrivalRecv);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            string filter = "dst host " + IP + " and dst port " + portID;
            device.Filter = filter;
            device.StartCapture();
            this.dev.Add(device);
            return dev;
        }
        /// <summary>
        /// 当数据包到达的  上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void device_OnPacketArrivalSend(object sender, CaptureEventArgs e)
        {
            var len = e.Packet.Data.Length;
            this.NetSendBytes += len;
        }
        /// <summary>
        /// 当数据包达到 下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void device_OnPacketArrivalRecv(object sender, CaptureEventArgs e)
        {
            var len = e.Packet.Data.Length;
            this.NetRecvBytes += len;
        }

        public void GetNetInfo(int count, List<int> ports, string IP)
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
            Console.WriteLine("proc NetTotalBytes : " + this.NetTotalBytes);
            Console.WriteLine("proc NetSendBytes : " + this.NetSendBytes);
            Console.WriteLine("proc NetRecvBytes : " + this.NetRecvBytes);
        }
    }
    

    class Program
    {
        static void Main(string[] args)
        {
            //获取进程端口
            Process[] pros = Process.GetProcesses();
           
          
            for (int i = 0; i < pros.Length; i++)
            {
                ProcessPerformanceInfo proPerInfo = new ProcessPerformanceInfo(pros[i].Id);
                string cpIP = proPerInfo.GetIPAddr();
                List<int> proInterface = proPerInfo.GetProInterface();
                int devicescount = proPerInfo.GetDevicesCount();
                Console.WriteLine(pros[i].ProcessName);
                proPerInfo.GetNetInfo(devicescount, proInterface, cpIP);
                Console.WriteLine("----------------------------------------------------------------------------------------------");
            }
            
            

            //
            //开始抓包


            Console.Read();
        }
    }
}
