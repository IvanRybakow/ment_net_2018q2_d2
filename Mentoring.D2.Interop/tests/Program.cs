using PowerManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var mgr = new PowerManager();

            var batteryInfo = mgr.GetSystemBatteryState();
            Console.WriteLine(batteryInfo.Charging);
            var systemInfo = mgr.GetSystemPowerInfo();
            Console.WriteLine(systemInfo.CoolingMode);

            Console.ReadKey();
        }
    }
}
