using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerManagement
{
    internal class NativePowerManagementInterop
    {
        [DllImport("powrprof.dll")]
        public static extern uint CallNtPowerInformation(
            int informationLevel,
            IntPtr lpInputBuffer,
            uint nInputBufferSize,
            out SYSTEM_POWER_INFORMATION lpOutputBuffer,
            int nOutputBufferSize
            );

        [DllImport("powrprof.dll")]
        public static extern uint CallNtPowerInformation(
            int informationLevel,
            IntPtr lpInputBuffer,
            uint nInputBufferSize,
            out SYSTEM_BATTERY_STATE lpOutputBuffer,
            int nOutputBufferSize
        );

        [DllImport("powrprof.dll")]
        public static extern uint CallNtPowerInformation(
            int informationLevel,
            IntPtr lpInputBuffer,
            uint nInputBufferSize,
            out ulong lpOutputBuffer,
            int nOutputBufferSize
        );

        [DllImport("powrprof.dll")]
        public static extern uint CallNtPowerInformation(
            int informationLevel,
            ref bool lpInputBuffer,
            int nInputBufferSize,
            IntPtr lpOutputBuffer,
            int nOutputBufferSize
        );

        [DllImport("Powrprof.dll")]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
    }
    enum InformationLevel
    {
        SystemBatteryState = 5,
        SystemReserveHiberFile = 10,
        SystemPowerInformation = 12,
        LastWakeTime = 14,
        LastSleepTime = 15
    }
}
