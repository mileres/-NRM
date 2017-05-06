using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace NetWorkInfometion
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                bool Pd1 = (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet); //判断是否是以太网连接
                if (Pd1)
                {
                    Console.WriteLine("网络适配器名称：" + adapter.Name);
                    Console.WriteLine("网络适配器标识符：" + adapter.Id);
                    Console.WriteLine("适配器连接状态：" + adapter.OperationalStatus.ToString());


                    IPInterfaceProperties ip = adapter.GetIPProperties();     //IP配置信息
                    if (ip.UnicastAddresses.Count > 0)
                    {
                        Console.WriteLine("IP地址:" + ip.UnicastAddresses[0].Address.ToString());
                        Console.WriteLine("子网掩码:" + ip.UnicastAddresses[0].IPv4Mask.ToString());
                    }
                    if (ip.GatewayAddresses.Count > 0)
                    {
                        Console.WriteLine("默认网关:" + ip.GatewayAddresses[0].Address.ToString());   //默认网关
                    }
                    int DnsCount = ip.DnsAddresses.Count;
                    Console.WriteLine("DNS服务器地址：");   //默认网关
                    if (DnsCount > 0)
                    {
                        //其中第一个为首选DNS，第二个为备用的，余下的为所有DNS为DNS备用，按使用顺序排列
                        for (int i = 0; i < DnsCount; i++)
                        {
                            Console.WriteLine("              " + ip.DnsAddresses[i].ToString());
                        }
                    }
                    Console.WriteLine("网络接口速度：" + (adapter.Speed / 1000000).ToString("0.0") + "Mbps");
                    Console.WriteLine("接口描述：" + adapter.Description);
                    Console.WriteLine("适配器的媒体访问控制 (MAC) 地址:" + adapter.GetPhysicalAddress().ToString());

                    Console.WriteLine("该接口是否只接收数据包：" + adapter.IsReceiveOnly.ToString());

                    Console.WriteLine("该接口收到的字节数：" + adapter.GetIPv4Statistics().BytesReceived.ToString());
                    Console.WriteLine("该接口发送的字节数：" + adapter.GetIPv4Statistics().BytesSent.ToString());

                    Console.WriteLine("该接口丢弃的传入数据包数：" + adapter.GetIPv4Statistics().IncomingPacketsDiscarded.ToString());
                    Console.WriteLine("该接口丢弃的传出数据包数：" + adapter.GetIPv4Statistics().OutgoingPacketsDiscarded.ToString());

                    Console.WriteLine("该接口有错误的传入数据包数：" + adapter.GetIPv4Statistics().IncomingPacketsWithErrors.ToString());
                    Console.WriteLine("该接口有错误的传出数据包数：" + adapter.GetIPv4Statistics().OutgoingPacketsWithErrors.ToString());

                    Console.WriteLine("该接口协议未知的数据包数：" + adapter.GetIPv4Statistics().IncomingUnknownProtocolPackets.ToString());
                    Console.WriteLine("---------------------------------\n");
                }

            }
            Console.ReadLine();
        }
    }
}