using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Process17
{
    class Program
    {
        static void Main(string[] args)
        {
            Process[] pro =  Process.GetProcesses();
            foreach(Process item in pro)
            {
                Console.WriteLine(item.Id);
                
            }
            Console.Read();
        }
    }
}
