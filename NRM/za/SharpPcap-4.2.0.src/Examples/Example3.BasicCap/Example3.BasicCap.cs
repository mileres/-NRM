using System;
using System.Collections.Generic;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.AirPcap;
using SharpPcap.WinPcap;

namespace Example3
{
    /// <summary>
    /// Basic capture example
    /// </summary>
    public class BasicCap
    {
        public static void Main(string[] args)
        {
            // Print SharpPcap version  SharpPcap 的版本
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}, Example3.BasicCap.cs", ver);

            // Retrieve the device list  检测 设备 列表
            var devices = CaptureDeviceList.Instance;

            // If no devices were found print an error 
            if(devices.Count < 1) 
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i = 0;

            // Print out the devices  
            foreach(var dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse( Console.ReadLine() );

            var device = devices[i];

            // Register our handler function to the 'packet arrival' event  将处理函数注册到数据包到达事件。委托。
            device.OnPacketArrival += 
                new PacketArrivalEventHandler( device_OnPacketArrival );//PacketArrivalEventHandler : a delegate for 'packet arrival' event

            // Open the device for capturing 打开设备捕获
            int readTimeoutMilliseconds = 1000; //毫秒
            if (device is AirPcapDevice)
            {
                // NOTE: AirPcap devices cannot disable local capture   AirPcap不能禁止本地捕获
                var airPcap = device as AirPcapDevice;
                airPcap.Open(SharpPcap.WinPcap.OpenFlags.DataTransferUdp, readTimeoutMilliseconds);
            }
            else if(device is WinPcapDevice)
            {
                var winPcap = device as WinPcapDevice;
                winPcap.Open(SharpPcap.WinPcap.OpenFlags.DataTransferUdp | SharpPcap.WinPcap.OpenFlags.NoCaptureLocal, readTimeoutMilliseconds);
            }
            else if (device is LibPcapLiveDevice)
            {
                var livePcapDevice = device as LibPcapLiveDevice;
                livePcapDevice.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);   //混杂模式
            }
            else
            {
                throw new System.InvalidOperationException("unknown device type of " + device.GetType().ToString());
            }

            Console.WriteLine();
            Console.WriteLine("-- Listening on {0} {1}, hit 'Enter' to stop...",
                device.Name, device.Description);

            // Start the capturing process 开启捕获程序
            device.StartCapture();

            // Wait for 'Enter' from the user.  等待用户操作“Enter”
            Console.ReadLine();

            // Stop the capturing process  停止捕获程序
            device.StopCapture();

            Console.WriteLine("-- Capture stopped.");

            // Print out the device statistics    打印设备统计
            Console.WriteLine(device.Statistics.ToString());

            // Close the pcap device  关闭 Pcap 装置
            device.Close();

            Console.Read();
        }

        /// <summary>
        /// Prints the time and length of each received packet    打印每个接收到的数据包的时间和长度
        /// </summary>
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var time = e.Packet.Timeval.Date;
            var len = e.Packet.Data.Length;
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}", 
                time.Hour, time.Minute, time.Second, time.Millisecond, len);
            Console.WriteLine(e.Packet.ToString());
        }
    }
}
