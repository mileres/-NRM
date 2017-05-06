using SharpPcap.WinPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SharpPcap01
{
    class Program
    {
        static void Main(string[] args)
        {

        }
    }

    //记录特定进程性能信息的类
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
        public void Dispose()
        {
            foreach (ICaptureDevice d in dev)
            {
                d.StopCapture();
                d.Close();
            }
        }
       public ProcessPerformanceInfo ProcInfo { get; set; }
        public int[]  GetPorts()
        {
            //进程id
            int pid = ProcInfo.ProcessID;
            //存放进程使用的端口号链表
            int[] ports = new int[10];

            //获取指定进程对应端口号
            Process pro = new Process();
            //StartInfo 获取要传递给Process.start（）的属性。
            pro.StartInfo.FileName = "cmd.exe";
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true;
            pro.StartInfo.RedirectStandardError = true;
            pro.StartInfo.CreateNoWindow = true;
            pro.Start();
            pro.StandardInput.WriteLine("netstat -ano");//获取用于读取应用程序输入的流。
            pro.StandardInput.WriteLine("exit");
            Regex reg = new Regex("\\s+", RegexOptions.Compiled);
            string line = null;
            ports.Clear();
            while ((line = pro.StandardOutput.ReadLine()) != null)  //获取用于读取应用程序输出的流。
            {
                line = line.Trim(); //移除前部空字符串和后部空字符串。
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
        }
    }

    

    //获取



}

