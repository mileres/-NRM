using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace ZhuaBao
{
    class Program
    {
        public class Test
        {
            public int a { get; set; }
            public int b { get; set; }
            public int c { get; set; }
            public int d { get; set; }
            public int e { get; set; }
            public int f { get; set; }
            public int g { get; set; }
            public int h { get; set; }
            public static Test T1 { get; set; }

        }
        static void Main(string[] args)
        {
            //NetworkInterface[] arrInterface = NetworkInterface.GetAllNetworkInterfaces();
            //foreach(NetworkInterface item in arrInterface)
            //{
            //    Console.WriteLine("接口ID：{0}", item.Id);
            //    Console.WriteLine("接口描述：{0}", item.Description);

            //    Console.WriteLine("当前实例的类型：{0}", item.GetType());
            //    Console.WriteLine("接口类型：{0}", item.NetworkInterfaceType);
            //    Console.WriteLine("！网络接口速度：{0}", item.Speed);

            //    Console.Read();

            //}
            Test t = new Test();
            t.a = 1;
            Test.T1 = t;
            
            Console.WriteLine( Test.T1.a);
            Console.Read();

           
        }
    }
}
