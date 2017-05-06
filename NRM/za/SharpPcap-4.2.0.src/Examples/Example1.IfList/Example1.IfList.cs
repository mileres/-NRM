using System;
using System.Collections.Generic;
using SharpPcap;

namespace Example1
{
    /// <summary>
    /// Obtaining the device list
    /// </summary>
    public class IfListAdv
    {
        /// <summary>
        /// Obtaining the device list
        /// </summary>
        public static void Main(string[] args)
        {
            // Print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}, Example1.IfList.cs", ver);

            // Retrieve the device list  ����豸�б�
            var devices = CaptureDeviceList.Instance;

            // If no devices were found print an error  ���û���豸����⵽�ʹ�ӡ һ������
            if(devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine("\nThe following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------\n");

            /* Scan the list printing every entry */ 
            //ɨ��ÿ���б���ӡÿ����Ŀ��
            foreach(var dev in devices)
                Console.WriteLine("{0}\n",dev.ToString());

            Console.Write("Hit 'Enter' to exit...");
            Console.ReadLine();
        }
    }
}
