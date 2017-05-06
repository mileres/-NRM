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
            // Print SharpPcap version  SharpPcap �İ汾
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}, Example3.BasicCap.cs", ver);

            // Retrieve the device list  ��� �豸 �б�
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

            // Register our handler function to the 'packet arrival' event  ��������ע�ᵽ���ݰ������¼���ί�С�
            device.OnPacketArrival += 
                new PacketArrivalEventHandler( device_OnPacketArrival );//PacketArrivalEventHandler : a delegate for 'packet arrival' event

            // Open the device for capturing ���豸����
            int readTimeoutMilliseconds = 1000; //����
            if (device is AirPcapDevice)
            {
                // NOTE: AirPcap devices cannot disable local capture   AirPcap���ܽ�ֹ���ز���
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
                livePcapDevice.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);   //����ģʽ
            }
            else
            {
                throw new System.InvalidOperationException("unknown device type of " + device.GetType().ToString());
            }

            Console.WriteLine();
            Console.WriteLine("-- Listening on {0} {1}, hit 'Enter' to stop...",
                device.Name, device.Description);

            // Start the capturing process �����������
            device.StartCapture();

            // Wait for 'Enter' from the user.  �ȴ��û�������Enter��
            Console.ReadLine();

            // Stop the capturing process  ֹͣ�������
            device.StopCapture();

            Console.WriteLine("-- Capture stopped.");

            // Print out the device statistics    ��ӡ�豸ͳ��
            Console.WriteLine(device.Statistics.ToString());

            // Close the pcap device  �ر� Pcap װ��
            device.Close();

            Console.Read();
        }

        /// <summary>
        /// Prints the time and length of each received packet    ��ӡÿ�����յ������ݰ���ʱ��ͳ���
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
