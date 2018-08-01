using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentoring.D2.Debugging
{
    class Program
    {
        static void Main(string[] args)
        {
            var netInterface = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault();
            if (netInterface == null)
            {
                Console.WriteLine("empty");
                Console.ReadKey();
                return;
            }
            var bytes1 = netInterface.GetPhysicalAddress().GetAddressBytes();
            var bytes2 = BitConverter.GetBytes(DateTime.Now.Date.ToBinary());
            var key = bytes1.Select((item, index) => (item ^ bytes2[index])).Select(item => item <= 999 ? item * 10 : item);
            Console.WriteLine(string.Join("-", key.Select(item => item.ToString())));
            Console.ReadKey();
        }
    }
}
