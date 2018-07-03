using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerManagement
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("69831BC3-44A7-4BC5-BE4E-136FA6560B06")]
    public class PowerManager : IPowerManager
    {
        public void ChangeHibernateFileState(bool flag)
        {
            NativePowerManagementInterop.CallNtPowerInformation(
                (int)InformationLevel.SystemReserveHiberFile,
                ref flag,
                Marshal.SizeOf(typeof(bool)),
                IntPtr.Zero,
                0);
        }

        public string GetLastSleepTime()
        {
            NativePowerManagementInterop.CallNtPowerInformation(
                (int)InformationLevel.LastSleepTime,
                IntPtr.Zero,
                0,
                out ulong lastSleepTime,
                Marshal.SizeOf(typeof(ulong)));
            return TimeSpan.FromTicks((long) lastSleepTime).ToString();
        }

        public String GetLastWakeTime()
        {
            NativePowerManagementInterop.CallNtPowerInformation(
                (int)InformationLevel.LastWakeTime,
                IntPtr.Zero,
                0,
                out ulong lastWakeTime,
                Marshal.SizeOf(typeof(ulong)));
            return TimeSpan.FromTicks((long)lastWakeTime).ToString();
        }

        public SYSTEM_BATTERY_STATE GetSystemBatteryState()
        {
            NativePowerManagementInterop.CallNtPowerInformation(
                (int)InformationLevel.SystemBatteryState,
                IntPtr.Zero,
                0,
                out SYSTEM_BATTERY_STATE sysBatteryInfo,
                Marshal.SizeOf(typeof(SYSTEM_BATTERY_STATE)));
            return sysBatteryInfo;
        }

        public SYSTEM_POWER_INFORMATION GetSystemPowerInfo()
        {
            NativePowerManagementInterop.CallNtPowerInformation(
                (int)InformationLevel.SystemPowerInformation,
                IntPtr.Zero,
                0,
                out SYSTEM_POWER_INFORMATION sysPowerInfo,
                Marshal.SizeOf(typeof(SYSTEM_POWER_INFORMATION)));
            return sysPowerInfo;
        }

        public void SetHibernateState()
        {
            NativePowerManagementInterop.SetSuspendState(true, false, false);
        }

        public void SetSuspendState()
        {
            NativePowerManagementInterop.SetSuspendState(false, false, false);
        }
    }
}
