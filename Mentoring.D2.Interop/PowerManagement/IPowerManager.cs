using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerManagement
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("09BB9165-1998-4C16-9ADF-6EC0E1260B42")]
    public interface IPowerManager
    {
        SYSTEM_POWER_INFORMATION GetSystemPowerInfo();
        SYSTEM_BATTERY_STATE GetSystemBatteryState();
        string GetLastWakeTime();
        string GetLastSleepTime();
        void ChangeHibernateFileState(bool flag);
        void SetSuspendState();
        void SetHibernateState();

    }
}
