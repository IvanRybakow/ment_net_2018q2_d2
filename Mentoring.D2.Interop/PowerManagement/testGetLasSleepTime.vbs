set pm = CreateObject("PowerManagement.PowerManager")

a = pm.GetLastSleepTime()

WScript.Echo(a)